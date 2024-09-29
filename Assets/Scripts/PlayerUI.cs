using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    private PlayerState playerState;
    public GameObject panel;                 // 플레이어 상태를 보여주는 패널
    public Button openPanelButton;           // 패널을 여는 버튼
    public Button closePanelButton;          // 패널을 닫는 버튼
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI damageMultiplierText;
    public TextMeshProUGUI LifeStealText;
    public TextMeshProUGUI reduceDamageText;
    public TextMeshProUGUI reflectDamageText;
    public TextMeshProUGUI fireDamageMultiplierText;
    public TextMeshProUGUI windHitBonusText;
    public TextMeshProUGUI woodPoisonBonusText;
    public TextMeshProUGUI lightningStunChanceText;
    public TextMeshProUGUI waterRegenText;
    public TextMeshProUGUI earthDefenseText;
    
    private string description;
    private TooltipManager tooltipManager;

    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
        tooltipManager = FindObjectOfType<TooltipManager>();
        openPanelButton.gameObject.SetActive(false);
        panel.SetActive(true);
        closePanelButton.gameObject.SetActive(true); 
    }

    // 패널 열기 함수
    public void OpenPlayerPanel()
    {
        panel.SetActive(true);
        UpdatePlayerUI();
        openPanelButton.gameObject.SetActive(false);
        closePanelButton.gameObject.SetActive(true);
    }

    // 패널 닫기 함수
    public void ClosePlayerPanel()
    {
        panel.SetActive(false);
        openPanelButton.gameObject.SetActive(true);
        closePanelButton.gameObject.SetActive(false);
    }

    // 플레이어 상태 UI를 갱신하는 함수
    public void UpdatePlayerUI()
    {
        int maxHP = playerState.maxHealth;
        int currentHP = playerState.currentHealth;

        HPText.text = currentHP + " / " + maxHP;
        damageMultiplierText.text = playerState.damageMultiplier * 100 + " %";
        LifeStealText.text = playerState.LifeSteal * 100 + " %";
        reduceDamageText.text = playerState.reduceDamage * 100 + " %";
        reflectDamageText.text = playerState.reflectDamage * 100 + " %";
        fireDamageMultiplierText.text = playerState.fireDamageMultiplier * 100 + " %";
        windHitBonusText.text = "+ " + playerState.windHitBonus;
        woodPoisonBonusText.text = "+ " + playerState.woodPoisonBonus;
        lightningStunChanceText.text = playerState.lightningStunChance * 100 + " %";
        waterRegenText.text = "+ " + playerState.waterRegen;
        earthDefenseText.text = "+ " + playerState.earthDefense;
    }

    public void ShowDescription(int number)
    {   
        description = "";
        switch (number)
        {
            case 1:
                description = $"체력\n<size=15>현재 체력과 최대 체력입니다. 현재 체력이 모두 소진되면 패배합니다.</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 2:
                description = $"데미지 배율\n<size=15>공격 시 표기된 수치만큼 증폭된 데미지를 가합니다. (기본값 100%)</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 3:
                description = $"체력 흡수\n<size=15>공격 시 적에게 준 데미지 중 표기된 수치만큼의 체력을 회복합니다. (기본값 0%)</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 4:
                description = $"데미지 감소\n<size=15>적에게 공격을 받으면 표기된 수치만큼 감소된 데미지를 받습니다. (기본값 0%)</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 5:
                description = $"데미지 반사\n<size=15>적에게 공격을 받으면 표기된 수치만큼 받은 데미지를 되돌려줍니다. (기본값 0%)</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 6:
                description = $"불 마법의 데미지 배율\n<size=15>불 마법으로 공격 시 표기된 수치만큼 증폭된 데미지를 가합니다.</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 7:
                description = $"전기 마법의 경직 확률\n<size=15>전기 마법으로 공격 시 표기된 확률에 따라 적에게 경직 디버프를 부여합니다.</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 8:
                description = $"나무 마법의 추가 독\n<size=15>나무 마법 중에 독을 부여하는 마법으로 공격 시 표기된 수치만큼 추가 독을 부여합니다.</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 9:
                description = $"바람 마법의 추가 타수\n<size=15>바람 마법으로 공격 시 표기된 수치만큼 추가 공격을 가합니다.</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 10:
                description = $"매 턴 회복되는 체력\n<size=15>매 턴마다 표기된 수치만큼 체력을 회복합니다.</size>";
                tooltipManager.ShowTooltip(description);
                break;
            case 11:
                description = $"매 턴 적립되는 방어\n<size=15>매 턴마다 표기된 수치만큼 방어가 적립됩니다.</size>";
                tooltipManager.ShowTooltip(description);
                break;
        }
    }

    public void HideDescription()
    {
        tooltipManager.HideTooltip();
    }
}
