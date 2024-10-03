using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TurnActionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SpriteRenderer monsterActionUI;
    public TextMeshProUGUI monsterActionText;
    public Sprite[] monsterActionSprite;

    // 심장 박동 효과의 지속 시간과 크기 변화 속도
    public float pulseDuration = 0.5f; // 한 번의 펄스 지속 시간
    public float pulseScale = 1.2f; // 펄스 시 커지는 크기 배율
    public int pulseCount = 2; // 펄스 반복 횟수

    private TooltipManager tooltipManager;
    private string description;

    private void Start()
    {
        tooltipManager = FindObjectOfType<TooltipManager>();
    }

    public void Initialize(Transform target, ActionType monsterAction)
    {
        transform.SetParent(target);
        transform.localPosition = new Vector3(0, 3.0f, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipManager.ShowTooltip(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
    }

    private void UpdateDescription(ActionType monsterAction, int intEffect, float floatEffect)
    {
        switch (monsterAction)
        {
            case ActionType.Damage:
                description = $"<size=15>이번 턴에 {intEffect} 데미지로 공격을 가합니다.</size>";
                break;
            case ActionType.StrongAttack:
                description = $"<size=15>이번 턴에 {intEffect} 데미지로 강력한 공격을 가합니다.</size>";
                break;
            case ActionType.LifeStealAttack:
                description = $"<size=15>이번 턴에 {intEffect} 데미지로 흡혈 공격을 가합니다.</size>";
                break;
            case ActionType.Shield:
                description = $"<size=15>이번 턴에 {intEffect}만큼 방어도를 쌓습니다.</size>";
                break;
            case ActionType.Heal:
                description = $"<size=15>이번 턴에 {intEffect}만큼 본인의 체력을 회복합니다.</size>";
                break;
            case ActionType.AreaHeal:
                description = $"<size=15>이번 턴에 {intEffect}만큼 몬스터 전체의 체력을 회복합니다.</size>";
                break;
            case ActionType.Poison:
                description = $"<size=15>이번 턴에 {intEffect}의 독을 부여합니다.</size>";
                break;
            case ActionType.SelfDestruct:
                description = $"<size=15>이번 턴에 자폭하여 {intEffect}의 데미지를 줍니다.</size>";
                break;
            case ActionType.IncreaseDamage:
                description = $"<size=15>이번 턴에 {floatEffect}% 데미지 증가 버프를 겁니다.</size>";
                break;
            case ActionType.IncreaseDamageStack:
                description = $"<size=15>이번 턴에 {floatEffect}% 데미지 영구 증가 버프를 겁니다.</size>";
                break;
            case ActionType.LifeSteal:
                description = $"<size=15>이번 턴에 {floatEffect}% 흡혈 버프를 겁니다.</size>";
                break;
            case ActionType.ReduceDamage:
                description = $"<size=15>이번 턴에 {floatEffect}% 데미지 감소 버프를 겁니다.</size>";
                break;
            case ActionType.ReflectDamage:
                description = $"<size=15>이번 턴에 {floatEffect}% 데미지 반사 버프를 겁니다.</size>";
                break;
            case ActionType.Wait:
                description = $"<size=15>이번 턴에 아무런 행동을 하지 않습니다.</size>";
                break;
            case ActionType.SkipTurn:
                description = $"<size=15>이번 턴에 경직 디버프를 겁니다.</size>";
                break;
            case ActionType.Confuse:
                description = $"<size=15>이번 턴에 혼란 디버프를 겁니다.</size>";
                break;
        }
    }
    
    public void UpdateAction(ActionType monsterAction, int intEffect, float floatEffect)
    {
        UpdateDescription(monsterAction, intEffect, floatEffect);
        monsterActionUI.color = new Color(1, 1, 1, 1);
        switch (monsterAction)
        {
            case ActionType.Damage:
            case ActionType.StrongAttack:
            case ActionType.LifeStealAttack:
            case ActionType.Shield:
            case ActionType.Heal:
            case ActionType.AreaHeal:
            case ActionType.Poison:
            case ActionType.SelfDestruct:
                if(intEffect != 0) monsterActionText.text = intEffect + "";
                else monsterActionText.text = "";
                monsterActionUI.sprite = monsterActionSprite[(int)monsterAction];
                break;
            case ActionType.IncreaseDamage:
            case ActionType.IncreaseDamageStack:
            case ActionType.LifeSteal:
            case ActionType.ReduceDamage:
            case ActionType.ReflectDamage:
                monsterActionText.text = floatEffect + "%";
                monsterActionUI.sprite = monsterActionSprite[(int)monsterAction];
                break;
            case ActionType.Wait:
            case ActionType.SkipTurn:
            case ActionType.Confuse:
                monsterActionText.text = "";
                monsterActionUI.sprite = monsterActionSprite[(int)monsterAction];
                break;
        }
    }

    // 심장 박동 효과를 연출하는 함수
    public void StartPulseEffect()
    {
        StartCoroutine(PulseEffectCoroutine());
    }

    private IEnumerator PulseEffectCoroutine()
    {
        Vector3 originalScale = monsterActionUI.transform.localScale;
        Vector3 targetScale = originalScale * pulseScale;
        float alphaStart = 1f; // 시작 투명도
        float alphaEnd = 0f; // 종료 투명도

        for (int i = 0; i < pulseCount; i++)
        {
            // 커지는 애니메이션
            float elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                float t = elapsedTime / pulseDuration;
                monsterActionUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                monsterActionUI.color = new Color(1, 1, 1, Mathf.Lerp(alphaStart, alphaEnd, t)); // 투명도 변화
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 작아지는 애니메이션
            elapsedTime = 0f;
            while (elapsedTime < pulseDuration)
            {
                float t = elapsedTime / pulseDuration;
                monsterActionUI.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                monsterActionUI.color = new Color(1, 1, 1, Mathf.Lerp(alphaEnd, alphaStart, t)); // 투명도 변화
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // 최종 투명도 조정 및 크기 초기화
        monsterActionUI.transform.localScale = originalScale;
        monsterActionUI.color = new Color(1, 1, 1, alphaEnd); // 최종 투명도 설정
    }

    public void DestroyActionUI()
    {
        Destroy(gameObject);
    }
}
