using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffManager : MonoBehaviour
{
    private Dictionary<GameObject, List<(EffectType, int, float, int)>> entityBuffs = new Dictionary<GameObject, List<(EffectType, int, float, int)>>();

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

    public void ApplyIncreaseDamageBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.IncreaseDamage, duration, effectValue, 0);
    }

    public void ApplySkipTurnBuff(GameObject entity, int duration)
    {
        ApplyBuff(entity, EffectType.SkipTurn, duration, 0f, 0);
    }

    public void ApplyRandomActionBuff(GameObject entity, int duration)
    {
        ApplyBuff(entity, EffectType.RandomAction, duration, 0f, 0);
    }

    public void ApplyAreaEffectBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.AreaEffect, duration, 0f, 0);
    }

    public void ApplyConfuseBuff(GameObject entity, int duration)
    {
        ApplyBuff(entity, EffectType.Confuse, duration, 0f, 0);
    }

    public void ApplyHealOnDamageBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.HealOnDamage, duration, effectValue, 0);
    }

    public void ApplyReflectDamageBuff(GameObject entity, int duration, float effectValue)
    {
        ApplyBuff(entity, EffectType.ReflectDamage, duration, effectValue, 0);
    }

    public void ApplyReduceCostBuff(GameObject entity, int duration, int intValue)
    {
        ApplyBuff(entity, EffectType.ReduceCost, duration, 0f, intValue);
    }
}
