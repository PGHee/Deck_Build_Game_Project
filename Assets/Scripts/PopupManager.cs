using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public GameObject popupPanelCardReward; // 팝업 패널
    public GameObject popupPanelPortal;     // 포탈 패널
    public GameObject popupPanelEvent;      // 이벤트 패널
    public GameObject popupPanelArtifactMount;  // 아티팩트 장착 패널
    public GameObject popupPanelShop;       // 상점

    public GameObject popupPanelUIBar;

    public Transform contentArea; // 팝업 내 오브젝트가 생성될 위치

    public ArtifactMountManager artifactMountManager;

    void Start()
    {
        DeactivePanels(); // 시작할 때 팝업 비활성화
    }

    void Update()
    {

    }

    public void ShowPopup(string message)
    {

        switch (message)
        {
            case "CardReward":
                popupPanelCardReward.SetActive(true);
                break;

            case "Portal":
                popupPanelPortal.SetActive(true);
                break;

            case "Event":
                popupPanelEvent.SetActive(true);
                break;

            case "ArtifactMount":
                popupPanelArtifactMount.SetActive(true);
                break;

            case "Shop":
                popupPanelShop.SetActive(true);
                break;

            default:
                Debug.Log("알 수 없는 메시지를 받았습니다: " + message);
                break;
        }

        popupPanelUIBar.SetActive(false);
    }

    public void ClosePopup(string message)
    {

        switch (message)
        {
            case "CardReward":
                popupPanelCardReward.SetActive(false);
                break;

            case "Portal":
                popupPanelPortal.SetActive(false);
                break;

            case "Event":
                popupPanelEvent.SetActive(false);
                break;

            case "ArtifactMount":
                popupPanelArtifactMount.SetActive(false);
                break;

            case "Shop":
                popupPanelShop.SetActive(false);
                break;

            default:
                Debug.Log("알 수 없는 메시지를 받았습니다: " + message);
                break;
        }

        popupPanelUIBar.SetActive(true);
    }

    void ClearPopupContents()
    {
        // 팝업의 내용물을 정리
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
    }

    public void DeactivePanels()
    {
        popupPanelCardReward.SetActive(false);
        popupPanelPortal.SetActive(false);
        popupPanelEvent.SetActive(false);
        popupPanelArtifactMount.SetActive(false);
        popupPanelShop.SetActive(false);
    }
}

