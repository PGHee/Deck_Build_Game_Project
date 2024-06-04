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

    // 패시브 효과 관련 변수
    public float fireDamageMultiplier = 1.0f;
    public int windHitBonus = 0;
    public int woodPoisonBonus = 0;
    public float lightningStunChance = 0.0f;

    public enum AttributeType { Fire, Water, Wood, Metal, Earth, Lightning, Wind, Light, Dark, Void }
    public Dictionary<AttributeType, int> attributeMastery;
    public Dictionary<AttributeType, int> attributeExperience;

    private PassiveEffects passiveEffects;

    void Start()
    {
        currentHealth = maxHealth;
        resource = level;
        currentResource = resource;
        InitializeAttributes();

        passiveEffects = new PassiveEffects(this);
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
        passiveEffects.ApplyAttributePassiveEffect(attribute, attributeMastery[attribute]);
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

    // 상태이상 및 기타 게임 로직 메서드 추가 필요
}
