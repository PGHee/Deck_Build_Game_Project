using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEffects
{
    private PlayerState player;

    public PassiveEffects(PlayerState player)
    {
        this.player = player;
    }

    public void ApplyAttributePassiveEffect(PlayerState.AttributeType attribute, int level)
    {
        switch (attribute)
        {
            case PlayerState.AttributeType.Fire:
                ApplyFirePassive(level);
                break;
            case PlayerState.AttributeType.Wind:
                ApplyWindPassive(level);
                break;
            case PlayerState.AttributeType.Wood:
                ApplyWoodPassive(level);
                break;
            case PlayerState.AttributeType.Water:
                ApplyWaterPassive(level);
                break;
            case PlayerState.AttributeType.Earth:
                ApplyEarthPassive(level);
                break;
            case PlayerState.AttributeType.Lightning:
                ApplyLightningPassive(level);
                break;
            // 다른 속성 패시브도 여기에 추가
        }
    }

    private void ApplyFirePassive(int level)
    {
        if (level >= 6)
        {
            // 불 속성 데미지 +50% 적용 로직
            player.fireDamageMultiplier = 1.5f;
        }
        else if (level >= 3)
        {
            // 불 속성 데미지 +25% 적용 로직
            player.fireDamageMultiplier = 1.25f;
        }
    }

    private void ApplyWindPassive(int level)
    {
        if (level >= 6)
        {
            // 바람 속성 타격 횟수 +2 적용 로직
            player.windHitBonus = 2;
        }
        else if (level >= 3)
        {
            // 바람 속성 타격 횟수 +1 적용 로직
            player.windHitBonus = 1;
        }
    }

    private void ApplyWoodPassive(int level)
    {
        if (level >= 6)
        {
            // 나무 속성의 독 +3 적용 로직
            player.woodPoisonBonus = 3;
        }
        else if (level >= 3)
        {
            // 나무 속성의 독 +1 적용 로직
            player.woodPoisonBonus = 1;
        }
    }

    private void ApplyLightningPassive(int level)
    {
        if (level >= 6)
        {
            // 번개 속성 스턴 확률 30% 적용 로직
            player.lightningStunChance = 0.30f;
        }
        else if (level >= 3)
        {
            // 번개 속성 스턴 확률 15% 적용 로직
            player.lightningStunChance = 0.15f;
        }
    }

    private void ApplyWaterPassive(int level)
    {
        if (level >= 6)
        {
            player.waterRegen = 14;
        }
        else if (level >= 3)
        {
            player.waterRegen = 4;
        }
    }

    private void ApplyEarthPassive(int level)
    {
        if (level >= 6)
        {
            player.earthDefense = 14;
        }
        else if (level >= 3)
        {
            player.earthDefense = 4;
        }
    }
}
