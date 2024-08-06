using UnityEngine;
using UnityEngine.EventSystems;

public class BuffDebuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EffectType effectType;
    private TooltipManager tooltipManager;
    private BuffDebuffManager buffDebuffManager;
    private GameObject entity;

    private void Start()
    {
        tooltipManager = FindObjectOfType<TooltipManager>();
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
        entity = transform.parent.parent.parent.parent.gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = "";
        description += "[ 버프 ]\n";
        if (buffDebuffManager.entityBuffs.TryGetValue(entity, out var buffs))
        {
            foreach (var buff in buffs)
            {
                description += tooltipManager.GetBuffDescription(buff.Item1, buff.Item2, buff.Item3, buff.Item4) + "\n";
            }
        }
        
        description += "\n[ 디버프 ]\n";
        if (buffDebuffManager.entityDebuffs.TryGetValue(entity, out var debuffs))
        {
            foreach (var debuff in debuffs)
            {
                description += tooltipManager.GetBuffDescription(debuff.Item1, debuff.Item2, debuff.Item3, debuff.Item4) + "\n";
            }
        }
        tooltipManager.ShowTooltip(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
    }
}
