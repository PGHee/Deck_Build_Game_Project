using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int level = 3;
    public int maxLevel = 10;
    public int experience = 0;
    public int resource;
    public int currentResource;
    public int shield = 0;
    public float damageMultiplier = 1.0f;

    // 패시브 효과 관련 변수
    public float fireDamageMultiplier = 1.0f;
    public int windHitBonus = 0;
    public int woodPoisonBonus = 0;
    public float lightningStunChance = 0.0f;
    public int waterRegen = 0;
    public int earthDefense = 0;

    public enum AttributeType { Fire, Water, Wood, Metal, Earth, Lightning, Wind, Light, Dark, Void }
    public Dictionary<AttributeType, int> attributeMastery;
    public Dictionary<AttributeType, int> attributeExperience;

    private Animator animator;                      // 애니메이션 동작용
    private Transform playerTransform;              // 애니메이션 크기 맞추는 용

    public List<BuffDebuff> activeBuffsAndDebuffs = new List<BuffDebuff>();     // 현재 적용되고 있는 버프와 디버프
    public bool isAreaEffect = false;               // 광역 공격 버프 상태
    public bool healOnDamage = false;               // HealOnDamage 버프 상태
    public int stunDuration = 0;                    // 스턴 지속 시간 (디버프용)
    public bool isStunned = false;                  // 플레이어가 스턴 상태인지 여부

    void Start()
    {
        currentHealth = maxHealth;
        resource = level;
        currentResource = resource;
        InitializeAttributes();

        animator = GetComponent<Animator>();            // 애니메이션 동작용
        playerTransform = GetComponent<Transform>();    // 애니메이션 크기 맞추는 용
    }

    void InitializeAttributes()
    {
        attributeMastery = new Dictionary<AttributeType, int>();
        attributeExperience = new Dictionary<AttributeType, int>();
        foreach (AttributeType attr in System.Enum.GetValues(typeof(AttributeType)))
        {
            attributeMastery[attr] = 1;
            attributeExperience[attr] = 0;
        }
    }

    public void AddExperience(int exp)
    {
        experience += exp;
        while (experience >= ExperienceToNextLevel() && level < maxLevel)
        {
            experience -= ExperienceToNextLevel();
            LevelUp();
        }
    }

    int ExperienceToNextLevel() => level * 100;

    void LevelUp()
    {
        level++;
        resource = level;
        currentResource = resource;
        Debug.Log($"Level Up! New Level: {level}, Resource: {resource}");
    }

    public void AddAttributeExperience(AttributeType attribute, int exp)
    {
        attributeExperience[attribute] += exp;
        while (attributeExperience[attribute] >= ExperienceToNextAttributeLevel(attribute) && attributeMastery[attribute] < 10)
        {
            attributeExperience[attribute] -= ExperienceToNextAttributeLevel(attribute);
            AttributeLevelUp(attribute);
        }
    }

    int ExperienceToNextAttributeLevel(AttributeType attribute)
    {
        int[] experienceRequired = { 3, 6, 9, 15, 25, 35, 55, 75, 95 };
        int masteryLevel = attributeMastery[attribute] - 1;
        return masteryLevel < experienceRequired.Length ? experienceRequired[masteryLevel] : int.MaxValue;
    }

    void AttributeLevelUp(AttributeType attribute)
    {
        attributeMastery[attribute]++;
        ApplyAttributePassiveEffect(attribute, attributeMastery[attribute]);
        Debug.Log($"Attribute Level Up! {attribute} Mastery: {attributeMastery[attribute]}");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player healed by {amount}. Current health: {currentHealth}");
    }

    public void RestoreResource(int amount)
    {
        currentResource = Mathf.Min(currentResource + amount, resource);
        Debug.Log($"Player restored {amount} resource. Current resource: {currentResource}");
    }

    public void ApplyShield(int amount)
    {
        shield += amount;
        Debug.Log($"Player shield increased by {amount}. Current shield: {shield}");
    }

    public void SpendResource(int amount)
    {
        if (currentResource >= amount)
        {
            currentResource -= amount;
            Debug.Log($"Resource spent: {amount}. Current resource: {currentResource}");
        }
        else
        {
            Debug.Log("Not enough resources.");
        }
    }

    // 데미지 처리 메서드
    public void TakeDamage(int damage)
    {
        animator.SetTrigger("HitTrigger");
        if (shield >= damage)
        {
            shield -= damage;
        }
        else
        {
            int remainingDamage = damage - shield;
            shield = 0;
            currentHealth -= remainingDamage;
            if (currentHealth <= 0)
            {
                // 플레이어가 사망했을 때의 처리
                Debug.Log("Player is dead.");
            }
        }
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}, Current shield: {shield}");
    }

    // 매 턴마다 적용되는 패시브 효과
    public void ApplyTurnBasedPassives()
    {
        if (waterRegen > 0)
        {
            Heal(waterRegen);
        }

        if (earthDefense > 0)
        {
            ApplyShield(earthDefense);
        }
    }

    // 버프와 디버프를 적용하는 메서드 추가
    public void ApplyBuffDebuff(EffectType effectType, int duration, float effectValue, int intValue)
    {
        switch (effectType)
        {
            case EffectType.IncreaseDamage:
                damageMultiplier += effectValue;
                break;
            case EffectType.AreaEffect:
                isAreaEffect = true;
                break;
            case EffectType.SkipTurn:
                isStunned = stunDuration > 0;
                break;
            // 다른 버프/디버프 처리 로직 추가
        }
    }

    public void RemoveBuffDebuff(EffectType effectType, int duration, float effectValue, int intValue)
    {
        switch (effectType)
        {
            case EffectType.IncreaseDamage:
                damageMultiplier -= effectValue;
                break;
            case EffectType.AreaEffect:
                isAreaEffect = false;
                break;
            case EffectType.SkipTurn:
                isStunned = false;
                break;
            // 다른 버프/디버프 처리 로직 추가
        }
    }

    public void ApplyStun()                             // 스턴 적용 메서드
    {
        isStunned = true;
    }

    public void ApplyAttributePassiveEffect(AttributeType attribute, int level)
    {
        switch (attribute)
        {
            case AttributeType.Fire:
                fireDamageMultiplier = (level >= 6) ? 1.5f : (level >= 3) ? 1.25f : fireDamageMultiplier;
                break;
            case AttributeType.Wind:
                windHitBonus = (level >= 6) ? 2 : (level >= 3) ? 1 : windHitBonus;
                break;
            case AttributeType.Wood:
                woodPoisonBonus = (level >= 6) ? 3 : (level >= 3) ? 1 : woodPoisonBonus;
                break;
            case AttributeType.Water:
                waterRegen = (level >= 6) ? 14 : (level >= 3) ? 4 : waterRegen;
                break;
            case AttributeType.Earth:
                earthDefense = (level >= 6) ? 14 : (level >= 3) ? 4 : earthDefense;
                break;
            case AttributeType.Lightning:
                lightningStunChance = (level >= 6) ? 0.30f : (level >= 3) ? 0.15f : lightningStunChance;
                break;
        }
    }

    public void AttackMotion()
    {
        animator.SetTrigger("AttackTrigger");
    }
    public void AdjustScale(float newScale)             // 애니메이션 동작 시 스케일 조절용
    {
        playerTransform.localScale = new Vector2(newScale, newScale);
    }
    // 상태이상 및 기타 게임 로직 메서드 추가 필요
}
