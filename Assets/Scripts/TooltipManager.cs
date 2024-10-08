using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public GameObject tooltipPanel;             // 툴팁 패널
    public TextMeshProUGUI tooltipText;         // 툴팁에 표시될 텍스트
    private RectTransform tooltipRectTransform;

    private void Awake()
    {
        tooltipRectTransform = tooltipPanel.GetComponent<RectTransform>();
        tooltipRectTransform.pivot = new Vector2(0.5f, 0f); // 패널 하단 위치를 고정, 길이가 늘어나면 위쪽으로 확장
        HideTooltip(); 
    }

    public void ShowTooltip(string text)
    {
        Vector2 mousePosition = Input.mousePosition;
        tooltipText.text = text;

        // 텍스트 레이아웃을 즉시 갱신하여 크기를 가져옴
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipText.rectTransform);

        // 텍스트의 높이에 따라 툴팁 패널의 높이를 조정
        float preferredHeight = tooltipText.preferredHeight;
        Vector2 newSize = new Vector2(tooltipRectTransform.sizeDelta.x, preferredHeight + 20f);
        tooltipRectTransform.sizeDelta = newSize;

        // 텍스트 RectTransform을 패널 크기에 맞춰 조정
        tooltipText.rectTransform.sizeDelta = new Vector2(tooltipRectTransform.sizeDelta.x - 20f, preferredHeight);

        // 툴팁이 화면 밖으로 나가는지 확인하여 위치를 조정
        tooltipRectTransform.position = mousePosition + new Vector2(150f, 10f);
        Vector3 tooltipPosition = tooltipRectTransform.position;

        // 화면 경계를 넘어가는지 확인 후 X축 좌표 보정
        float panelWidth = tooltipRectTransform.sizeDelta.x;
        if (tooltipPosition.x + panelWidth / 2 > Screen.width) // 오른쪽 경계를 넘는 경우
        {
            tooltipPosition.x = Screen.width - panelWidth / 2;
        }
        else if (tooltipPosition.x - panelWidth / 2 < 0) // 왼쪽 경계를 넘는 경우
        {
            tooltipPosition.x = panelWidth / 2;
        }

        // X축이 조정된 위치로 재배치
        tooltipRectTransform.position = new Vector3(tooltipPosition.x, tooltipPosition.y, tooltipPosition.z);

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
                if(duration >= 100) description = $"영구 데미지 증가: {effectValue*100}%";
                else description = $"데미지 증가: {effectValue*100}%, {duration}턴";
                break;
            case EffectType.AreaEffect:
                description = $"범위 공격: {duration}턴";
                break;
            case EffectType.LifeSteal:
                description = $"흡혈: {effectValue*100}%, {duration}턴";
                break;
            case EffectType.ReduceDamage:
                description = $"받는 피해 감소: {effectValue*100}%, {duration}턴";
                break;
            case EffectType.ReflectDamage:
                description = $"데미지 반사: {effectValue*100}%, {duration}턴";
                break;
            case EffectType.DecreaseDamage:
                description = $"데미지 감소: {effectValue*100}%, {duration}턴";
                break;
            case EffectType.SkipTurn:
                description = $"경직: {duration}턴";
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

    public string GetPassiveDescription(PassiveType passiveType)
    {
        string description = "";
        switch (passiveType)
        {
            case PassiveType.Revive:
                description = $"<size=15>부활 : 체력이 모두 소진되면 한 번 부활합니다.</size>";
                break;
            case PassiveType.Bond:
                description = $"<size=15>결속 : 결속된 몬스터를 잡지 못하면 부활합니다.</size>";
                break;
        }
        return description;
    }
}
