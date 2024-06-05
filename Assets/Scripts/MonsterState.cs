using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterState : MonoBehaviour
{
    public int maxHealth;                               // 몬스터의 최대 체력
    public int currentHealth;                           // 몬스터의 현재 체력
    public int attackPower;                             // 몬스터의 데미지 (비용값)

    public bool isStunned;                              // 몬스터의 스턴 상태 판별
    public int poisonStacks;                            // 몬스터의 중독 상태 판별

    public bool IsStunned => isStunned;                 // IsStunned 프로퍼티 추가
    public bool IsPoisoned => poisonStacks > 0;         // IsPoisoned 프로퍼티 추가

    void Start()
    {
        TurnManager.instance.RegisterMonster(this);
        currentHealth = maxHealth;
        isStunned = false;
        poisonStacks = 0;
    }

    public void TakeDamage(int damage)                  // 몬스터가 데미지를 받을 때 동작
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage, remaining health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyPoison(int amount)                 // 몬스터가 독을 받을 때 동작
    {
        poisonStacks += amount;
    }

    public void ApplyPoisonDamage()                     // 독 데미지 적용 메서드
    {
        if (IsPoisoned)
        {
            TakeDamage(poisonStacks);
            poisonStacks--;
        }
    }

    public void ApplyStun()                             // 몬스터가 스턴에 빠질 때 동작
    {
        isStunned = true;
    }

    public void HandleStun()                            // 스턴 처리 메서드
    {
        if (IsStunned)
        {
            Debug.Log($"{gameObject.name} is stunned and skips its turn.");
            isStunned = false;  // 스턴 상태 해제
        }
    }

    void Die()                                          // 몬스터의 체력이 0 밑으로 떨어지면 사망 처리 됨.
    {
        Debug.Log($"{gameObject.name} died");
        Destroy(gameObject);
    }
}
