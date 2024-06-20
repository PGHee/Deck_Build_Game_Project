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
                // Lightning 속성 패시브 효과도 함수에서 직접 처리 (별도 구현 필요 x)
                break;
        }
    }

    // 단일 대상 공격
    public void DealSingleTargetDamage(GameObject target, int damage, CardAction killEffect = null, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            if(monsterState.reduceDamage > 0) damage = Mathf.RoundToInt(damage * (1-monsterState.reduceDamage));
            monsterState.TakeDamage(damage);
            Debug.Log($"{target.name} took {damage} damage. Current health: {monsterState.currentHealth}");
            if(player.LifeSteal > 0) player.Heal(Mathf.RoundToInt(damage * player.LifeSteal));
            if(attributeType == PlayerState.AttributeType.Lightning) TryApplyStun(monsterState);
            if(monsterState.currentHealth <= 0 && killEffect != null) ApplyKillEffect(killEffect);
            if(monsterState.reflectDamage > 0) ReflectDamage(player.gameObject, Mathf.RoundToInt(damage * monsterState.reflectDamage));
        }
        
        PlayerState playerState = target.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.TakeDamage(damage);
        }
    }

    // 단일 대상 다중 공격
    public void DealMultipleHits(GameObject target, int damage, int hits, CardAction killEffect = null, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        for (int i = 0; i < hits; i++)
        {
            DealSingleTargetDamage(target, damage, killEffect, attributeType);
        }
    }

    // 범위 공격
    public void DealAreaDamage(List<GameObject> targets, int damage, int hits, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        foreach (var target in targets)
        {
            for (int i = 0; i < hits; i++)
            {
                DealSingleTargetDamage(target, damage, null, attributeType);
            }
        }
    }

    // 처치 시 추가 행동
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
    
    // 랜덤 대상 공격
    public void DealRandomTargetDamage(List<GameObject> enemies, int damage, int hits, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        if (enemies.Count == 0) return;
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        for (int i = 0; i < hits; i++)
        {
            int randomIndex = Random.Range(0, enemies.Count);
            DealSingleTargetDamage(enemies[randomIndex], damage, null, attributeType);
        }
    }
    
    // 랜덤 대상 공격 + 한 대상을 여러 번 타격 시 추가 공격
    public void DealRandomTargetDamageWithBonus(List<GameObject> enemies, int damage, int hits, int bonusHitFrequency, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        if (enemies.Count == 0) return;
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
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

    // 타격할수록 데미지가 늘어나는 단일 대상 공격
    public void DealIncreasingDamage(GameObject target, int baseDamage, int hits, PlayerState.AttributeType attributeType = PlayerState.AttributeType.Fire)
    {
        baseDamage = Mathf.RoundToInt(baseDamage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref baseDamage, ref hits);
        for (int i = 0; i < hits; i++)
        {
            DealSingleTargetDamage(target, baseDamage + i, null, attributeType);
        }
    }

    // 반사 데미지
    public void ReflectDamage(GameObject target, int damage)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        PlayerState playerState = target.GetComponent<PlayerState>();
        if (monsterState != null) monsterState.TakeDamage(damage);
        else if (playerState != null) playerState.TakeDamage(damage);
    }

    // 자원 회복
    public void RestoreResource(PlayerState player, int amount)
    {
        player.RestoreResource(amount);
        Debug.Log($"Player restored {amount} resource. Current resource: {player.currentResource}");
    }

    // 독 적용
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

    // 스턴 적용
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
