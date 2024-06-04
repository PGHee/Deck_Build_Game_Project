using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public enum CardActionType
{
    Damage, Heal, Poison, Shield, MultiDamage, MultiPoison,     // 데미지, 회복, 독, 방어, 광역 데미지, 광역 독
    RestoreResource, OverhealToDamage, RandomTargetDamage,      // 자원 회복, 회복 시 초과량 데미지, 랜덤 지정 데미지
    StunCheckDamage, PoisonCheckDamage, killEffect, MultiHit,   // 스턴 상태 추가 데미지, 독 상태 추가 독, 처치 시 추가 행동, 다중 타격
    IncrementalDamage, RandomTargetDamageWithBonus              // 타격할수록 데미지 증가, 랜덤 지정 데미지와 추가 타격
}

[System.Serializable]
public class CardAction
{
    public CardActionType actionType;       // 카드의 행동 방식을 지정
    public int value;                       // 지정된 행동 방식의 값을 정의
    public int secondaryValue;              // 지정된 행동 방식에서 두 번째 값이 필요한 경우에 정의
    public CardActionType killEffectType;   // 처치 시 해야할 추가 행동 방식을 지정
    public int thirdValue;                  // 세 번째 값이 필요한 경우에 정의. 현재는 처치 시 추가 행동의 값과 동일 대상 타격 기준으로 사용
}

public class Card : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public PlayerState.AttributeType attributeType;             // 카드의 속성 지정
    public int cost;                                            // 카드 소모 코스트 지정
    public List<CardAction> actions = new List<CardAction>();   // 카드 행동 지정

    private Vector3 startPosition;
    private CardActions cardActions;
    private PlayerState player;
    private int originalLayer;

    void Start()
    {
        startPosition = transform.position;
        cardActions = FindObjectOfType<CardActions>();      // CardActions 스크립트를 가진 오브젝트를 찾음
        player = FindObjectOfType<PlayerState>();           // PlayerState 스크립트를 가진 오브젝트를 찾음
        originalLayer = gameObject.layer;                   // 오리지널 레이어 저장. 드래그한 카드가 레이캐스트에 충돌하는 것을 방지하기 위해 필요.
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

        // 자원 검사를 추가하여 자원이 충분하지 않으면 효과를 적용하지 않음
        if (player.currentResource >= cost)
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))          // 부딪힌 콜라이더가 플레이어 태그인 경우
                {
                    PlayerState target = hit.collider.GetComponent<PlayerState>();
                    if (target != null)
                    {
                        ApplyEffects(target);
                        effectApplied = true;
                    }
                }
                else if (hit.collider.CompareTag("Monster"))    // 부딪힌 콜라이더가 몬스터 태그인 경우
                {
                    MonsterState target = hit.collider.GetComponent<MonsterState>();
                    if (target != null)
                    {
                        ApplyEffects(target);
                        effectApplied = true;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Not enough resources to play this card.");
        }

        // 원래 레이어로 복구
        gameObject.layer = originalLayer;
        transform.position = startPosition; // 드래그 종료 후 카드 원위치

        // 자원 소비 및 속성 경험치 추가
        if (effectApplied && player != null)
        {
            player.SpendResource(cost);
            // 카드 사용 후 속성 경험치 추가
            player.AddAttributeExperience(attributeType, GetAttributeExperienceGain(cost));
        }
    }


    void ApplyEffects(MonsterState target)                          // 미리 지정해둔 카드의 행동 방식에 따라 적용 효과를 달리함. (몬스터 전용)
    {
        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case CardActionType.Damage:                         // 카드 행동 방식이 데미지로 설정된 경우
                    cardActions.DealMultipleTargetDamage(target.gameObject, action.value, 1, actions.FirstOrDefault(a => a.actionType == CardActionType.killEffect), attributeType);
                    break;
                case CardActionType.Poison:                         // 카드 행동 방식이 독으로 설정된 경우
                    cardActions.ApplyPoison(target.gameObject, action.value, attributeType);
                    break;
                case CardActionType.MultiDamage:                    // 카드 행동 방식이 광역 데미지로 설정된 경우
                    cardActions.DealAreaDamage(FindObjectsOfType<MonsterState>().Select(m => m.gameObject).ToList(), action.value, attributeType);
                    break;
                case CardActionType.MultiPoison:                    // 카드 행동 방식이 광역 독으로 설정된 경우
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.ApplyPoison(monster.gameObject, action.value, attributeType);
                    }
                    break;
                case CardActionType.Heal:                           // 카드 행동 방식이 회복, 방어, 자원 회복으로 설정된 경우
                case CardActionType.Shield:                         // 이 함수 아래 쪽에 동일한 이름의 함수를 동작시킴 (아래쪽 함수는 플레이어 전용)
                case CardActionType.RestoreResource:
                    ApplyEffects(player);   // 힐, 방어, 자원 회복은 플레이어에게 적용
                    break;
                case CardActionType.OverhealToDamage:               // 카드 행동 방식이 회복 후 초과량만큼 데미지로 설정된 경우
                    int overheal = action.value - (player.maxHealth - player.currentHealth);
                    if (overheal > 0)
                    {
                        player.Heal(player.maxHealth - player.currentHealth);
                        cardActions.DealSingleTargetDamage(target.gameObject, overheal, null, attributeType);
                    }
                    else
                    {
                        player.Heal(action.value);
                    }
                    break;
                case CardActionType.RandomTargetDamage:             // 카드 행동 방식이 랜덤 대상에게 데미지로 설정된 경우
                    cardActions.DealRandomTargetDamage(FindObjectsOfType<MonsterState>().Select(m => m.gameObject).ToList(), action.value, action.secondaryValue, attributeType);
                    break;
                case CardActionType.StunCheckDamage:                // 카드 행동 방식이 스턴 상태 이상인 적에게 추가 데미지로 설정된 경우
                    if (target.isStunned)
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value + action.secondaryValue, null, attributeType);
                    }
                    else
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value, null, attributeType);
                    }
                    break;
                case CardActionType.MultiHit:                       // 다중 타격
                    cardActions.DealMultipleTargetDamage(target.gameObject, action.value, action.secondaryValue, null, attributeType);
                    break;
                case CardActionType.IncrementalDamage:              // 다중 타격 + 타격 시마다 데미지 증가
                    cardActions.DealIncreasingDamage(target.gameObject, action.value, action.secondaryValue, attributeType);
                    break;
                case CardActionType.PoisonCheckDamage:              // 카드 행동 방식이 중독 상태 이상인 적에게 추가 독으로 설정된 경우
                    if (target.poisonStacks > 0)
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value + action.secondaryValue, null, attributeType);
                    }
                    else
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value, null, attributeType);
                    }
                    break;
                case CardActionType.RandomTargetDamageWithBonus:    // 랜덤 타겟 데미지 + 동일 대상 타격 시 추가 타격
                    cardActions.DealRandomTargetDamageWithBonus(
                        FindObjectsOfType<MonsterState>().Select(m => m.gameObject).ToList(),
                        action.value, action.secondaryValue, action.thirdValue, attributeType);
                    break;
            }
        }
    }

    void ApplyEffects(PlayerState target)                           // 미리 지정해둔 카드의 행동 방식에 따라 적용 효과를 달리함. (플레이어 전용)
    {
        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case CardActionType.Heal:                           // 카드 행동 방식이 힐로 설정된 경우
                    target.Heal(action.value);
                    break;
                case CardActionType.Shield:                         // 카드 행동 방식이 방어로 설정된 경우
                    target.ApplyShield(action.value);
                    break;
                case CardActionType.RestoreResource:                // 카드 행동 방식이 자원 회복으로 설정된 경우
                    target.RestoreResource(action.value);
                    break;
            }
        }
    }


    // 코스트에 따라 속성 경험치를 반환하는 함수
    private int GetAttributeExperienceGain(int cost)
    {
        int[] experienceGain = { 1, 3, 5, 7, 9, 11, 14, 17, 20 };
        if (cost - 1 < experienceGain.Length)
        {
            return experienceGain[cost - 1];
        }
        return 0;
    }
}
