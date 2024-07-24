using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Actions : MonoBehaviour
{
    private PlayerState player;
    private BuffDebuffManager buffDebuff;
    private Effect effect;
    private DamageText damageText;

    void Start()
    {
        player = FindObjectOfType<PlayerState>();
        buffDebuff = FindObjectOfType<BuffDebuffManager>();
        effect = FindObjectOfType<Effect>();
        damageText = FindObjectOfType<DamageText>();
    }

    void ApplyPassiveEffects(PlayerState.AttributeType? attributeType, ref int damage, ref int hits)
    {
        if (player == null) return;

        switch (attributeType)
        {
            case PlayerState.AttributeType.Fire:
                damage = Mathf.RoundToInt(damage * player.fireDamageMultiplier);
                break;
            case PlayerState.AttributeType.Wind:
                hits += player.windHitBonus;
                break;
        }
    }

    // 단일 대상 공격
    public void DealSingleTargetDamage(GameObject target, int damage, CardAction killEffect = null, PlayerState.AttributeType? attributeType = null)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        MonsterState attackerState = MonsterState.currentAttacker;
        if(buffDebuff.currentField == PlayerState.AttributeType.Wind && attributeType == PlayerState.AttributeType.Wind) damage += 1;
        if (monsterState != null)
        {
            if(monsterState.reduceDamage > 0) damage = Mathf.RoundToInt(damage * (1-monsterState.reduceDamage));
            monsterState.TakeDamage(damage);
            if(player.LifeSteal > 0) player.Heal(Mathf.RoundToInt(damage * player.LifeSteal));
            if(attributeType == PlayerState.AttributeType.Lightning) TryApplyStun(target);
            if(monsterState.currentHealth <= 0 && killEffect != null) ApplyKillEffect(killEffect, attributeType);
            if(monsterState.reflectDamage > 0) ReflectDamage(player.gameObject, Mathf.RoundToInt(damage * monsterState.reflectDamage));
        }
        
        PlayerState playerState = target.GetComponent<PlayerState>();
        if (playerState != null)
        {
            effect.ApplyEffect(target, 1, 1, 0.1f);   // 수정 필요
            damageText.ShowDamage(target, 9, damage, 1, 0.1f);
            if(playerState.reduceDamage > 0) damage = Mathf.RoundToInt(damage * (1-playerState.reduceDamage));
            playerState.TakeDamage(damage);
            if(attackerState != null && attackerState.LifeSteal > 0) attackerState.Heal(Mathf.RoundToInt(damage * attackerState.LifeSteal));
            if(playerState.reflectDamage > 0) ReflectDamage(attackerState.gameObject, Mathf.RoundToInt(damage * playerState.reflectDamage));
        }
    }

    // 단일 대상 다중 공격
    public void DealMultipleHits(GameObject target, int damage, int hits, CardAction killEffect = null, PlayerState.AttributeType? attributeType = null)
    {
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        effect.ApplyEffect(target, (int)attributeType, hits, 0.1f);
        damageText.ShowDamage(target, (int)attributeType, damage, hits, 0.1f);
        for (int i = 0; i < hits; i++)
        {
            DealSingleTargetDamage(target, damage, killEffect, attributeType);
        }
    }

    // 범위 공격
    public void DealAreaDamage(List<GameObject> targets, int damage, int hits, PlayerState.AttributeType? attributeType = null)
    {
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        effect.ApplyAreaEffect(targets, (int)attributeType, hits, 0.1f);
        damageText.ShowAreaDamage(targets, (int)attributeType, damage, hits, 0.1f);
        foreach (var target in targets)
        {
            for (int i = 0; i < hits; i++)
            {
                DealSingleTargetDamage(target, damage, null, attributeType);
            }
        }
    }

    // 처치 시 추가 행동
    void ApplyKillEffect(CardAction killEffect, PlayerState.AttributeType? attributeType = null)
    {
        switch (killEffect.killEffectType)
        {
            case CardActionType.Damage:
                DealRandomTargetDamage(FindObjectsOfType<MonsterState>().Where(m => m.currentHealth > 0).Select(m => m.gameObject).ToList(), killEffect.thirdValue, 1, attributeType);
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
    public void DealRandomTargetDamage(List<GameObject> enemies, int damage, int hits, PlayerState.AttributeType? attributeType = null)
    {
        if (enemies.Count == 0) return;
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        for (int i = 0; i < hits; i++)
        {
            if (enemies.Count == 0) break;
            int randomIndex = Random.Range(0, enemies.Count);
            GameObject target = enemies[randomIndex];
            effect.ApplyEffect(target, (int)attributeType, 1, 0.1f * i);
            damageText.ShowDamage(target, (int)attributeType, damage, 1, 0.1f * i);
            DealSingleTargetDamage(enemies[randomIndex], damage, null, attributeType);

            if (target.GetComponent<MonsterState>().currentHealth <= 0)
            {
                enemies.RemoveAt(randomIndex);
            }
        }
    }
    
    // 랜덤 대상 공격 + 한 대상을 여러 번 타격 시 추가 공격
    public void DealRandomTargetDamageWithBonus(List<GameObject> enemies, int damage, int hits, int bonusHitFrequency, PlayerState.AttributeType? attributeType = null)
    {
        if (enemies.Count == 0) return;
        damage = Mathf.RoundToInt(damage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref damage, ref hits);
        Dictionary<GameObject, int> hitCounts = new Dictionary<GameObject, int>();
        for (int i = 0; i < hits; i++)
        {
            if (enemies.Count == 0) break;
            int randomIndex = Random.Range(0, enemies.Count);
            GameObject target = enemies[randomIndex];
            
            // 데미지 적용 및 이펙트 호출
            effect.ApplyEffect(target, (int)attributeType, 1, 0.1f * i);
            damageText.ShowDamage(target, (int)attributeType, damage, 1, 0.1f * i);
            DealSingleTargetDamage(target, damage, null, attributeType);

            if (hitCounts.ContainsKey(target))
            {
                hitCounts[target]++;
                if (hitCounts[target] % bonusHitFrequency == 0)
                {
                    effect.ApplyEffect(target, (int)attributeType, 1, 0.1f * i);
                    damageText.ShowDamage(target, (int)attributeType, damage, 1, 0.1f * i);
                    DealSingleTargetDamage(target, damage, null, attributeType);
                }
            }
            else
            {
                hitCounts[target] = 1;
            }

            // 체력이 0 이하인 경우 리스트에서 제거
            if (target.GetComponent<MonsterState>().currentHealth <= 0)
            {
                enemies.RemoveAt(randomIndex);
            }
        }
    }

    // 타격할수록 데미지가 늘어나는 단일 대상 공격
    public void DealIncreasingDamage(GameObject target, int baseDamage, int hits, PlayerState.AttributeType? attributeType = null)
    {
        baseDamage = Mathf.RoundToInt(baseDamage * player.damageMultiplier);
        ApplyPassiveEffects(attributeType, ref baseDamage, ref hits);
        effect.ApplyEffect(target, (int)attributeType, hits, 0.1f);
        for (int i = 0; i < hits; i++)
        {
            damageText.ShowDamage(target, (int)attributeType, baseDamage + i, 1, 0.1f * i);
            DealSingleTargetDamage(target, baseDamage + i, null, attributeType);
        }
    }

    // 반사 데미지
    public void ReflectDamage(GameObject target, int damage)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        PlayerState playerState = target.GetComponent<PlayerState>();
        if (monsterState != null)
        {
            effect.ApplyEffect(target, 1, 1, 0.1f); // 수정 필요
            damageText.ShowDamage(target, 9, damage, 1, 0.1f);
            monsterState.TakeDamage(damage);
        }
        else if (playerState != null)
        {
            effect.ApplyEffect(target, 1, 1 , 0.1f); // 수정 필요
            damageText.ShowDamage(target, 9, damage, 1, 0.1f);
            playerState.TakeDamage(damage);
        }
    }

    // 자원 회복
    public void RestoreResource(PlayerState player, int amount)
    {
        player.RestoreResource(amount);
    }

    // 독 적용
    public void ApplyPoison(GameObject target, int poisonAmount, int hits, PlayerState.AttributeType? attributeType = null)
    {
        MonsterState monsterState = target.GetComponent<MonsterState>();
        if(monsterState != null)
        {
            if(attributeType == PlayerState.AttributeType.Wood) poisonAmount += player.woodPoisonBonus;
            effect.ApplyEffect(target, (int)attributeType, hits, 0.1f);
            for(int i = 0; i < hits; i++) monsterState.ApplyPoison(poisonAmount);
        }

        PlayerState playerState = target.GetComponent<PlayerState>();
        if(playerState != null)
        {
            effect.ApplyEffect(target, 1, hits, 0.1f); // 수정 필요
            for(int i = 0; i < hits; i++)
            {
                playerState.ApplyPoison(poisonAmount);
            }
        }
    }

    // 스턴 적용
    void TryApplyStun(GameObject target)
    {
        if (Random.value < player.lightningStunChance) buffDebuff.ApplySkipTurnDebuff(target, 1);
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
