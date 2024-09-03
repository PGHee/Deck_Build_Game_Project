using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public enum CardActionType
{
    Damage, MultiHit, AreaDamage, RandomTargetDamage, RandomTargetDamageWithBonus, IncrementalDamage,
    TrueDamage, TrueAreaDamage, ShieldAttack, OverhealToDamage, StunCheckDamage, 
    Poison, AreaPoison, DoublePoison, PoisonToDamage, RandomTargetPoison, PoisonCheckDamage,
    killEffect, Heal, Shield, DoubleShield, RestoreResource
}

public enum EffectType
{
    IncreaseDamage, AreaEffect, LifeSteal, ReduceDamage, ReflectDamage, ReduceCost, Purification, 
    Field, DecreaseDamage, SkipTurn, Confuse, RandomAction, DelayedImpact
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

public class Card : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerState.AttributeType attributeType;
    public int cost;

    public List<CardAction> actions = new List<CardAction>();           // 공격 카드의 행동들
    public List<BuffDebuff> buffsAndDebuffs = new List<BuffDebuff>();   // 버프와 디버프 카드의 효과들

    public Sprite cardFrontFrame;
    public Sprite cardBackFrame;
    public Sprite cardImage;
    public string cardName;
    private string cardDescription;
    public Sprite[] attributeSprites;

    public SpriteRenderer cardFront;
    public SpriteRenderer cardBack;
    public TextMeshProUGUI cardNameText;
    public SpriteRenderer cardAttributeImageUI;
    public SpriteRenderer cardImageUI;
    public TextMeshProUGUI cardTypeText;
    public TextMeshProUGUI cardDescriptionText;
    public TextMeshProUGUI cardCostText;

    private Vector3 startPosition;
    private Actions cardActions;
    private PlayerState player;
    private BuffDebuffManager buffDebuffManager;
    private int originalLayer;
    public HandControl handController;
    public DeckManager deckManager;


    private bool isDragging = false;
    public GameObject originalPrefab;
    private GameObject zoomedCard; // 확대된 카드 오브젝트
    private Camera mainCamera;
    private Vector3 fixedScreenPosition = new Vector3(0.85f, 0.55f, 10f); // 화면 특정 위치, Z값은 카메라와의 거리
    private Vector3 originalScale; // 원본 스케일

    private SystemMessage message;

    private void Awake()
    {
        if (originalPrefab == null)
        {
            string prefabName = gameObject.name.Replace("(Clone)", "").Trim(); // 프리팹 이름 추출
            originalPrefab = Resources.Load<GameObject>($"Prefabs/Card/{prefabName}");
        }
    }

    void Start()
    {
        message = FindObjectOfType<SystemMessage>();
        startPosition = transform.position;
        cardActions = FindObjectOfType<Actions>();                  // Actions 스크립트를 가진 오브젝트를 찾음
        player = FindObjectOfType<PlayerState>();                   // PlayerState 스크립트를 가진 오브젝트를 찾음
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();  // BuffDebuffManager 스크립트를 가진 오브젝트를 찾음
        UpdateCardUI();
        originalLayer = gameObject.layer;                           // 오리지널 레이어 저장. 드래그한 카드가 레이캐스트에 충돌하는 것을 방지하기 위해 필요.
        mainCamera = Camera.main;                                   // 메인 카메라를 찾음
        originalScale = transform.localScale;                       // 원본 스케일 저장
        handController = FindObjectOfType<HandControl>();
        deckManager = FindObjectOfType<DeckManager>();

    }

    public void OnDrag(PointerEventData eventData)          // 카드를 드래그했을 때의 동작
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
        newPosition.z = 0; // z축 값을 0으로 고정
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)       // 카드의 드래그를 마쳤을 때의 동작
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // 드래그 중에 레이어를 일시적으로 변경하여 레이캐스트에 잡히지 않도록 설정

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        bool effectApplied = false;
        int adjustedCost = cost;
        if ((buffDebuffManager.currentField == PlayerState.AttributeType.Light && attributeType == PlayerState.AttributeType.Light) ||
            (buffDebuffManager.currentField == PlayerState.AttributeType.Dark && attributeType == PlayerState.AttributeType.Dark))
        {
            adjustedCost = Mathf.Max(cost - 2, 0);
        }

        if (!TurnManager.instance.IsPlayerTurn)                 // 플레이어의 턴인지 확인
        {
            message.ShowSystemMessage("플레이어 턴이 아닙니다.");
        }
        else if (player.currentResource >= adjustedCost)        // 자원 검사를 추가하여 자원이 충분하지 않으면 효과를 적용하지 않음
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))          // 부딪힌 콜라이더가 플레이어 태그인 경우
                {
                    PlayerState targetPlayer = hit.collider.GetComponent<PlayerState>();
                    if (targetPlayer != null)
                    {
                        if (actions.Any(action => IsDamageAction(action))) message.ShowSystemMessage("몬스터에게 사용해 주세요.");
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
        else message.ShowSystemMessage("보유한 서클이 부족합니다.");

        // 원래 레이어로 복구
        gameObject.layer = originalLayer;
        transform.position = startPosition; // 드래그 종료 후 카드 원위치

        // 자원 소비 및 속성 경험치 추가
        if (effectApplied && player != null)
        {
            player.SpendResource(adjustedCost);
            player.AddAttributeExperience(attributeType, GetAttributeExperienceGain(adjustedCost));     // 카드 사용 후 속성 경험치 추가
            player.AttackMotion();          // attackMotion을 이곳으로 이동

            handController.HandSort(gameObject, false);     // 자신을 제외한 카드 정렬
            deckManager.graveArray = deckManager.Card2Grave(int.Parse(gameObject.name));    // 사용한 카드 묘지로

            Destroy(gameObject);
            
        }

        isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging && zoomedCard != null) Destroy(zoomedCard);
        else if(isDragging) return;
        
        if (originalPrefab != null)
        {
            zoomedCard = Instantiate(originalPrefab);   // 원본 프리팹을 복제하여 확대된 카드로 사용

            // 화면의 특정 위치에 고정 (스크린 좌표를 월드 좌표로 변환)
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width * fixedScreenPosition.x, Screen.height * fixedScreenPosition.y, fixedScreenPosition.z));
            zoomedCard.transform.position = worldPosition;
            zoomedCard.transform.localScale = originalScale * 2.0f; // 크기 조정
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (zoomedCard != null) // 마우스가 카드에서 벗어나면 확대된 카드 제거
        {
            Destroy(zoomedCard);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        startPosition = transform.position;
    }

    public void UpdateCardUI()
    {
        cardFront.sprite = cardFrontFrame;
        cardBack.sprite = cardBackFrame;
        cardNameText.text = cardName;
        cardImageUI.sprite = cardImage;
        cardDescriptionText.text = GetCardDescription();
        cardCostText.text = cost.ToString();
        cardTypeText.text = GetCardType();
        cardAttributeImageUI.sprite = attributeSprites[(int)attributeType];
    }

    public string GetCardType()
    {
        bool hasAttack = actions.Any(IsDamageAction);
        bool hasSupport = actions.Any(IsPlayerEffect);
        bool hasBuff = buffsAndDebuffs.Any(IsPlayerBuff);
        bool hasDebuff = buffsAndDebuffs.Any(IsMonsterDebuff);
        bool isField = buffsAndDebuffs.Any(buff => buff.effectType == EffectType.Field);

        List<string> types = new List<string>();

        if (hasAttack) types.Add("공격");
        if (hasSupport) types.Add("지원");
        if (hasBuff) types.Add("버프");
        if (hasDebuff) types.Add("디버프");
        if (isField) types.Add("필드");

        return string.Join("+", types);
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
                case CardActionType.RandomTargetDamage:
                case CardActionType.RandomTargetDamageWithBonus:
                case CardActionType.IncrementalDamage:
                case CardActionType.TrueDamage:
                case CardActionType.TrueAreaDamage:
                case CardActionType.ShieldAttack:
                case CardActionType.OverhealToDamage:
                case CardActionType.StunCheckDamage:
                case CardActionType.Poison:
                case CardActionType.AreaPoison:
                case CardActionType.DoublePoison:
                case CardActionType.PoisonToDamage:
                case CardActionType.RandomTargetPoison:
                case CardActionType.PoisonCheckDamage:
                    if (monsterTarget != null)
                    {
                        ApplyDamageEffects(monsterTarget, action);
                    }
                    break;
                case CardActionType.Heal:
                case CardActionType.Shield:
                case CardActionType.DoubleShield:
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
                case EffectType.DecreaseDamage:
                case EffectType.SkipTurn:
                case EffectType.RandomAction:
                case EffectType.Confuse:
                case EffectType.DelayedImpact:
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
        List<GameObject> monsterObjects = GameObject.FindGameObjectsWithTag("Monster").ToList();
        List<MonsterState> monsters = monsterObjects.Select(obj => obj.GetComponent<MonsterState>()).ToList();
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
            case CardActionType.TrueDamage:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.DealTrueDamage(monster.gameObject, action.value, action.secondaryValue, attributeType);
                    }
                }
                else cardActions.DealTrueDamage(target.gameObject, action.value, action.secondaryValue, attributeType);
                break;
            case CardActionType.TrueAreaDamage:
                foreach (var monster in FindObjectsOfType<MonsterState>())
                {
                    cardActions.DealTrueDamage(monster.gameObject, action.value, action.secondaryValue, attributeType);
                }
                break;
            case CardActionType.ShieldAttack:
                if(player.isAreaEffect) cardActions.DealAreaDamage(monsters.Select(m => m.gameObject).ToList(), player.shield, 1, attributeType);
                else cardActions.DealMultipleHits(target.gameObject, player.shield, 1, null, attributeType);
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
            case CardActionType.Poison:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.DealMultiplePoison(monster.gameObject, action.value, action.secondaryValue, attributeType);
                    }
                }
                else cardActions.DealMultiplePoison(target.gameObject, action.value, action.secondaryValue, attributeType);
                break;
            case CardActionType.AreaPoison:
                foreach (var monster in FindObjectsOfType<MonsterState>())
                {
                    cardActions.DealMultiplePoison(monster.gameObject, action.value, action.secondaryValue, attributeType);
                }
                break;
            case CardActionType.DoublePoison:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.DealMultiplePoison(monster.gameObject, monster.poisonStacks, 1, attributeType);
                    }
                }
                else cardActions.DealMultiplePoison(target.gameObject, target.poisonStacks, 1, attributeType);
                break;
            case CardActionType.PoisonToDamage:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        cardActions.DealMultipleHits(monster.gameObject, monster.poisonStacks, 1, null, attributeType);
                        monster.poisonStacks = 0;
                        monster.UpdateHPBar();
                    }
                }
                else 
                {
                    cardActions.DealMultipleHits(target.gameObject, target.poisonStacks, 1, null, attributeType);
                    target.poisonStacks = 0;
                    target.UpdateHPBar();
                }
                break;
            case CardActionType.RandomTargetPoison:
                cardActions.DealRandomTargetPoison(monsters.Select(m => m.gameObject).ToList(), action.value, action.secondaryValue, attributeType);
                break;
            case CardActionType.PoisonCheckDamage:
                if(player.isAreaEffect)
                {
                    foreach (var monster in FindObjectsOfType<MonsterState>())
                    {
                        if (monster.poisonStacks > 0) cardActions.DealMultiplePoison(monster.gameObject, action.value + action.secondaryValue, 1, attributeType);
                        else cardActions.DealMultiplePoison(monster.gameObject, action.value, 1, attributeType);
                    }
                }
                else
                {
                    if (target.poisonStacks > 0) cardActions.DealMultiplePoison(target.gameObject, action.value + action.secondaryValue, 1, attributeType);
                    else cardActions.DealMultiplePoison(target.gameObject, action.value, 1, attributeType);
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
            case CardActionType.DoubleShield:
                target.ApplyShield(player.shield);
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
            case EffectType.DecreaseDamage:
                buffDebuffManager.ApplyDecreaseDamageDebuff(target.gameObject, buffDebuff.duration, buffDebuff.effectValue);
                break;
            case EffectType.SkipTurn:
                buffDebuffManager.ApplySkipTurnDebuff(target.gameObject, buffDebuff.duration);
                break;
            case EffectType.RandomAction:
                buffDebuffManager.ApplyRandomActionDebuff(target.gameObject);
                break;
            case EffectType.Confuse:
                buffDebuffManager.ApplyConfuseDebuff(target.gameObject, buffDebuff.duration);
                break;
            case EffectType.DelayedImpact:
                buffDebuffManager.ApplyDelayedImpact(target.gameObject, buffDebuff.intValue);
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
        int[] experienceGain = { 0, 25, 40, 60, 85, 115, 150, 14, 17, 20 };
        if (cost < experienceGain.Length) return experienceGain[cost];
        return 0;
    }

    public string GetCardDescription()
    {
        string description = "";

        foreach (var action in actions)
        {
            switch (action.actionType)
            {
                case CardActionType.Damage:
                    description += $"적에게 {action.value} 데미지를 줍니다. ";
                    break;
                case CardActionType.MultiHit:
                    description += $"적에게 {action.value} 데미지로 {action.secondaryValue}번 데미지를 줍니다. ";
                    break;
                case CardActionType.AreaDamage:
                    description += $"적 전체에게 {action.value} 데미지로 {action.secondaryValue}번 데미지를 줍니다. ";
                    break;
                case CardActionType.RandomTargetDamage:
                    description += $"랜덤한 적에게 {action.value} 데미지로 {action.secondaryValue}번 데미지를 줍니다. ";
                    break;
                case CardActionType.RandomTargetDamageWithBonus:
                    description += $"랜덤한 적에게 {action.value} 데미지로 {action.secondaryValue}번 데미지를 주며, 동일한 대상에게 {action.thirdValue}번 타격 시 1회 추가 타격합니다. ";
                    break;
                case CardActionType.IncrementalDamage:
                    description += $"적에게 타격할 때마다 1 데미지가 증가하는 공격을 {action.value} 데미지로 {action.secondaryValue}번 데미지를 줍니다. ";
                    break;
                case CardActionType.TrueDamage:
                    description += $"적에게 {action.value}의 고정 데미지를 {action.secondaryValue}번 줍니다. ";
                    break;
                case CardActionType.TrueAreaDamage:
                    description += $"적 전체에게 {action.value}의 고정 데미지를 {action.secondaryValue}번 줍니다. ";
                    break;
                case CardActionType.ShieldAttack:
                    description += $"자신의 방어도만큼 적에게 데미지를 줍니다. ";
                    break;
                case CardActionType.OverhealToDamage:
                    description += $"자신의 체력을 {action.value}만큼 회복하며, 초과된 회복량만큼 데미지를 줍니다. ";
                    break;
                case CardActionType.StunCheckDamage:
                    description += $"적에게 {action.value} 데미지를 주고, 적이 스턴 상태라면 추가로 {action.secondaryValue} 데미지를 줍니다. ";
                    break;
                case CardActionType.Poison:
                    description += $"적에게 {action.value}의 독을 {action.secondaryValue}번 부여합니다. ";
                    break;
                case CardActionType.AreaPoison:
                    description += $"적 전체에게 {action.value}의 독을 {action.secondaryValue}번 부여합니다. ";
                    break;
                case CardActionType.DoublePoison:
                    description += $"적에게 부여된 독이 2배가 됩니다. ";
                    break;
                case CardActionType.PoisonToDamage:
                    description += $"적에게 부여된 독만큼 데미지를 주고, 독을 제거합니다. ";
                    break;
                case CardActionType.RandomTargetPoison:
                    description += $"랜덤한 적에게 {action.value}의 독을 {action.secondaryValue}번 부여합니다. ";
                    break;
                case CardActionType.PoisonCheckDamage:
                    description += $"적에게 {action.value} 데미지를 주고, 적이 중독 상태라면 추가로 {action.secondaryValue} 데미지를 줍니다. ";
                    break;
                case CardActionType.killEffect:
                    switch (action.killEffectType)
                    {
                        case CardActionType.Damage:
                            description += $"처치 시 추가로 랜덤한 적에게 {action.thirdValue} 데미지를 줍니다. ";
                            break;
                        case CardActionType.Heal:
                            description += $"처치 시 추가로 자신의 체력을 {action.thirdValue}만큼 회복합니다. ";
                            break;
                        case CardActionType.Shield:
                            description += $"처치 시 추가로 자신의 방어도를 {action.thirdValue}만큼 부여합니다. ";
                            break;
                        case CardActionType.RestoreResource:
                            description += $"처치 시 추가로 자신의 서클을 {action.thirdValue}만큼 회복합니다. ";
                            break;
                    }
                    break;
                case CardActionType.Heal:
                    description += $"자신의 체력을 {action.value}만큼 회복합니다. ";
                    break;
                case CardActionType.Shield:
                    description += $"자신의 방어도를 {action.value}만큼 부여합니다. ";
                    break;
                case CardActionType.DoubleShield:
                    description += $"자신에게 부여된 방어도가 2배가 됩니다. ";
                    break;
            }
        }

        foreach (var buffDebuff in buffsAndDebuffs)
        {
            switch (buffDebuff.effectType)
            {
                case EffectType.DecreaseDamage:
                    description += $"적에게 {buffDebuff.duration}턴 동안 {buffDebuff.effectValue * 100}% 데미지 감소 디버프를 적용합니다. ";
                    break;
                case EffectType.SkipTurn:
                    description += $"적에게 {buffDebuff.duration}턴 동안 행동 불가 디버프를 적용합니다. ";
                    break;
                case EffectType.RandomAction:
                    description += $"적이 이번 턴에 수행할 행동을 변경합니다. ";
                    break;
                case EffectType.Confuse:
                    description += $"적에게 {buffDebuff.duration}턴 동안 혼란 디버프를 적용합니다. ";
                    break;
                case EffectType.DelayedImpact:
                    description += $"적에게 한 턴 뒤에 {buffDebuff.intValue} 데미지를 줍니다. ";
                    break;
                case EffectType.IncreaseDamage:
                    description += $"자신에게 {buffDebuff.duration}턴 동안 {buffDebuff.effectValue * 100}% 데미지 증가 버프를 적용합니다. ";
                    break;
                case EffectType.AreaEffect:
                    description += $"자신에게 {buffDebuff.duration}턴 동안 광역 공격 버프를 적용합니다. ";
                    break;
                case EffectType.LifeSteal:
                    description += $"자신에게 {buffDebuff.duration}턴 동안 {buffDebuff.effectValue * 100}% 흡혈 버프를 적용합니다. ";
                    break;
                case EffectType.ReduceDamage:
                    description += $"자신에게 {buffDebuff.duration}턴 동안 {buffDebuff.effectValue * 100}% 피해 감소 버프를 적용합니다. ";
                    break;
                case EffectType.ReflectDamage:
                    description += $"자신에게 {buffDebuff.duration}턴 동안 {buffDebuff.effectValue * 100}% 반사 버프를 적용합니다. ";
                    break;
                case EffectType.Purification:
                    description += $"자신에게 적용된 해로운 효과들을 정화합니다. ";
                    break;
                case EffectType.Field:
                    switch (attributeType)
                    {
                        case PlayerState.AttributeType.Fire:
                            description += $"필드를 불 속성으로 바꿉니다. 불 속성 마법의 데미지가 25% 증가합니다. ";
                            break;
                        case PlayerState.AttributeType.Water:
                            description += $"필드를 물 속성으로 바꿉니다. 매 턴마다 자신의 체력을 8만큼 회복합니다.";
                            break;
                        case PlayerState.AttributeType.Wood:
                            description += $"필드를 나무 속성으로 바꿉니다. 중독된 대상에게 매 턴마다 3의 독을 부여합니다. ";
                            break;
                        case PlayerState.AttributeType.Earth:
                            description += $"필드를 땅 속성으로 바꿉니다. 자신이 받는 데미지가 25% 감소됩니다. ";
                            break;
                        case PlayerState.AttributeType.Lightning:
                            description += $"필드를 전기 속성으로 바꿉니다. 매 턴 종료 시 스턴 상태의 적에게 20 데미지를 줍니다. ";
                            break;
                        case PlayerState.AttributeType.Wind:
                            description += $"필드를 바람 속성으로 바꿉니다. 바람 마법의 고정 데미지가 1 추가됩니다.";
                            break;
                        case PlayerState.AttributeType.Light:
                            description += $"필드를 빛 속성으로 바꿉니다. 빛 마법의 소모 서클이 2 감소합니다. ";
                            break;
                        case PlayerState.AttributeType.Dark:
                            description += $"필드를 어둠 속성으로 바꿉니다. 어둠 마법의 소모 서클이 2 감소합니다. ";
                            break;
                    }
                    break;
            }
        }
        return description;
    }

    private bool IsDamageAction(CardAction action)
    {
        return action.actionType == CardActionType.Damage ||
               action.actionType == CardActionType.MultiHit ||
               action.actionType == CardActionType.AreaDamage ||
               action.actionType == CardActionType.RandomTargetDamage ||
               action.actionType == CardActionType.RandomTargetDamageWithBonus ||
               action.actionType == CardActionType.IncrementalDamage ||
               action.actionType == CardActionType.TrueDamage ||
               action.actionType == CardActionType.TrueAreaDamage ||
               action.actionType == CardActionType.ShieldAttack ||
               action.actionType == CardActionType.OverhealToDamage ||
               action.actionType == CardActionType.StunCheckDamage ||
               action.actionType == CardActionType.Poison ||
               action.actionType == CardActionType.AreaPoison ||
               action.actionType == CardActionType.DoublePoison ||
               action.actionType == CardActionType.PoisonToDamage ||
               action.actionType == CardActionType.RandomTargetPoison ||
               action.actionType == CardActionType.PoisonCheckDamage;
    }

    private bool IsPlayerEffect(CardAction action)
    {
        return action.actionType == CardActionType.Heal ||
               action.actionType == CardActionType.Shield ||
               action.actionType == CardActionType.DoubleShield ||
               action.actionType == CardActionType.RestoreResource;
    }

    private bool IsMonsterDebuff(BuffDebuff effect)
    {
        return  effect.effectType == EffectType.DecreaseDamage ||
                effect.effectType == EffectType.SkipTurn ||
                effect.effectType == EffectType.RandomAction ||
                effect.effectType == EffectType.Confuse ||
                effect.effectType == EffectType.DelayedImpact;
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
