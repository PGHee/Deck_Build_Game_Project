using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public BuffDebuffManager buffDebuffManager;
    public PlayerState player;
    public DeckManager deckManager;
    private List<MonsterState> monsters;
    private Actions cardActions;
    private SystemMessage message;
    private InvestCrystalManager investCrystal;
    public ArtifactManager artifactManager;
    public HandControl handController;
    public GameObject endTurnButton;

    private bool isPlayerTurn;
    public bool IsPlayerTurn => isPlayerTurn;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cardActions = FindObjectOfType<Actions>();
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
        message = FindObjectOfType<SystemMessage>();
        handController = FindObjectOfType<HandControl>();
        artifactManager = FindObjectOfType<ArtifactManager>();
        investCrystal = FindObjectOfType<InvestCrystalManager>();
        this.enabled = false;
    }

    public void StartBattle()
    {
        this.enabled = true;
        monsters = new List<MonsterState>(FindObjectsOfType<MonsterState>()); // 씬 내의 모든 몬스터를 가져옴
        foreach (var monster in monsters)
        {
            monster.GetRandomAction(); // 각 몬스터의 첫 행동 설정
        }
        StartPlayerTurn();
    }

    public void RegisterMonster(MonsterState monster)
    {
        if (!monsters.Contains(monster))
        {
            monsters.Add(monster);
        }
    }

    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        player.currentResource = player.resource;
        player.ApplyTurnBasedPassives();        // 패시브 효과 적용
        player.ApplyPoisonDamage();             // 독 데미지 적용
        buffDebuffManager.UpdateBuffs();        // 버프 업데이트
        buffDebuffManager.UpdateDebuffs();      // 디버프 업데이트

        artifactManager.ResetArtifactReady();

        player.UpdateHPBar();
        if (buffDebuffManager.entityDebuffs.ContainsKey(player.gameObject))
        {
            if (buffDebuffManager.entityDebuffs[player.gameObject].Any(debuff => debuff.Item1 == EffectType.SkipTurn)) EndPlayerTurn();
        }

        deckManager.TurnStartCard(); // draw cards when Pturn start

        message.ShowSystemMessage("플레이어 턴");    // 플레이어가 행동을 완료하면 턴 종료 버튼으로 EndPlayerTurn 호출
    }

    public void EndPlayerTurn()
    {
        if (isPlayerTurn)
        {
            handController.DiscardAllHand();
            isPlayerTurn = false;
            StartCoroutine(MonsterTurn());
            artifactManager.DeactivateArtfactReady();
            investCrystal.CheckInvest();
        }
    }

    IEnumerator MonsterTurn()
    {
        message.ShowSystemMessage("몬스터 턴 시작");
        CheckBondedRevive();
        foreach (var monster in monsters)
        {
            if (monster != null && monster.gameObject.activeInHierarchy)
            {
                if(buffDebuffManager.currentField == PlayerState.AttributeType.Wood) monster.ApplyPoison(3);
                yield return new WaitForSeconds(1);
                monster.ApplyPoisonDamage();            // 독 데미지와 스택 감소 적용
                if(monster.currentHealth > 0)
                {
                    if (buffDebuffManager.entityDebuffs.ContainsKey(monster.gameObject))
                    {
                        if (buffDebuffManager.entityDebuffs[monster.gameObject].Any(debuff => debuff.Item1 == EffectType.SkipTurn)) continue;
                    }
                    else if (monster.isStunned)
                    {
                        if(buffDebuffManager.currentField == PlayerState.AttributeType.Lightning) cardActions.DealSingleTargetDamage(monster.gameObject, 20);
                        monster.HandleStun();               // 스턴 상태 처리
                        continue;                           // 스턴 상태라면 행동을 스킵
                    }
                    monster.executeAction();
                    if(monster.selectedAction.actionType != ActionType.Wait) monster.AttackMotion();
                    yield return new WaitForSeconds(1);
                    monster.GetRandomAction();
                    monster.UpdateValueEffect();
                    monster.UpdateAction();
                    yield return new WaitForSeconds(1);
                }
            }
                
        }
        if(!monsters.TrueForAll(m => m.currentHealth <= 0)) StartPlayerTurn();
    }

    private void CheckBondedRevive()
    {
        List<MonsterState> bondedMonsters = FindObjectsOfType<MonsterState>().Where(m => m.passives.Any(p => p.passiveType == PassiveType.Bond)).ToList();
        bool allWaitingForDeath = bondedMonsters.All(m => m.gameObject.tag == "WaitingForDeath");   // 결속된 몬스터의 죽음 대기 확인

        if (allWaitingForDeath)
        {
            foreach (MonsterState monster in bondedMonsters)
            {
                monster.animator.SetTrigger("DieTrigger");
                Destroy(monster.hpBar.gameObject);
            }
        }
        else
        {
            foreach (MonsterState monster in bondedMonsters.Where(m => m.gameObject.tag == "WaitingForDeath"))
            {
                monster.Heal(Mathf.RoundToInt(monster.maxHealth * 0.25f));
                monster.gameObject.tag = monster.originalTag;   // 태그를 원래 태그로 복구
                monster.UpdateHPBar();                          // 부활 후 HP 바 업데이트
            }
        }
    }

    public void CheckBattleEnd()
    {
        if (player.currentHealth <= 0)
        {
            Debug.Log("Player is defeated. Battle ended.");
            // 전투 종료 처리 (패배)
            this.enabled = false;
        }
        else if (monsters.TrueForAll(m => m.currentHealth <= 0))
        {
            message.ShowSystemMessage("승리!");

            endTurnButton.SetActive(false);
            buffDebuffManager.RemoveCurrentField();
            handController.DiscardAllHand(); // 핸드 비움
            investCrystal.ClearInvest(); // 투자 정보 삭제

            player.ResetPlayerStateAfterBattle();

            GameObject popupManager = GameObject.Find("PopupManager");
            popupManager.GetComponent<PopupManager>().ShowPopup("BattleReward");

            GameObject battleRewardManager = GameObject.Find("BattleRewardManager");
            battleRewardManager.GetComponent<BattleRewardManager>().rewardCrystal = monsters.Count;
            battleRewardManager.GetComponent<BattleRewardManager>().rewardEXP = monsters.Count;
            battleRewardManager.GetComponent<BattleRewardManager>().RestartBattleReward();

            this.enabled = false;
        }
    }
}
