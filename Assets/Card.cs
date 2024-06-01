using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public enum CardActionType
{
    Damage,                     // 데미지
    Heal,                       // 회복
    Poison,                     // 독
    Shield,                     // 방어
    MultiDamage,                // 광역 데미지
    MultiPoison,                // 광역 독
    RestoreResource,            // 자원 회복
    OverhealToDamage,           // 회복 시 초과량만큼 데미지
    RandomTargetDamage,         // 대상을 랜덤으로 지정하는 데미지
    StunCheckDamage,            // 데미지 + 스턴 상태 이상을 갖고 있는 경우 추가 데미지
    PoisonCheckDamage,          // 독 + 중독 상태 이상을 갖고 있는 경우 추가 독
    killEffect                  // 데미지 + 처치 시 추가 행동 (미완성임. 수정이 필요하니 건들지 말 것)
}

[System.Serializable]
public class CardAction
{
    public CardActionType actionType;       // 카드의 행동 방식을 지정
    public int value;                       // 지정된 행동 방식의 값을 정의
    public int secondaryValue;              // 지정된 행동 방식에서 두 번째 값이 필요한 경우에 정의
    public CardActionType killEffectType;   // 처치 시 해야할 추가 행동 방식을 지정
    public int killEffectValue;             // 처치 시 지정된 행동 방식의 값을 정의
}

public class Card : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public int cost;
    public List<CardAction> actions = new List<CardAction>();

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
    }                       // 해당 카드의 레이어를 영구 변경하는 방식이 아니라 드래그 동작을 할때만 콜라이더에 부딪히지 않도록 임시로 레이어를 변경함.

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

        // 원래 레이어로 복구
        gameObject.layer = originalLayer;
        transform.position = startPosition; // 드래그 종료 후 카드 원위치

        // 자원 소비
        if (effectApplied && player != null)
        {
            player.SpendResource(cost);
        }
    }

    void ApplyEffects(MonsterState target)                  // 미리 지정해둔 카드의 행동 방식에 따라 적용 효과를 달리함. (몬스터 전용)
    {
        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case CardActionType.Damage:                 // 카드 행동 방식이 데미지로 설정된 경우
                    cardActions.DealSingleTargetDamage(target.gameObject, action.value, actions.FirstOrDefault(a => a.actionType == CardActionType.killEffect));
                    break;
                case CardActionType.Poison:                 // 카드 행동 방식이 독으로 설정된 경우
                    cardActions.ApplyPoison(target.gameObject, action.value);
                    break;
                case CardActionType.MultiDamage:            // 카드 행동 방식이 광역 데미지로 설정된 경우
                    cardActions.DealAreaDamage(FindObjectsOfType<MonsterState>().Select(m => m.gameObject).ToList(), action.value);
                    break;
                case CardActionType.MultiPoison:            // 카드 행동 방식이 광역 독으로 설정된 경우
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.ApplyPoison(monster.gameObject, action.value);
                    }
                    break;
                case CardActionType.Heal:                   // 카드 행동 방식이 회복, 방어, 자원 회복으로 설정된 경우
                case CardActionType.Shield:                 // 이 함수 아래 쪽에 동일한 이름의 함수를 동작시킴 (아래쪽 함수는 플레이어 전용)
                case CardActionType.RestoreResource:
                    ApplyEffects(player); // 힐, 방어, 자원 회복은 플레이어에게 적용
                    break;
                case CardActionType.OverhealToDamage:       // 카드 행동 방식이 회복 후 초과량만큼 데미지로 설정된 경우
                    int overheal = action.value - (player.maxHealth - player.currentHealth);
                    if (overheal > 0)
                    {
                        player.Heal(player.maxHealth - player.currentHealth);
                        cardActions.DealSingleTargetDamage(target.gameObject, overheal);
                    }
                    else
                    {
                        player.Heal(action.value);
                    }
                    break;
                case CardActionType.RandomTargetDamage:     // 카드 행동 방식이 랜덤 대상에게 데미지로 설정된 경우
                    cardActions.DealRandomTargetDamage(FindObjectsOfType<MonsterState>().Select(m => m.gameObject).ToList(), action.value, action.secondaryValue);
                    break;
                case CardActionType.StunCheckDamage:        // 카드 행동 방식이 스턴 상태 이상인 적에게 추가 데미지로 설정된 경우
                    if (target.isStunned)
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value + action.secondaryValue);
                    }
                    else
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value);
                    }
                    break;
                case CardActionType.PoisonCheckDamage:      // 카드 행동 방식이 중독 상태 이상인 적에게 추가 독으로 설정된 경우
                    if (target.poisonStacks > 0)
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value + action.secondaryValue);
                    }
                    else
                    {
                        cardActions.DealSingleTargetDamage(target.gameObject, action.value);
                    }
                    break;
            }
        }
    }

    void ApplyEffects(PlayerState target)                   // 미리 지정해둔 카드 행동 방식에 따른 효과 적용 (플레이어 전용)
    {
        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case CardActionType.Heal:                   // 카드 행동 방식이 회복인 경우
                    target.Heal(action.value);
                    break;
                case CardActionType.Shield:                 // 카드 행동 방식이 방어인 경우
                    target.ApplyShield(action.value);
                    break;
                case CardActionType.RestoreResource:        // 카드 행동 방식이 자원 회복인 경우
                    cardActions.RestoreResource(target, action.value);
                    break;
            }
        }
    }
}
