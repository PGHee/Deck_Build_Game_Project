using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpPanelManager : MonoBehaviour
{
    public GameObject helpPanel;            // 전체 도움말 패널
    public List<GameObject> parts;          // 설명할 오브젝트 리스트 (A, B, C 등)
    public List<GameObject> partPanels;     // 각 파트에 해당하는 패널 리스트 (A, B, C 등)
    public TextMeshProUGUI pageNumberText;  // 페이지 번호를 표시할 TextMeshPro 객체
    public GameObject helpButton;           // 도움말 패널을 여는 버튼
    public int currentPartIndex = 0;        // 현재 활성화된 파트 인덱스

    private RectTransform helpButtonRect;   // 버튼의 RectTransform 저장

    void Start()
    {
        // 처음엔 패널과 모든 파트 및 파트 패널을 비활성화
        helpPanel.SetActive(false);
        SetAllPartsActive(false);
        SetAllPartPanelsActive(false);
        UpdatePageNumberText();  // 페이지 번호 초기화

        // 페이지 번호 텍스트가 클릭을 가로막지 않도록 설정
        pageNumberText.raycastTarget = false;

        // 버튼의 RectTransform 가져오기
        helpButtonRect = helpButton.GetComponent<RectTransform>();
    }

    // 도움말 패널을 활성화하는 함수
    public void ShowHelpPanel()
    {
        helpPanel.SetActive(true);         // 도움말 패널 활성화
        helpButtonRect.anchoredPosition += new Vector2(50f, 0);
        ShowCurrentPart();
    }

    // 도움말 패널을 비활성화하는 함수
    public void HideHelpPanel()
    {
        helpPanel.SetActive(false);        // 도움말 패널 비활성화
        helpButtonRect.anchoredPosition -= new Vector2(50f, 0);
        SetAllPartsActive(false);
        SetAllPartPanelsActive(false);
    }

    // 이전 버튼 클릭 시 호출될 함수
    public void OnPreviousPart()
    {
        if (currentPartIndex > 0)
        {
            currentPartIndex--;
            ShowCurrentPart();
        }
    }

    // 다음 버튼 클릭 시 호출될 함수
    public void OnNextPart()
    {
        if (currentPartIndex < parts.Count - 1)
        {
            currentPartIndex++;
            ShowCurrentPart();
        }
    }

    // 현재 파트를 활성화하고 설명 패널을 업데이트하는 함수
    private void ShowCurrentPart()
    {
        SetAllPartsActive(false);
        SetAllPartPanelsActive(false);

        // 현재 파트와 설명 패널 활성화
        parts[currentPartIndex].SetActive(true);
        partPanels[currentPartIndex].SetActive(true);
        UpdatePageNumberText(); // 페이지 번호 업데이트
    }

    // 모든 파트를 비활성화하는 함수
    private void SetAllPartsActive(bool active)
    {
        foreach (GameObject part in parts)
        {
            part.SetActive(active);
        }
    }

    // 모든 파트 패널을 비활성화하는 함수
    private void SetAllPartPanelsActive(bool active)
    {
        foreach (GameObject panel in partPanels)
        {
            panel.SetActive(active);
        }
    }

    // 페이지 번호를 업데이트하는 함수
    private void UpdatePageNumberText()
    {
        pageNumberText.text = (currentPartIndex + 1).ToString() + " / " + partPanels.Count.ToString();
    }
}
