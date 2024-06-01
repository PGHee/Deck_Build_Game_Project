using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public int maxHealth = 100;             // 플레이어의 최대 체력
    public int currentHealth;               // 플레이어의 현재 체력

    public int level = 3;                   // 플레이어의 현재 레벨
    public int maxLevel = 10;               // 플레이어의 최대 레벨
    public int experience = 0;              // 플레이어의 경험치

    public int resource;                    // 플레이어의 최대 자원, 플레이어의 등급과 동일
    public int currentResource;             // 플레이어의 현재 남은 자원

    public int shield = 0;                  // 플레이어의 방어

    public enum AttributeType { Fire, Water, Wood, Metal, Earth, Lightning, Wind, Light, Dark, Void }   // 숙련도용. 추후 수정 예정
    public Dictionary<AttributeType, int> attributeMastery;
    public Dictionary<AttributeType, int> attributeExperience;

    void Start()
    {
        currentHealth = maxHealth;
        resource = level;
        currentResource = resource;

        // 속성 숙련도와 경험치 초기화
        attributeMastery = new Dictionary<AttributeType, int>();
        attributeExperience = new Dictionary<AttributeType, int>();
        foreach (AttributeType attr in System.Enum.GetValues(typeof(AttributeType)))
        {
            attributeMastery[attr] = 1; // 초기 숙련도는 1
            attributeExperience[attr] = 0; // 초기 경험치는 0
        }
    }

    // 경험치를 추가하고 레벨 업을 처리하는 메서드
    public void AddExperience(int exp)
    {
        experience += exp;
        while (experience >= ExperienceToNextLevel() && level < maxLevel)
        {
            experience -= ExperienceToNextLevel();
            LevelUp();
        }
    }

    // 다음 레벨까지 필요한 경험치 계산 메서드
    private int ExperienceToNextLevel()
    {
        return level * 100; // 레벨업에 필요한 경험치 계산 (예: 현재 레벨 * 100)
    }

    // 레벨 업 처리 메서드
    private void LevelUp()
    {
        level++;
        resource = level; // 자원을 새로운 등급으로 설정
        currentResource = resource; // 자원 회복
    }

    // 속성 경험치를 추가하고 숙련도 업을 처리하는 메서드
    public void AddAttributeExperience(AttributeType attribute, int exp)
    {
        attributeExperience[attribute] += exp;
        while (attributeExperience[attribute] >= ExperienceToNextAttributeLevel(attribute) && attributeMastery[attribute] < 10)
        {
            attributeExperience[attribute] -= ExperienceToNextAttributeLevel(attribute);
            AttributeLevelUp(attribute);
        }
    }

    // 다음 속성 레벨까지 필요한 경험치 계산 메서드
    private int ExperienceToNextAttributeLevel(AttributeType attribute)
    {
        return attributeMastery[attribute] * 50; // 숙련도 업에 필요한 경험치 계산 (예: 현재 숙련도 * 50)
    }

    // 속성 숙련도 업 처리 메서드
    private void AttributeLevelUp(AttributeType attribute)
    {
        attributeMastery[attribute]++;
    }

    // 체력 회복 메서드
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player healed by {amount}. Current health: {currentHealth}");
    }

    // 자원 회복 메서드
    public void RestoreResource(int amount)
    {
        currentResource = Mathf.Min(currentResource + amount, resource);
        Debug.Log($"Player restored {amount} resource. Current resource: {currentResource}");
    }

    // 방어 적용 메서드
    public void ApplyShield(int amount)
    {
        shield += amount;
        Debug.Log($"Player gained {amount} shield. Current shield: {shield}");
    }

    // 자원 소비 메서드
    public void SpendResource(int amount)
    {
        if (currentResource >= amount)
        {
            currentResource -= amount;
            Debug.Log($"Player spent {amount} resource. Current resource: {currentResource}");
        }
        else
        {
            Debug.Log("Not enough resource.");
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
