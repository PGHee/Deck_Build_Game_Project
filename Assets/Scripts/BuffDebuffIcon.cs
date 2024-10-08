using UnityEngine;
using UnityEngine.EventSystems;

public class BuffDebuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EffectType effectType;
    private TooltipManager tooltipManager;
    private BuffDebuffManager buffDebuffManager;
    private GameObject entity;
    private MonsterState monsterState;

    private void Start()
    {
        tooltipManager = FindObjectOfType<TooltipManager>();
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
        entity = transform.parent.parent.parent.parent.gameObject;
        monsterState = transform.parent.parent.parent.parent.GetComponent<MonsterState>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = "";

        if (monsterState != null && monsterState.passives.Count > 0)
        {
            description += "[ 패시브 ]\n";
            foreach (Passive passive in monsterState.passives)
            {
                description += tooltipManager.GetPassiveDescription(passive.passiveType) + "\n";
            }
        }

        // 버프 출력
        if (buffDebuffManager.entityBuffs.TryGetValue(entity, out var buffs) && buffs.Count > 0)
        {
            description += "[ 버프 ]\n";
            foreach (var buff in buffs)
            {
                description += tooltipManager.GetBuffDescription(buff.Item1, buff.Item2, buff.Item3, buff.Item4) + "\n";
            }
        }

        // 디버프 출력
        if (buffDebuffManager.entityDebuffs.TryGetValue(entity, out var debuffs) && debuffs.Count > 0)
        {
            if (description.Length > 0) description += "\n"; // 버프가 있었으면 줄바꿈 추가
            description += "[ 디버프 ]\n";
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
