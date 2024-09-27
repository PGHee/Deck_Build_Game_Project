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

    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
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
}
