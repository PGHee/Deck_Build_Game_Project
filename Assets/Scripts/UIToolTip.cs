using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EffectType effectType;
    private TooltipManager tooltipManager;
    private BuffDebuffManager buffDebuffManager;
    private GameObject entity;
    public bool isArtifact;
    public string descriptionText;
    public string descriptionTextArtifact;
    public bool mouseOn;
    public bool openTooltip;

    private void Start()
    {
        tooltipManager = FindObjectOfType<TooltipManager>();
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
        entity = GameObject.Find("Artifact");
        openTooltip = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = "";

        description += descriptionText;

        if (isArtifact)
        {
            description += descriptionTextArtifact;
        }

        if (openTooltip)
        {
            tooltipManager.ShowTooltip(description);
        }
        mouseOn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
        mouseOn = false;
    }
}
