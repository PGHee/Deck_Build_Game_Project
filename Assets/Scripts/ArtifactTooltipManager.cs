using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ArtifactTooltipManager : MonoBehaviour
{
    public GameObject tooltipPanel;             // 툴팁 패널
    public TextMeshProUGUI tooltipText;         // 툴팁에 표시될 텍스트
    private RectTransform tooltipRectTransform;

    private void Awake()
    {
        tooltipRectTransform = tooltipPanel.GetComponent<RectTransform>();
        HideTooltip();  // 시작할 때 툴팁을 숨깁니다.
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePosition = Input.mousePosition;
            tooltipRectTransform.position = mousePosition + new Vector2(0f, 0f);
        }
    }

    public void ShowTooltip(string text)
    {
        tooltipText.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipText.rectTransform);

        // 텍스트의 높이에 따라 툴팁 패널의 높이를 조정합니다.
        float preferredHeight = tooltipText.preferredHeight;
        Vector2 newSize = new Vector2(tooltipRectTransform.sizeDelta.x, preferredHeight + 20f); // 20f는 패딩
        tooltipRectTransform.sizeDelta = newSize;

        // 텍스트 RectTransform을 패널 크기에 맞춰 조정
        tooltipText.rectTransform.sizeDelta = new Vector2(tooltipRectTransform.sizeDelta.x - 20f, preferredHeight); // 20f는 패딩

        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public string GetBuffDescription(EffectType effectType, int duration, float effectValue, int intValue)
    {
        string description = "";

        switch (effectType)
        {
            case EffectType.IncreaseDamage:
                description = $"데미지 증가: {effectValue * 100}%, {duration}턴";
                break;
            case EffectType.AreaEffect:
                description = $"범위 공격: {duration}턴";
                break;
            case EffectType.LifeSteal:
                description = $"흡혈: {effectValue * 100}%, {duration}턴";
                break;
            case EffectType.ReduceDamage:
                description = $"받는 피해 감소: {effectValue * 100}%, {duration}턴";
                break;
            case EffectType.ReflectDamage:
                description = $"데미지 반사: {effectValue * 100}%, {duration}턴";
                break;
            case EffectType.DecreaseDamage:
                description = $"데미지 감소: {effectValue * 100}%, {duration}턴";
                break;
            case EffectType.SkipTurn:
                description = $"스턴: {duration}턴";
                break;
            case EffectType.Confuse:
                description = $"혼란: {duration}턴";
                break;
            case EffectType.DelayedImpact:
                description = $"한 턴 뒤 데미지: {intValue}";
                break;
        }

        return description;
    }
}
