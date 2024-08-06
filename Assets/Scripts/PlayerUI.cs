using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    private PlayerState playerState;
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
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerUI();
    }

    private void UpdatePlayerUI()
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
