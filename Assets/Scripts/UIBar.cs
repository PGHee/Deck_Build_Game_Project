using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBar : MonoBehaviour
{
    private PlayerState playerState;
    public TextMeshProUGUI levelText;           // 레벨을 표시할 텍스트
    public TextMeshProUGUI experienceText;      // 경험치를 표시할 텍스트
    public Image experienceFill;                // 경험치 바의 Fill을 조절할 이미지

    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
    }

    private void Update()
    {
        UpdateLevelText();
        UpdateExperienceBar();
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
}
