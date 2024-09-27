using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerState playerState;
    public TextMeshProUGUI levelText;           // 레벨을 표시할 텍스트
    public TextMeshProUGUI experienceText;      // 경험치를 표시할 텍스트
    public TextMeshProUGUI crystalText;         // 화폐를 표시할 텍스트
    public Image experienceFill;                // 경험치 바의 Fill을 조절할 이미지

    public CrystalExchangePanel crystalExchangePanel;
    private SystemMessage message;

    public float fadedAlpha = 0.5f;             // 흐릿할 때의 알파 값
    public float normalAlpha = 1f;              // 선명할 때의 알파 값

    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
        SetUIAlpha(fadedAlpha);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetUIAlpha(normalAlpha);  // UI를 선명하게
    }

    // 마우스가 UI에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        SetUIAlpha(fadedAlpha);  // UI를 흐릿하게
    }

    // 현재 오브젝트와 모든 자식 오브젝트의 알파 값을 변경하는 함수
    private void SetUIAlpha(float alpha)
    {
        // 부모 오브젝트의 모든 자식 오브젝트를 탐색
        foreach (Image img in GetComponentsInChildren<Image>(true))
        {
            Color imgColor = img.color;
            imgColor.a = alpha;
            img.color = imgColor;
        }

        foreach (TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            Color textColor = text.color;
            textColor.a = alpha;
            text.color = textColor;
        }
    }
}
