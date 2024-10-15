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
    private PopupManager popupManager;
    private DeckListManager deckListManager;
    public ArtifactManager artifactManager;
    public HandControl handController;
    public GameObject endTurnButton;
    private GameDirector gameDirector;

    private bool isPlayerTurn;
    public bool IsPlayerTurn => isPlayerTurn;
    public string isSearchEnd;
    private int turnCount;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        cardActions = FindObjectOfType<Actions>();
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
        message = FindObjectOfType<SystemMessage>();
        handController = FindObjectOfType<HandControl>();
        artifactManager = FindObjectOfType<ArtifactManager>();
        investCrystal = FindObjectOfType<InvestCrystalManager>();
        popupManager = FindObjectOfType<PopupManager>();
        deckListManager = FindObjectOfType<DeckListManager>();
        gameDirector = FindObjectOfType<GameDirector>();
        this.enabled = false;
    }

    public void StartBattle()
    {
        this.enabled = true;
        turnCount = 0;
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
        turnCount++;
        foreach (var monster in monsters)
        {
            if (monster != null && monster.gameObject.activeInHierarchy && player != null)
            {
                if(turnCount == 5)
                {
                    if(buffDebuffManager.entityDebuffs.ContainsKey(monster.gameObject))
                    {
                        if(!buffDebuffManager.entityBuffs[monster.gameObject].Any(buff => buff.Item1 == EffectType.Enrage))
                        {
                            buffDebuffManager.ApplyEnrage(monster.gameObject);
                        }
                    }
                    else buffDebuffManager.ApplyEnrage(monster.gameObject);
                }
                else if(turnCount >= 6 && (turnCount-5) % 2 == 0)
                {
                    buffDebuffManager.ApplyEnrage(monster.gameObject);
                }
            }
        }
        
        player.currentResource = player.resource;
        player.ApplyTurnBasedPassives();        // 패시브 효과 적용
        player.ApplyPoisonDamage();             // 독 데미지 적용
        artifactManager.ResetArtifactReady();

        player.UpdateHPBar();
        if (buffDebuffManager.entityDebuffs.ContainsKey(player.gameObject))
        {
            if (buffDebuffManager.entityDebuffs[player.gameObject].Any(debuff => debuff.Item1 == EffectType.SkipTurn)) 
            {
                message.ShowSystemMessage("경직으로 인해 행동할 수 없습니다.");
                buffDebuffManager.UpdateBuffs();        // 버프 업데이트
                buffDebuffManager.UpdateDebuffs();      // 디버프 업데이트
                EndPlayerTurn();
                return;
            }
        }
        buffDebuffManager.UpdateBuffs();        // 버프 업데이트
        buffDebuffManager.UpdateDebuffs();      // 디버프 업데이트
        deckManager.TurnStartCard(); // draw cards when Pturn start
        if (artifactManager.bonusDraw > 0)
        {
            for (int i = 0; i < artifactManager.bonusDraw; i++)
            {
                deckManager.CardDraw();
            }
        }
        if (artifactManager.bonusShield > 0) player.ApplyShield(artifactManager.bonusShield);
        if (artifactManager.bonusHeal > 0) player.Heal(artifactManager.bonusHeal);
        message.ShowSystemMessage("플레이어 턴");    // 플레이어가 행동을 완료하면 턴 종료 버튼으로 EndPlayerTurn 호출
        StartCoroutine(CardSearchPhaseLight());
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
        yield return new WaitForSeconds(1);
        message.ShowSystemMessage("몬스터 턴");
        CheckBondedRevive();
        foreach (var monster in monsters)
        {
            if (monster != null && monster.gameObject.activeInHierarchy && player != null)
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
                    if (monster.currentHealth <= 0) continue;
                    monster.GetRandomAction();
                    monster.UpdateValueEffect();
                    monster.UpdateAction();
                    yield return new WaitForSeconds(1);
                }
            }
                
        }
        if(!monsters.TrueForAll(m => m.currentHealth <= 0)) StartPlayerTurn();
    }

    IEnumerator CardSearchPhaseLight()
    {
        isSearchEnd = "Light";
        
        if (player.attributeMastery[PlayerState.AttributeType.Light] >= 3 && player.attributeMastery[PlayerState.AttributeType.Light] < 6)
        {
            deckManager.CardDraw();
            TurnStartSearchSwitch();
        }
        else if (player.attributeMastery[PlayerState.AttributeType.Light] >= 6)
        {
            popupManager.ShowPopup("DeckList");
            deckListManager.CardListUp("DeckArray");
            yield return new WaitForSeconds(1);
            message.ShowSystemMessage("덱에서 빛 속성 카드 획득");
        }
        else
        {
            TurnStartSearchSwitch();
        }
    }

    IEnumerator CardSearchPhaseDark()
    {
        isSearchEnd = "Dark";
        if (player.attributeMastery[PlayerState.AttributeType.Dark] >= 3 && player.attributeMastery[PlayerState.AttributeType.Dark] < 6)
        {
            if (deckManager.deckArray.Length > 4 + artifactManager.bonusDraw && deckManager.graveArray.Length != 0)
            {
                yield return new WaitForSeconds(1);
                deckManager.CardSalvage(Random.Range(0, deckManager.graveArray.Length));
            }
            else
            {
                yield return new WaitForSeconds(1);
                message.ShowSystemMessage("묘지에 카드가 없습니다.");
            }
            TurnStartSearchSwitch();
        }
        else if (player.attributeMastery[PlayerState.AttributeType.Dark] >= 6)
        {
            if (deckManager.graveArray.Length != 0)
            {
                yield return new WaitForSeconds(1);
                popupManager.ShowPopup("DeckList");
                deckListManager.CardListUp("GraveArray");
                yield return new WaitForSeconds(1);
                message.ShowSystemMessage("묘지에서 어둠 속성 카드 획득");
            }
            else
            {
                message.ShowSystemMessage("묘지에 카드가 없습니다.");
            }

        }
        else
        {
            TurnStartSearchSwitch();
        }
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
            StartCoroutine(gameDirector.EndGame());
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

            BackgroundManager background = FindObjectOfType<BackgroundManager>();
            background.PlayRewardSound();

            GameObject battleRewardManager = GameObject.Find("BattleRewardManager");
            battleRewardManager.GetComponent<BattleRewardManager>().rewardCrystal = monsters.Count;
            battleRewardManager.GetComponent<BattleRewardManager>().rewardEXP = monsters.Count;
            battleRewardManager.GetComponent<BattleRewardManager>().RestartBattleReward();

            this.enabled = false;
        }
    }
    
    public void TurnStartSearchSwitch()
    {
        if (isSearchEnd == "Light")
        {
            StartCoroutine(CardSearchPhaseDark());
        }
        else if(isSearchEnd == "Dark")
        {
            isSearchEnd = "End";
        }
    }
}
