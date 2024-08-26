using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnActionUI : MonoBehaviour
{
    public SpriteRenderer monsterActionUI;
    public TextMeshProUGUI monsterActionText;
    public Sprite[] monsterActionSprite;

    // 심장 박동 효과의 지속 시간과 크기 변화 속도
    public float pulseDuration = 0.5f; // 한 번의 펄스 지속 시간
    public float pulseScale = 1.2f; // 펄스 시 커지는 크기 배율
    public int pulseCount = 2; // 펄스 반복 횟수

    public void Initialize(Transform target, ActionType monsterAction)
    {
        transform.SetParent(target);
        transform.localPosition = new Vector3(0, 3.0f, 0);
    }
    
    public void UpdateAction(ActionType monsterAction, int intEffect, float floatEffect)
    {
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
}
