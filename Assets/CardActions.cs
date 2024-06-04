using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardActions : MonoBehaviour
{
    private PlayerState player;

    void Start()
    {
        player = FindObjectOfType<PlayerState>();
    }

    void ApplyPassiveEffects(PlayerState.AttributeType attributeType, ref int damage, ref int hits)
    {
        if (player == null) return;

        switch (attributeType)
        {
            case PlayerState.AttributeType.Fire:
                damage = Mathf.RoundToInt(damage * player.fireDamageMultiplier);
                Debug.Log("Fire attribute passive effect applied: Increased damage.");
                break;
            case PlayerState.AttributeType.Wind:
                hits += player.windHitBonus;
                Debug.Log("Wind attribute passive effect applied: Increased hits.");
                break;
            case PlayerState.AttributeType.Wood:
                // Wood 속성의 패시브 효과는 ApplyPoison 함수에서 직접 처리 (별도 구현 필요 X)
                break;
            case PlayerState.AttributeType.Lightning:
                // Lightning 속성의 스턴 관련 패시브 효과 적용
                // 예시로, 일정 확률로 스턴을 적용하는 패시브 효과
                break;
        }
    }

    public void DealSingleTargetDamage(GameObject target, int damage, CardAction killEffect = null, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            monsterState.TakeDamage(damage);
            Debug.Log($"{target.name} took {damage} damage. Current health: {monsterState.currentHealth}");

            if (attributeType == PlayerState.AttributeType.Lightning)
            {
                TryApplyStun(monsterState);
            }

            if (monsterState.currentHealth <= 0 && killEffect != null)
            {
                ApplyKillEffect(killEffect);
            }
        }
    }

    void ApplyKillEffect(CardAction killEffect)
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
    
    public void DealRandomTargetDamage(List<GameObject> enemies, int damage, int hits, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        if (enemies.Count == 0) return;
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        for (int i = 0; i < hits; i++)
        {
            int randomIndex = Random.Range(0, enemies.Count);
            DealSingleTargetDamage(enemies[randomIndex], damage, null, attributeType);
        }
    }
    
    public void DealRandomTargetDamageWithBonus(List<GameObject> enemies, int damage, int hits, int bonusHitFrequency, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        if (enemies.Count == 0) return;
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        Dictionary<GameObject, int> hitCounts = new Dictionary<GameObject, int>();
        for (int i = 0; i < hits; i++)
        {
            int randomIndex = Random.Range(0, enemies.Count);
            GameObject target = enemies[randomIndex];
            DealSingleTargetDamage(target, damage, null, attributeType);

            if (hitCounts.ContainsKey(target))
            {
                hitCounts[target]++;
                if (hitCounts[target] % bonusHitFrequency == 0)
                {
                    DealSingleTargetDamage(target, damage, null, attributeType);
                }
            }
            else
            {
                hitCounts[target] = 1;
            }
        }
    }

    public void DealAreaDamage(List<GameObject> targets, int damage, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        int hits = 1;
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        foreach (var target in targets)
        {
            for (int i = 0; i < hits; i++)
            {
                DealSingleTargetDamage(target, damage, null, attributeType);
            }
        }
    }

    public void DealMultipleTargetDamage(GameObject target, int damage, int hits, CardAction killEffect = null, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        for (int i = 0; i < hits; i++)
        {
            DealSingleTargetDamage(target, damage, killEffect, attributeType);
        }
    }

    public void DealIncreasingDamage(GameObject target, int baseDamage, int hits, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        ApplyPassiveEffects(attributeType, ref baseDamage, ref hits);
        for (int i = 0; i < hits; i++)
        {
            DealSingleTargetDamage(target, baseDamage + i, null, attributeType);
        }
    }

    public void RestoreResource(PlayerState player, int amount)
    {
        player.RestoreResource(amount);
        Debug.Log($"Player restored {amount} resource. Current resource: {player.currentResource}");
    }

    public void ApplyPoison(GameObject target, int poisonAmount, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Wood)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            if (attributeType == PlayerState.AttributeType.Wood)
            {
                poisonAmount += player.woodPoisonBonus;
                Debug.Log("Wood attribute passive effect applied: Increased poison amount.");
            }
            monsterState.ApplyPoison(poisonAmount);
            Debug.Log($"{target.name} poisoned with {poisonAmount} amount");
        }
    }

    void TryApplyStun(MonsterState monsterState)
    {
        if (Random.value < player.lightningStunChance)
        {
            monsterState.ApplyStun();
            Debug.Log($"{monsterState.name} is stunned.");
        }
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
