using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffManager : MonoBehaviour
{
    public Dictionary<GameObject, List<(EffectType, int, float, int)>> entityBuffs = new Dictionary<GameObject, List<(EffectType, int, float, int)>>();
    public Dictionary<GameObject, List<(EffectType, int, float, int)>> entityDebuffs = new Dictionary<GameObject, List<(EffectType, int, float, int)>>();
    public PlayerState.AttributeType? currentField = null;     // 현재 필드 속성을 저장
    private PlayerState player;

    void Start()
    {
        player = FindObjectOfType<PlayerState>();
    }

    public void ApplyBuff(GameObject entity, EffectType effectType, int duration, float effectValue, int intValue)
    {
        if (!entityBuffs.ContainsKey(entity))
        {
            entityBuffs[entity] = new List<(EffectType, int, float, int)>();
        }
        entityBuffs[entity].Add((effectType, duration, effectValue, intValue));

        // 버프 효과를 즉시 적용
        PlayerState playerState = entity.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.ApplyBuffDebuff(effectType, duration, effectValue, intValue);
            return;
        }

        MonsterState monsterState = entity.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            monsterState.ApplyBuffDebuff(effectType, duration, effectValue, intValue);
        }
    }

    public void ApplyDebuff(GameObject entity, EffectType effectType, int duration, float effectValue, int intValue)
    {
        if (!entityDebuffs.ContainsKey(entity))
        {
            entityDebuffs[entity] = new List<(EffectType, int, float, int)>();
        }
        entityDebuffs[entity].Add((effectType, duration, effectValue, intValue));

        // 디버프 효과를 즉시 적용
        PlayerState playerState = entity.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.ApplyBuffDebuff(effectType, duration, effectValue, intValue);
            return;
        }

        MonsterState monsterState = entity.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            monsterState.ApplyBuffDebuff(effectType, duration, effectValue, intValue);
        }
    }

    private void RemoveBuff(GameObject entity, EffectType effectType, float effectValue, int intValue)
    {
        PlayerState playerState = entity.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.RemoveBuffDebuff(effectType, 0, effectValue, intValue);
            return;
        }

        MonsterState monsterState = entity.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            monsterState.RemoveBuffDebuff(effectType, 0, effectValue, intValue);
        }
    }

    private void RemoveDebuff(GameObject entity, EffectType effectType, float effectValue, int intValue)
    {
        PlayerState playerState = entity.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.RemoveBuffDebuff(effectType, 0, effectValue, intValue);
            return;
        }

        MonsterState monsterState = entity.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            monsterState.RemoveBuffDebuff(effectType, 0, effectValue, intValue);
        }
    }

    public void UpdateBuffs()
    {
        foreach (var entity in entityBuffs.Keys)
        {
            for (int i = entityBuffs[entity].Count - 1; i >= 0; i--)
            {
                var buff = entityBuffs[entity][i];
                buff.Item2--; // duration 감소
                if (buff.Item2 <= 0)
                {
                    RemoveBuff(entity, buff.Item1, buff.Item3, buff.Item4);     // 버프가 만료되면 효과 제거
                    entityBuffs[entity].RemoveAt(i);
                }
                else
                {
                    entityBuffs[entity][i] = buff;
                }
            }
        }
    }

    public void UpdateDebuffs()
    {
        foreach (var entity in entityDebuffs.Keys)
        {
            for (int i = entityDebuffs[entity].Count - 1; i >= 0; i--)
            {
                var debuff = entityDebuffs[entity][i];
                debuff.Item2--; // duration 감소
                if (debuff.Item2 <= 0)
                {
                    RemoveDebuff(entity, debuff.Item1, debuff.Item3, debuff.Item4);     // 디버프가 만료되면 효과 제거
                    entityDebuffs[entity].RemoveAt(i);
                }
                else
                {
                    entityDebuffs[entity][i] = debuff;
                }
            }
        }
    }


    // 버프
    public void ApplyIncreaseDamageBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.IncreaseDamage, duration, effectValue, 0);
    }

    public void ApplyAreaEffectBuff(GameObject entity, int duration)
    {
        ApplyBuff(entity, EffectType.AreaEffect, duration, 0f, 0);
    }

    public void ApplyLifeStealBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.LifeSteal, duration, effectValue, 0);
    }

    public void ApplyReduceDamageBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.ReduceDamage, duration, effectValue, 0);
    }

    public void ApplyReflectDamageBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.ReflectDamage, duration, effectValue, 0);
    }

    public void ApplyReduceCostBuff(GameObject entity, int duration, int intValue)
    {
        ApplyBuff(entity, EffectType.ReduceCost, duration, 0f, intValue);
    }

    public void ApplyPurification(GameObject entity)
    {
        if (entityDebuffs.ContainsKey(entity))
        {
            var debuffs = entityDebuffs[entity];
            
            // 모든 디버프를 제거
            foreach (var debuff in debuffs)
            {
                RemoveDebuff(entity, debuff.Item1, debuff.Item3, debuff.Item4);
            }

            // 해당 엔티티의 디버프 리스트 초기화
            entityDebuffs[entity].Clear();
        }
    }

    // 디버프
    public void ApplyDecreaseDamageDebuff(GameObject entity, int duration, float effectValue)
    {
        ApplyDebuff(entity, EffectType.DecreaseDamage, duration, effectValue, 0);
    }
    public void ApplySkipTurnDebuff(GameObject entity, int duration)
    {
        ApplyDebuff(entity, EffectType.SkipTurn, duration, 0f, 0);
    }

    public void ApplyRandomActionDebuff(GameObject entity)
    {
        MonsterState monsterState = entity.GetComponent<MonsterState>();
        monsterState.GetRandomAction();
    }

    public void ApplyConfuseDebuff(GameObject entity, int duration)
    {
        ApplyDebuff(entity, EffectType.Confuse, duration, 0f, 0);
    }

    // 필드 적용
    public void ApplyField(PlayerState.AttributeType? attributeType = null)
    {
        if(currentField != null)
        {
            RemoveCurrentField();
        }

        currentField = attributeType;
        Debug.Log($"Applied field: {attributeType}");
        
        switch(attributeType)
        {
            case PlayerState.AttributeType.Fire:
                player.fireDamageMultiplier += 0.25f;
                break;
            case PlayerState.AttributeType.Water:
                player.waterRegen += 8;
                break;
            case PlayerState.AttributeType.Metal:       // 골렘
                break;
            case PlayerState.AttributeType.Earth:
                player.reduceDamage += 0.25f;
                break;
            case PlayerState.AttributeType.Wood:        // 구현 완
            case PlayerState.AttributeType.Lightning:   // 구현 완
            case PlayerState.AttributeType.Wind:        // 구현 완
            case PlayerState.AttributeType.Light:       // 구현 완
            case PlayerState.AttributeType.Dark:        // 구현 완
                break;
            case PlayerState.AttributeType.Void:        // 매 턴 3장 추가 드로우
                break;
        }
    }

    private void RemoveCurrentField()
    {
        switch(currentField)
        {
            case PlayerState.AttributeType.Fire:
                player.fireDamageMultiplier -= 0.25f;
                break;
            case PlayerState.AttributeType.Water:
                player.waterRegen -= 8;
                break;
            case PlayerState.AttributeType.Metal:
                break;
            case PlayerState.AttributeType.Earth:
                player.reduceDamage -= 0.25f;
                break;
            case PlayerState.AttributeType.Wood:
            case PlayerState.AttributeType.Lightning:
            case PlayerState.AttributeType.Wind:
            case PlayerState.AttributeType.Light:
            case PlayerState.AttributeType.Dark:
                break;
            case PlayerState.AttributeType.Void:
                break;
        }
        currentField = null;
    }
}
