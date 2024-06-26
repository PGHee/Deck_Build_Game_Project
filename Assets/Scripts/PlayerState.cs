using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // 플레이어 기본 정보
    public int maxHealth = 100;
    public int currentHealth;
    public int level = 3;
    public int maxLevel = 10;
    public int experience = 0;
    public int resource;
    public int currentResource;
    public int shield = 0;

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

    // 버프 관련 변수
    public float damageMultiplier = 1.0f;           // 데미지 배율
    public bool isAreaEffect = false;               // 광역 공격 버프 상태
    public float LifeSteal = 0.0f;                  // 흡혈 버프 상태
    public float reduceDamage = 0.0f;               // 데미지 감소 버프 상태
    public float reflectDamage = 0.0f;              // 반사 버프 상태

    // 디버프 관련 변수
    public int poisonStacks = 0;                    // 독 스택
    public bool isStunned = false;                  // 플레이어가 스턴 상태인지 여부
    public bool isConfuse = false;                  // 플레이어가 혼란 상태인지 여부

    private Animator animator;                      // 애니메이션 동작용
    private Transform playerTransform;              // 애니메이션 크기 맞추는 용

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
            case EffectType.LifeSteal:
                LifeSteal += effectValue;
                break;
            case EffectType.ReduceDamage:
                reduceDamage += effectValue;
                break;
            case EffectType.ReflectDamage:
                reflectDamage += effectValue;
                break;
            case EffectType.ReduceCost:         // 좀 더 고민해야 됨.
                break;
            case EffectType.SkipTurn:
                isStunned = true;
                break;
            case EffectType.Confuse:
                isConfuse = true;
                break;
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
            case EffectType.LifeSteal:
                LifeSteal -= effectValue;
                break;
            case EffectType.ReduceDamage:
                reduceDamage -= effectValue;
                break;
            case EffectType.ReflectDamage:
                reflectDamage -= effectValue;
                break;
            case EffectType.ReduceCost:         // 좀 더 고민해야 됨.
                break;
            case EffectType.SkipTurn:
                isStunned = false;
                break;
            case EffectType.Confuse:
                isConfuse = false;
                break;
        }
    }

    public void ApplyPoison(int amount)                 // 독을 받을 때 동작
    {
        poisonStacks += amount;
    }

    public void ApplyPoisonDamage()                     // 독 데미지 적용 메서드
    {
        if (poisonStacks > 0)
        {
            TakeDamage(poisonStacks);
            poisonStacks--;
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
    // 상태이상 및 기타 게임 로직 메서드 추가 필요
}
