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

    public void ApplyStun()                             // 몬스터가 스턴에 빠질 때 동작
    {
        isStunned = true;
    }

    void Update()                                       // 실시간 업데이트. 현재는 독이 쌓이면 바로 까이게 되어있음. 턴 개념 정립 후 수정 필요.
    {
        if (poisonStacks > 0)
        {
            TakeDamage(poisonStacks);
            poisonStacks--;
        }
    }

    void Die()                                          // 몬스터의 체력이 0 밑으로 떨어지면 사망 처리 됨.
    {
        Debug.Log($"{gameObject.name} died");
        Destroy(gameObject);
    }
}
