using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public enum CardActionType
{
    Damage, MultiHit, AreaDamage, Poison, AreaPoison, 
    RandomTargetDamage, RandomTargetDamageWithBonus, IncrementalDamage,
    ShieldAttack, StunCheckDamage, PoisonCheckDamage, killEffect,
    Heal, OverhealToDamage, Shield, RestoreResource
}

public enum EffectType
{
    IncreaseDamage, AreaEffect, LifeSteal, ReduceDamage, ReflectDamage, 
    ReduceCost, Purification, Field, SkipTurn, Confuse, RandomAction
}

[System.Serializable]
public class CardAction
{
    public CardActionType actionType;
    public int value;
    public int secondaryValue;
    public CardActionType killEffectType;
    public int thirdValue;
}

[System.Serializable]
public class BuffDebuff
{
    public EffectType effectType;
    public bool isAreaEffect;
    public int duration;
    public float effectValue;
    public int intValue;
}

public class Card : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public PlayerState.AttributeType attributeType;
    public int cost;

    public List<CardAction> actions = new List<CardAction>();           // 공격 카드의 행동들
    public List<BuffDebuff> buffsAndDebuffs = new List<BuffDebuff>();   // 버프와 디버프 카드의 효과들

    private Vector3 startPosition;
    private Actions cardActions;
    private PlayerState player;
    private BuffDebuffManager buffDebuffManager;
    private int originalLayer;

    void Start()
    {
        startPosition = transform.position;
        cardActions = FindObjectOfType<Actions>();                  // Actions 스크립트를 가진 오브젝트를 찾음
        player = FindObjectOfType<PlayerState>();                   // PlayerState 스크립트를 가진 오브젝트를 찾음
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();  // BuffDebuffManager 스크립트를 가진 오브젝트를 찾음
        originalLayer = gameObject.layer;                           // 오리지널 레이어 저장. 드래그한 카드가 레이캐스트에 충돌하는 것을 방지하기 위해 필요.
    }

    public void OnDrag(PointerEventData eventData)          // 카드를 드래그했을 때의 동작
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
        newPosition.z = 0; // z축 값을 0으로 고정
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)       // 카드의 드래그를 마쳤을 때의 동작
    {
        // 드래그 중에 레이어를 일시적으로 변경하여 레이캐스트에 잡히지 않도록 설정
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        bool effectApplied = false;

        if((buffDebuffManager.currentField == PlayerState.AttributeType.Light && attributeType == PlayerState.AttributeType.Light) ||
            (buffDebuffManager.currentField == PlayerState.AttributeType.Dark && attributeType == PlayerState.AttributeType.Dark))
        {
            cost = Mathf.Max(cost - 2, 0);
        }

        // 플레이어의 턴인지 확인
        if (!TurnManager.instance.IsPlayerTurn)
        {
            Debug.Log("It's not the player's turn.");
        }
        else if (player.currentResource >= cost) // 자원 검사를 추가하여 자원이 충분하지 않으면 효과를 적용하지 않음
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))          // 부딪힌 콜라이더가 플레이어 태그인 경우
                {
                    PlayerState targetPlayer = hit.collider.GetComponent<PlayerState>();
                    if (targetPlayer != null)
                    {
                        if (actions.Any(action => IsDamageAction(action))) Debug.Log("Cannot apply damage actions to player.");
                        else
                        {
                            ApplyEffects(targetPlayer, null);
                            effectApplied = true;
                        }
                    }
                }
                else if (hit.collider.CompareTag("Monster"))    // 부딪힌 콜라이더가 몬스터 태그인 경우
                {
                    MonsterState targetMonster = hit.collider.GetComponent<MonsterState>();
                    if (targetMonster != null)
                    {
                        bool hasPlayerEffects = actions.Any(action => IsPlayerEffect(action));
                        bool hasMonsterEffects = actions.Any(action => IsDamageAction(action));
                        bool hasPlayerBuffs = buffsAndDebuffs.Any(buff => IsPlayerBuff(buff));
                        bool hasMonsterDebuffs = buffsAndDebuffs.Any(buff => IsMonsterDebuff(buff));

                        if ((hasPlayerEffects | hasPlayerBuffs) && (hasMonsterEffects | hasMonsterDebuffs))
                        {
                            foreach (var action in actions)
                            {
                                if (IsPlayerEffect(action)) ApplyPlayerEffects(player, action);
                                else if (IsDamageAction(action)) ApplyDamageEffects(targetMonster, action);
                            }
                            foreach (var buffDebuff in buffsAndDebuffs)
                            {
                                if (IsPlayerBuff(buffDebuff)) ApplyToPlayer(player, buffDebuff);
                                else if (IsMonsterDebuff(buffDebuff))
                                {
                                    if (buffDebuff.isAreaEffect) ApplyToAllTargets(buffDebuff);
                                    else ApplyToMonster(targetMonster, buffDebuff);
                                }
                            }
                        }
                        else if (hasMonsterEffects | hasMonsterDebuffs) 
                        {
                            ApplyEffects(null, targetMonster);
                        }
                        else if (hasPlayerEffects | hasPlayerBuffs) ApplyEffects(player, null);
                        effectApplied = true;
                    }
                }
            }
        }
        else Debug.Log("Not enough resources to play this card.");

        // 원래 레이어로 복구
        gameObject.layer = originalLayer;
        transform.position = startPosition; // 드래그 종료 후 카드 원위치

        // 자원 소비 및 속성 경험치 추가
        if (effectApplied && player != null)
        {
            player.SpendResource(cost);
            player.AddAttributeExperience(attributeType, GetAttributeExperienceGain(cost));     // 카드 사용 후 속성 경험치 추가
            player.AttackMotion();          // attackMotion을 이곳으로 이동
        }
    }

    private void ApplyEffects(PlayerState playerTarget, MonsterState monsterTarget)
    {
        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case CardActionType.Damage:
                case CardActionType.MultiHit:
                case CardActionType.AreaDamage:
                case CardActionType.Poison:
                case CardActionType.AreaPoison:
                case CardActionType.RandomTargetDamage:
                case CardActionType.RandomTargetDamageWithBonus:
                case CardActionType.IncrementalDamage:
                case CardActionType.OverhealToDamage:
                case CardActionType.ShieldAttack:
                case CardActionType.StunCheckDamage:
                case CardActionType.PoisonCheckDamage:
                    if (monsterTarget != null)
                    {
                        ApplyDamageEffects(monsterTarget, action);
                    }
                    break;
                case CardActionType.Heal:
                case CardActionType.Shield:
                case CardActionType.RestoreResource:
                    if (playerTarget != null)
                    {
                        ApplyPlayerEffects(playerTarget, action);
                    }
                    break;
            }
        }
        foreach (var buffDebuff in buffsAndDebuffs)
        {
            switch (buffDebuff.effectType)
            {
                case EffectType.SkipTurn:
                case EffectType.RandomAction:
                case EffectType.Confuse:
                    if (buffDebuff.isAreaEffect && monsterTarget != null)
                    {
                        ApplyToAllTargets(buffDebuff);
                    }
                    else if (monsterTarget != null)
                    {
                        ApplyToMonster(monsterTarget, buffDebuff);
                    }
                    break;
                case EffectType.IncreaseDamage:
                case EffectType.AreaEffect:
                case EffectType.LifeSteal:
                case EffectType.ReduceDamage:
                case EffectType.ReflectDamage:
                case EffectType.ReduceCost:
                case EffectType.Purification:
                case EffectType.Field:
                    if (playerTarget != null)
                    {
                        ApplyToPlayer(playerTarget, buffDebuff);
                    }
                    break;
            }
        }
    }

    private void ApplyDamageEffects(MonsterState target, CardAction action)
    {
        List<MonsterState> monsters = FindObjectsOfType<MonsterState>().ToList();
        if(player.isConfuse)
        {
            int randomIndex = Random.Range(0, monsters.Count);
            target = monsters[randomIndex];
        }
        switch (action.actionType)
        {
            case CardActionType.Damage:
                if(player.isAreaEffect) cardActions.DealAreaDamage(monsters.Select(m => m.gameObject).ToList(), action.value, 1, attributeType);
                else cardActions.DealMultipleHits(target.gameObject, action.value, 1, actions.FirstOrDefault(a => a.actionType == CardActionType.killEffect), attributeType);
                break;
            case CardActionType.MultiHit:
                if(player.isAreaEffect) cardActions.DealAreaDamage(monsters.Select(m => m.gameObject).ToList(), action.value, action.secondaryValue, attributeType);
                else cardActions.DealMultipleHits(target.gameObject, action.value, action.secondaryValue, null, attributeType);
                break;
            case CardActionType.AreaDamage:
                cardActions.DealAreaDamage(monsters.Select(m => m.gameObject).ToList(), action.value, action.secondaryValue, attributeType);
                break;
            case CardActionType.Poison:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.ApplyPoison(monster.gameObject, action.value, attributeType);
                    }
                }
                else cardActions.ApplyPoison(target.gameObject, action.value, attributeType);
                break;
            case CardActionType.AreaPoison:
                foreach (var monster in FindObjectsOfType<MonsterState>())
                {
                    cardActions.ApplyPoison(monster.gameObject, action.value, attributeType);
                }
                break;
            case CardActionType.RandomTargetDamage:
                cardActions.DealRandomTargetDamage(monsters.Select(m => m.gameObject).ToList(), action.value, action.secondaryValue, attributeType);
                break;
            case CardActionType.RandomTargetDamageWithBonus:
                cardActions.DealRandomTargetDamageWithBonus(monsters.Select(m => m.gameObject).ToList(), action.value, action.secondaryValue, action.thirdValue, attributeType);
                break;
            case CardActionType.IncrementalDamage:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.DealIncreasingDamage(monster.gameObject, action.value, action.secondaryValue, attributeType);
                    }
                }
                else cardActions.DealIncreasingDamage(target.gameObject, action.value, action.secondaryValue, attributeType);
                break;
            case CardActionType.OverhealToDamage:
                    if (target is MonsterState targetMonsterOverheal)
                    {
                        int overheal = action.value - (player.maxHealth - player.currentHealth);
                        if (overheal > 0)
                        {
                            player.Heal(player.maxHealth - player.currentHealth);
                            if(player.isAreaEffect) cardActions.DealAreaDamage(FindObjectsOfType<MonsterState>().Select(m => m.gameObject).ToList(), action.value, 1, attributeType);
                            else cardActions.DealMultipleHits(targetMonsterOverheal.gameObject, overheal, 1, null, attributeType);
                        }
                        else
                        {
                            player.Heal(action.value);
                        }
                    }
                    break;
            case CardActionType.ShieldAttack:
                if(player.isAreaEffect) cardActions.DealAreaDamage(monsters.Select(m => m.gameObject).ToList(), player.shield, 1, attributeType);
                else cardActions.DealMultipleHits(target.gameObject, player.shield, 1, null, attributeType);
                break;
            case CardActionType.StunCheckDamage:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        if (monster.isStunned) cardActions.DealMultipleHits(monster.gameObject, action.value + action.secondaryValue, 1, null, attributeType);
                        else cardActions.DealMultipleHits(monster.gameObject, action.value, 1, null, attributeType);
                    }
                }
                else
                {
                    if (target.isStunned) cardActions.DealMultipleHits(target.gameObject, action.value + action.secondaryValue, 1, null, attributeType);
                    else cardActions.DealMultipleHits(target.gameObject, action.value, 1, null, attributeType);
                }
                break;
            case CardActionType.PoisonCheckDamage:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        if (monster.poisonStacks > 0) cardActions.ApplyPoison(monster.gameObject, action.value + action.secondaryValue, attributeType);
                        else cardActions.ApplyPoison(monster.gameObject, action.value, attributeType);
                    }
                }
                else
                {
                    if (target.poisonStacks > 0) cardActions.ApplyPoison(target.gameObject, action.value + action.secondaryValue, attributeType);
                    else cardActions.ApplyPoison(target.gameObject, action.value, attributeType);
                }
                break;
        }
    }

    private void ApplyPlayerEffects(PlayerState target, CardAction action)
    {
        switch (action.actionType)
        {
            case CardActionType.Heal:
                target.Heal(action.value);
                break;
            case CardActionType.Shield:
                target.ApplyShield(action.value);
                break;
            case CardActionType.RestoreResource:
                target.RestoreResource(action.value);
                break;
        }
    }

    private void ApplyToPlayer(PlayerState target, BuffDebuff buffDebuff)
    {
        switch (buffDebuff.effectType)
        {
            case EffectType.IncreaseDamage:
                buffDebuffManager.ApplyIncreaseDamageBuff(target.gameObject, buffDebuff.duration, buffDebuff.effectValue);
                break;
            case EffectType.AreaEffect:
                buffDebuffManager.ApplyAreaEffectBuff(target.gameObject, buffDebuff.duration);
                break;
            case EffectType.LifeSteal:
                buffDebuffManager.ApplyLifeStealBuff(target.gameObject, buffDebuff.duration, buffDebuff.effectValue);
                break;
            case EffectType.ReduceDamage:
                buffDebuffManager.ApplyReduceDamageBuff(target.gameObject, buffDebuff.duration, buffDebuff.effectValue);
                break;
            case EffectType.ReflectDamage:
                buffDebuffManager.ApplyReflectDamageBuff(target.gameObject, buffDebuff.duration, buffDebuff.effectValue);
                break;
            case EffectType.ReduceCost:
                buffDebuffManager.ApplyReduceCostBuff(target.gameObject, buffDebuff.duration, buffDebuff.intValue);
                break;
            case EffectType.Purification:
                buffDebuffManager.ApplyPurification(target.gameObject);
                break;
            case EffectType.Field:
                buffDebuffManager.ApplyField(attributeType);
                break;
        }
    }

    private void ApplyToMonster(MonsterState target, BuffDebuff buffDebuff)
    {
        switch (buffDebuff.effectType)
        {
            case EffectType.SkipTurn:
                buffDebuffManager.ApplySkipTurnDebuff(target.gameObject, buffDebuff.duration);
                break;
            case EffectType.RandomAction:
                buffDebuffManager.ApplyRandomActionDebuff(target.gameObject);
                break;
            case EffectType.Confuse:
                buffDebuffManager.ApplyConfuseDebuff(target.gameObject, buffDebuff.duration);
                break;
        }
    }

    private void ApplyToAllTargets(BuffDebuff buffDebuff)
    {
        foreach (MonsterState monster in FindObjectsOfType<MonsterState>())
        {
            ApplyToMonster(monster, buffDebuff);
        }
    }

    // 코스트에 따라 속성 경험치를 반환하는 함수
    private int GetAttributeExperienceGain(int cost)
    {
        int[] experienceGain = { 0, 1, 3, 5, 7, 9, 11, 14, 17, 20 };
        if (cost < experienceGain.Length) return experienceGain[cost];
        return 0;
    }

    private bool IsDamageAction(CardAction action)
    {
        return action.actionType == CardActionType.Damage ||
               action.actionType == CardActionType.MultiHit ||
               action.actionType == CardActionType.AreaDamage ||
               action.actionType == CardActionType.Poison ||
               action.actionType == CardActionType.AreaPoison ||
               action.actionType == CardActionType.RandomTargetDamage ||
               action.actionType == CardActionType.RandomTargetDamageWithBonus ||
               action.actionType == CardActionType.IncrementalDamage ||
               action.actionType == CardActionType.OverhealToDamage ||
               action.actionType == CardActionType.ShieldAttack ||
               action.actionType == CardActionType.StunCheckDamage ||
               action.actionType == CardActionType.PoisonCheckDamage;
    }

    private bool IsPlayerEffect(CardAction action)
    {
        return action.actionType == CardActionType.Heal ||
               action.actionType == CardActionType.Shield ||
               action.actionType == CardActionType.RestoreResource;
    }

    private bool IsMonsterDebuff(BuffDebuff effect)
    {
        return effect.effectType == EffectType.SkipTurn ||
               effect.effectType == EffectType.RandomAction ||
               effect.effectType == EffectType.Confuse;
    }

    private bool IsPlayerBuff(BuffDebuff effect)
    {
        return effect.effectType == EffectType.IncreaseDamage ||
               effect.effectType == EffectType.AreaEffect ||
               effect.effectType == EffectType.LifeSteal ||
               effect.effectType == EffectType.ReduceDamage ||
               effect.effectType == EffectType.ReflectDamage ||
               effect.effectType == EffectType.ReduceCost ||
               effect.effectType == EffectType.Purification ||
               effect.effectType == EffectType.Field;
    }
}
