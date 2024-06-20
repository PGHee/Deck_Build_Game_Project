using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public BuffDebuffManager buffDebuffManager;
    private PlayerState player;
    private List<MonsterState> monsters;
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
        player = FindObjectOfType<PlayerState>();
        monsters = new List<MonsterState>(FindObjectsOfType<MonsterState>());
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
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
        player.RestoreResource(player.resource);
        player.ApplyTurnBasedPassives();        // 패시브 효과 적용

        // 플레이어에게 존재하는 버프와 디버프 효과 적용 및 소모
        buffDebuffManager.UpdateBuffs();        // 버프 업데이트
        buffDebuffManager.UpdateDebuffs();      // 디버프 업데이트

        Debug.Log("Player's turn started.");    // 플레이어가 행동을 완료하면 턴 종료 버튼으로 EndPlayerTurn 호출
    }

    public void EndPlayerTurn()
    {
        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            StartCoroutine(MonsterTurn());
        }
    }

    IEnumerator MonsterTurn()
    {
        Debug.Log("Monster's turn started.");
        // 몬스터에게 존재하는 버프와 디버프 효과 적용 및 소모 (추후 추가)
        foreach (var monster in monsters)
        {
            if (monster != null && monster.gameObject.activeInHierarchy)
            {
                monster.ApplyPoisonDamage();            // 독 데미지와 스택 감소 적용
                if (buffDebuffManager.entityDebuffs[monster.gameObject].Any(debuff => debuff.Item1 == EffectType.SkipTurn)) continue;
                else if (monster.IsStunned)
                {
                    monster.HandleStun();               // 스턴 상태 처리
                    continue;                           // 스턴 상태라면 행동을 스킵
                }
                // 몬스터의 행동 (추후 추가)
                Debug.Log($"{monster.name} is taking action.");
                yield return new WaitForSeconds(3);     // 각 몬스터의 행동 사이에 딜레이 추가
            }
        }
        StartPlayerTurn();
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
            Debug.Log("All monsters are defeated. Battle ended.");
            // 전투 종료 처리 (승리)
            this.enabled = false;
        }
    }
}
