using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActions : MonoBehaviour
{
    public void DealSingleTargetDamage(GameObject target, int damage, CardAction killEffect = null) // 단일 대상에게 데미지를 입히는 행동을 총괄
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            monsterState.TakeDamage(damage);
            Debug.Log($"{target.name} took {damage} damage. Current health: {monsterState.currentHealth}");

            if (monsterState.currentHealth <= 0 && killEffect != null)
            {
                ApplyKillEffect(killEffect);
            }
        }
    }

    void ApplyKillEffect(CardAction killEffect) // 공격 대상으로 지정된 타겟이 처치되면 발동될 추가 행동을 관리함
    {
        switch (killEffect.killEffectType)
        {
            case CardActionType.Damage:
                DealRandomTargetDamage(FindObjectsOfType<MonsterState>().Where(m => m.currentHealth > 0).Select(m => m.gameObject).ToList(), killEffect.thirdValue, 1);
                break;
            case CardActionType.Heal:
                FindObjectOfType<PlayerState>().Heal(killEffect.thirdValue);
                break;
            case CardActionType.Shield:
                FindObjectOfType<PlayerState>().ApplyShield(killEffect.thirdValue);
                break;
            case CardActionType.RestoreResource:
                FindObjectOfType<PlayerState>().RestoreResource(killEffect.thirdValue);
                break;
        }
    }
    
    public void DealRandomTargetDamage(List<GameObject> enemies, int damage, int hits)  // 공격 대상을 랜덤으로 지정해야 하는 경우에 동작함
    {
        if (enemies.Count == 0) return;
        for (int i = 0; i < hits; i++)
        {
            int randomIndex = Random.Range(0, enemies.Count);
            DealSingleTargetDamage(enemies[randomIndex], damage);
        }
    }
    
    // 다중 타격 + 동일 대상 반복 타격 시 추가 타격 발생
    public void DealRandomTargetDamageWithBonus(List<GameObject> enemies, int damage, int hits, int bonusHitFrequency)
    {
        if (enemies.Count == 0) return;

        Dictionary<GameObject, int> hitCounts = new Dictionary<GameObject, int>();

        for (int i = 0; i < hits; i++)
        {
            int randomIndex = Random.Range(0, enemies.Count);
            GameObject target = enemies[randomIndex];
            DealSingleTargetDamage(target, damage);

            if (hitCounts.ContainsKey(target))
            {
                hitCounts[target]++;
                if (hitCounts[target] % bonusHitFrequency == 0)
                {
                    DealSingleTargetDamage(target, damage);
                }
            }
            else
            {
                hitCounts[target] = 1;
            }
        }
    }


    public void DealAreaDamage(List<GameObject> targets, int damage)    // 공격 대상이 광역으로 발동되어야 하는 경우에 동작함
    {
        foreach (var target in targets)
        {
            DealSingleTargetDamage(target, damage);
        }
    }

    public void DealMultipleTargetDamage(GameObject target, int damage, int hits)   // 다중 타격하는 경우에 동작
    {
        for (int i = 0; i < hits; i++)
        {
            DealSingleTargetDamage(target, damage);
        }
    }

    public void DealIncreasingDamage(GameObject target, int baseDamage, int hits)   // 다중 타격하되 점차 데미지가 증가하는 방식
    {
        for (int i = 0; i < hits; i++)
        {
            DealSingleTargetDamage(target, baseDamage + i);
        }
    }

    public void RestoreResource(PlayerState player, int amount)     // 카드의 능력이 자원 회복인 경우에 동작함
    {
        player.RestoreResource(amount); // amount를 인자로 전달
        Debug.Log($"Player restored {amount} resource. Current resource: {player.currentResource}");
    }

    public void ApplyPoison(GameObject target, int poisonAmount)    // 공격 대상에게 독을 부여하는 경우 동작함
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            monsterState.ApplyPoison(poisonAmount);
            Debug.Log($"{target.name} poisoned with {poisonAmount} amount");
        }
    }

    public void ApplyStun(GameObject target)        // 공격 대상에게 스턴 상태이상을 부여해야하는 경우에 동작함.
    {                                               // 추후 값을 받아 해당 값에 따라 확률적으로 걸리게끔 수정이 필요함.
        target.GetComponent<MonsterState>().ApplyStun();
    }

    public void DrawCards(PlayerState player, int count)
    {
        //카드의 효과로 인한 드로우. 추후 카드 덱 관리하는 매니저의 카드 드로우 함수를 불러올 예정
    }

    public void DiscardCards(PlayerState player, int count)
    {
        //카드의 효과로 인한 패 버리기. 추후 카드 덱 관리 매니저에 존재하는 핸드 버리기 함수를 불러올 예정
    }
}
