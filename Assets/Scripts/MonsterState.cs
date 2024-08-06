using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ActionType
{
    Damage, StrongAttack, LifeStealAttack, 
    Shield, Heal, AreaHeal, Poison, SelfDestruct, Wait,

    IncreaseDamage, IncreaseDamageStack, LifeSteal, 
    ReduceDamage, ReflectDamage, SkipTurn, Confuse
}

public enum PassiveType
{
    Revive, Bond
}

[System.Serializable]
public class Action
{
    public ActionType actionType;
    public bool isRandomTargetBuff;         // 랜덤한 아군에게 버프주는 형태
    public int duration;                    // 버프나 디버프 형태의 기간
    public float floatValue;                // 실수형 파라미터가 필요한 경우
    public int intValue;                    // 정수형 파라미터가 필요한 경우
}

[System.Serializable]
public class Passive
{
    public PassiveType passiveType;     // 해당 몬스터가 기본적으로 갖고 있는 패시브 
}

public class MonsterState : MonoBehaviour
{
    public static MonsterState currentAttacker;                         // 현재 공격 중인 몬스터를 파악하기 위해 사용
    public int maxHealth;                                               // 몬스터의 최대 체력
    public int currentHealth;                                           // 몬스터의 현재 체력
    public int attackPower;                                             // 몬스터의 데미지 (비용값)
    public int shield;                                                  // 몬스터의 방어

    public float damageMultiplier = 1.0f;                               // 몬스터 데미지 
    public float LifeSteal = 0.0f;                                      // 흡혈 버프 상태
    public float reduceDamage = 0.0f;                                   // 데미지 감소 버프 상태
    public float reflectDamage = 0.0f;                                  // 몬스터 반사 버프 상태

    public bool isStunned = false;                                      // 몬스터의 스턴 상태 판별
    public bool isConfuse = false;                                      // 몬스터의 혼란 상태 판별
    public int poisonStacks = 0;                                        // 몬스터의 중독 상태 판별

    public List<Action> actions = new List<Action>();                   // 몬스터의 행동들
    public List<Passive> passives = new List<Passive>();                // 몬스터 패시브

    private BuffDebuffManager buffDebuffManager;
    private Actions monsterActions;
    public Action selectedAction;                                       // 몬스터가 이번 턴에 할 행동

    private HPBar hpBar;
    private Effect effect;
    private DamageText damageText;
    public Transform buffIconPanel;
    public Transform debuffIconPanel;

    private Animator animator;

    void Start()
    {
        TurnManager.instance.RegisterMonster(this);
        monsterActions = FindObjectOfType<Actions>();
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
        effect = FindObjectOfType<Effect>();
        damageText = FindObjectOfType<DamageText>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        isStunned = false;
        GetRandomAction();
    }

    public void SetHPBar(HPBar hpBar)
    {
        this.hpBar = hpBar;
        hpBar.UpdateHealth(currentHealth, maxHealth, shield, poisonStacks);   // HP 바 초기 상태 업데이트
    }

    public void UpdateHPBar()
    {
        hpBar.UpdateHealth(currentHealth, maxHealth, shield, poisonStacks);
    }

    public void Heal(int amount)
    {
        if (damageText != null) damageText.ShowDamage(this.gameObject, 2, amount, 1, 0.1f); //수정 필요
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHPBar();
    }
    
    public void TakeDamage(int damage)
    {
        if (shield >= damage)
        {
            shield -= damage;
            UpdateHPBar();
        }
        else
        {
            int remainingDamage = damage - shield;
            shield = 0;
            currentHealth -= remainingDamage;
            UpdateHPBar();
            if (currentHealth <= 0) Die();
        }
    }

    public void ApplyPoison(int amount)                                     // 몬스터가 독을 받을 때 동작
    {
        poisonStacks += amount;
        UpdateHPBar();
    }

    public void ApplyPoisonDamage()                                         // 독 데미지 적용 메서드
    {
        if (poisonStacks > 0)
        {
            effect.ApplyEffect(this.gameObject, 1, 1, 0.1f); // 수정 필요
            if (damageText != null) damageText.ShowDamage(this.gameObject, 8, poisonStacks, 1, 0.1f); //수정 필요
            TakeDamage(poisonStacks);
            poisonStacks--;
            UpdateHPBar();
        }
    }

    public void ApplyStun()                                                 // 몬스터가 스턴에 빠질 때 동작
    {
        isStunned = true;
    }

    public void HandleStun()                                                // 스턴 처리 메서드
    {
        if (isStunned)
        {
            Debug.Log($"{gameObject.name} is stunned and skips its turn.");
            isStunned = false;  // 스턴 상태 해제
        }
    }

    public void ApplyBuffDebuff(EffectType effectType, int duration, float effectValue, int intValue)
    {
        switch (effectType)
        {
            case EffectType.IncreaseDamage:
                damageMultiplier += effectValue;
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
            case EffectType.DecreaseDamage:
                damageMultiplier -= effectValue;
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
            case EffectType.LifeSteal:
                LifeSteal -= effectValue;
                break;
            case EffectType.ReduceDamage:
                reduceDamage -= effectValue;
                break;
            case EffectType.ReflectDamage:
                reflectDamage -= effectValue;
                break;
            case EffectType.DecreaseDamage:
                damageMultiplier += effectValue;
                break;
            case EffectType.SkipTurn:
                isStunned = false;
                break;
            case EffectType.Confuse:
                isConfuse = false;
                break;
            case EffectType.DelayedImpact:
                if(duration == 0){
                    effect.ApplyEffect(this.gameObject, 0, 1, 0.1f); // 수정 필요
                    if (damageText != null) damageText.ShowDamage(this.gameObject, 0, intValue, 1, 0.1f); //수정 필요
                    TakeDamage(intValue);
                }
                break;
        }
    }

    void Die()  // 몬스터의 체력이 0 밑으로 떨어지면 사망 처리 됨.
    {
        Passive revivePassive = passives.Find(p => p.passiveType == PassiveType.Revive);
        if (revivePassive != null)
        {
            passives.Remove(revivePassive);
            currentHealth = Mathf.RoundToInt(maxHealth * 0.5f);
            Debug.Log($"{gameObject.name} has revived with {currentHealth} health.");
        }
        else
        {
            Debug.Log($"{gameObject.name} died");
            animator.SetTrigger("DieTrigger");
            Destroy(gameObject);
            Destroy(hpBar.gameObject);
        }
    }

    public void GetRandomAction()
    {
        if (actions.Count == 0)
        {
            Debug.LogError("No actions available to select.");
        }
        int randomIndex = Random.Range(0, actions.Count);
        selectedAction = actions[randomIndex];
    }

    public void executeAction()
    {
        PlayerState target = FindObjectOfType<PlayerState>();
        List<MonsterState> monsters = FindObjectsOfType<MonsterState>().ToList();
        GameObject targetObject = target.gameObject;

        if(selectedAction.isRandomTargetBuff)
        {
            int randomIndex = Random.Range(0, monsters.Count);
            targetObject = monsters[randomIndex].gameObject;
        }

        if(isConfuse)
        {
            int randomIndex = Random.Range(0, monsters.Count + 1);
            if(randomIndex == 0) targetObject = target.gameObject;
            else targetObject = monsters[randomIndex - 1].gameObject;
        }

        currentAttacker = this;
        
        switch (selectedAction.actionType)
        {
            // 행동
            case ActionType.Damage:
                monsterActions.DealSingleTargetDamage(targetObject, Mathf.RoundToInt(attackPower * damageMultiplier));
                break;
            case ActionType.StrongAttack:
                monsterActions.DealSingleTargetDamage(targetObject, Mathf.RoundToInt(attackPower * damageMultiplier * 1.25f));
                break;
            case ActionType.LifeStealAttack:
                monsterActions.DealSingleTargetDamage(targetObject, Mathf.RoundToInt(attackPower * damageMultiplier * 0.75f));
                Heal(Mathf.RoundToInt(attackPower * damageMultiplier * 0.75f));
                break;
            case ActionType.Shield:
                int shieldAmount = Mathf.RoundToInt(attackPower * damageMultiplier * 0.75f);
                if (damageText != null) damageText.ShowDamage(this.gameObject, 1, shieldAmount, 1, 0.1f); //수정 필요
                shield += shieldAmount;
                UpdateHPBar();
                break;
            case ActionType.Heal:
                Heal(Mathf.RoundToInt(maxHealth * damageMultiplier * 0.5f));
                break;
            case ActionType.AreaHeal:
                foreach(var monster in monsters)
                {
                    monster.Heal(Mathf.RoundToInt(maxHealth * damageMultiplier * 0.5f));
                }
                break;
            case ActionType.Poison:
                monsterActions.DealSingleTargetPoison(targetObject, Mathf.RoundToInt(attackPower * damageMultiplier * 0.5f));
                break;
            case ActionType.SelfDestruct:
                monsterActions.DealSingleTargetDamage(targetObject, Mathf.RoundToInt(attackPower * damageMultiplier * 1.5f));
                Die();
                break;
            case ActionType.Wait:
                break;

            // 버프 & 디버프
            case ActionType.IncreaseDamage:
                buffDebuffManager.ApplyIncreaseDamageBuff(targetObject, selectedAction.duration, selectedAction.floatValue);
                break;
            case ActionType.IncreaseDamageStack:
                buffDebuffManager.ApplyIncreaseDamageBuff(targetObject, 1000, selectedAction.floatValue);
                break;
            case ActionType.LifeSteal:
                buffDebuffManager.ApplyLifeStealBuff(targetObject, selectedAction.duration, selectedAction.floatValue);
                break;
            case ActionType.ReduceDamage:
                buffDebuffManager.ApplyReduceDamageBuff(targetObject, selectedAction.duration, selectedAction.floatValue);
                break;
            case ActionType.ReflectDamage:
                buffDebuffManager.ApplyReflectDamageBuff(targetObject, selectedAction.duration, selectedAction.floatValue);
                break;
            case ActionType.SkipTurn:
                buffDebuffManager.ApplySkipTurnDebuff(targetObject, selectedAction.duration);
                break;
            case ActionType.Confuse:
                buffDebuffManager.ApplyConfuseDebuff(targetObject, selectedAction.duration);
                break;
        }
    }

    public void AttackMotion()
    {
        animator.SetTrigger("AttackTrigger");
    }
}
