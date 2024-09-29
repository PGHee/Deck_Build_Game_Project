using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EffectType effectType;
    private ArtifactTooltipManager tooltipManager;
    private BuffDebuffManager buffDebuffManager;
    private GameObject entity;
    public bool isArtifact;
    public string descriptionText;
    public string descriptionTextArtifact;
    public bool mouseOn;

    private void Start()
    {
        tooltipManager = FindObjectOfType<ArtifactTooltipManager>();
        buffDebuffManager = FindObjectOfType<BuffDebuffManager>();
        entity = GameObject.Find("Artifact");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = "";

        description += descriptionText;

        if (isArtifact)
        {
            description += descriptionTextArtifact;
        }

        tooltipManager.ShowTooltip(description);
        GameObject tooltip = GameObject.Find("BuffDebuffTooltip");
        RectTransform rectTransform = tooltip.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(0, 0, 0);

        mouseOn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
        mouseOn = false;
    }
}
