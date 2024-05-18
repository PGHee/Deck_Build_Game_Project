using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public int hp;
    public int attackPower;
    
    // 데미지 받기
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    // 몬스터 죽음 처리
    private void Die()
    {
        // 몬스터 사망 처리 로직
        Destroy(gameObject);
    }
}

