using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ArtifactTooltipManager : MonoBehaviour
{
    public GameObject tooltipPanel;             // ���� �г�
    public TextMeshProUGUI tooltipText;         // ������ ǥ�õ� �ؽ�Ʈ
    private RectTransform tooltipRectTransform;

    private void Awake()
    {
        tooltipRectTransform = tooltipPanel.GetComponent<RectTransform>();
        HideTooltip();  // ������ �� ������ ����ϴ�.
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

        // �ؽ�Ʈ�� ���̿� ���� ���� �г��� ���̸� �����մϴ�.
        float preferredHeight = tooltipText.preferredHeight;
        Vector2 newSize = new Vector2(tooltipRectTransform.sizeDelta.x, preferredHeight + 20f); // 20f�� �е�
        tooltipRectTransform.sizeDelta = newSize;

        // �ؽ�Ʈ RectTransform�� �г� ũ�⿡ ���� ����
        tooltipText.rectTransform.sizeDelta = new Vector2(tooltipRectTransform.sizeDelta.x - 20f, preferredHeight); // 20f�� �е�

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
                description = $"������ ����: {effectValue * 100}%, {duration}��";
                break;
            case EffectType.AreaEffect:
                description = $"���� ����: {duration}��";
                break;
            case EffectType.LifeSteal:
                description = $"����: {effectValue * 100}%, {duration}��";
                break;
            case EffectType.ReduceDamage:
                description = $"�޴� ���� ����: {effectValue * 100}%, {duration}��";
                break;
            case EffectType.ReflectDamage:
                description = $"������ �ݻ�: {effectValue * 100}%, {duration}��";
                break;
            case EffectType.DecreaseDamage:
                description = $"������ ����: {effectValue * 100}%, {duration}��";
                break;
            case EffectType.SkipTurn:
                description = $"����: {duration}��";
                break;
            case EffectType.Confuse:
                description = $"ȥ��: {duration}��";
                break;
            case EffectType.DelayedImpact:
                description = $"�� �� �� ������: {intValue}";
                break;
        }

        return description;
    }
}
