using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI 요소를 사용하기 위해 필요
using TMPro;  // TextMeshPro 요소를 사용하기 위해 필요

public class CrystalExchangePanel : MonoBehaviour
{
    public TextMeshProUGUI crystalCostText;         // 크리스탈 요구량 텍스트
    public TextMeshProUGUI recoverResourceText;     // 회복할 서클 양 텍스트
    public UIBar uiBar;

    private int targetRecoverAmount = 0;            // 회복하려는 서클 양
    private int crystalCostPerResource = 30;        // 서클 하나를 회복하는 데 필요한 크리스탈 양
    private PlayerState playerState;                // 플레이어 상태를 참조하기 위한 변수
    private SystemMessage message;                  // 시스템 메시지 출력을 위한 변수

    private void Start()
    {
        playerState = FindObjectOfType<PlayerState>();  // 플레이어 상태 찾기
        message = FindObjectOfType<SystemMessage>();    // 시스템 메시지 찾기
        UpdateUI();                                     // UI 초기화
    }

    private void UpdateUI()
    {
        int crystalCost = targetRecoverAmount * crystalCostPerResource;     // 현재 회복하려는 서클 양에 따른 크리스탈 요구량

        crystalCostText.text = crystalCost.ToString();                      // 크리스탈 요구량 텍스트 업데이트
        recoverResourceText.text = targetRecoverAmount.ToString();          // 회복할 서클 양 텍스트 업데이트

        // 크리스탈이 부족한 경우 빨간색 텍스트, 충분한 경우 흰색 텍스트 설정
        crystalCostText.color = playerState.crystal >= crystalCost ? Color.white : Color.red;
    }

    // 서클 증가 버튼을 눌렀을 때 호출되는 함수
    public void OnIncreaseButtonClicked()
    {
        int maxRecoverable = playerState.resource - playerState.currentResource;  // 회복 가능한 최대 서클 수

        if (targetRecoverAmount < maxRecoverable)
        {
            targetRecoverAmount++;
            UpdateUI();
        }
        else
        {
            message.ShowSystemMessage("최대 서클을 넘어 회복할 수 없습니다.");
        }
    }

    // 서클 감소 버튼을 눌렀을 때 호출되는 함수
    public void OnDecreaseButtonClicked()
    {
        if (targetRecoverAmount > 0)
        {
            targetRecoverAmount--;
            UpdateUI();
        }
    }

    // 교환 승인 버튼을 눌렀을 때 호출되는 함수
    public void OnConfirmButtonClicked()
    {
        if(!TurnManager.instance.enabled) message.ShowSystemMessage("전투 중인 경우에만 교환할 수 있습니다.");
        else if(TurnManager.instance.enabled && !TurnManager.instance.IsPlayerTurn) message.ShowSystemMessage("플레이어의 턴에만 교환할 수 있습니다.");
        else if(TurnManager.instance.enabled && TurnManager.instance.IsPlayerTurn)
        {
            int crystalCost = targetRecoverAmount * crystalCostPerResource;
            if (playerState.crystal >= crystalCost)
            {
                playerState.currentResource += targetRecoverAmount;     // 서클 회복
                playerState.crystal -= crystalCost;                     // 크리스탈 감소

                // UI 업데이트 및 교환 완료 후 초기화
                targetRecoverAmount = 0;
                UpdateUI();
                playerState.UpdateHPBar();
                uiBar.UpdateUIBar();
            }
            else message.ShowSystemMessage("플레이어의 마석이 부족합니다.");  // 크리스탈이 부족한 경우 메시지 출력
        }
        
    }

    // 교환 취소 버튼을 눌렀을 때 호출되는 함수
    public void OnCancelButtonClicked()
    {
        targetRecoverAmount = 0;  // 교환 양 초기화
        UpdateUI();  // UI 업데이트
    }

    public void OnOpenButtonClicked()
    {
        gameObject.SetActive(true);  // 패널 비활성화
    }

    // 패널 닫기 버튼을 눌렀을 때 호출되는 함수
    public void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);  // 패널 비활성화
    }
}
