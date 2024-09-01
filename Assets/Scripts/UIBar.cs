using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBar : MonoBehaviour
{
    private PlayerState playerState;
    public TextMeshProUGUI levelText;           // 레벨을 표시할 텍스트
    public TextMeshProUGUI experienceText;      // 경험치를 표시할 텍스트
    public TextMeshProUGUI crystalText;         // 화폐를 표시할 텍스트
    public Image experienceFill;                // 경험치 바의 Fill을 조절할 이미지

    public CrystalExchangePanel crystalExchangePanel;
    private SystemMessage message;

    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
    }

    private void Update()
    {
        UpdateLevelText();
        UpdateExperienceBar();
        UpdateCrystalText();
    }

    private void UpdateLevelText()
    {
        levelText.text = "Lv. " + playerState.level;
    }

    private void UpdateExperienceBar()
    {
        int currentLevel = playerState.level;
        int currentExperience = playerState.experience;
        int requiredExperience = playerState.level < playerState.maxLevel ? playerState.ExperienceToNextLevel() : 1;

        experienceText.text = currentExperience + " / " + requiredExperience;
        experienceFill.fillAmount = (float)currentExperience / requiredExperience;
    }

    private void UpdateCrystalText()
    {
        crystalText.text = playerState.crystal+"";
    }

    public void CrystalToCircle()
    {
        if(!TurnManager.instance.IsPlayerTurn) message.ShowSystemMessage("플레이어 턴이 아닙니다.");
        else if(TurnManager.instance.IsPlayerTurn) crystalExchangePanel.OnOpenButtonClicked();
    }
}
