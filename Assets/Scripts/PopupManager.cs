using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public GameObject popupPanelCardReward; // �˾� �г�
    public GameObject popupPanelPortal;     // ��Ż �г�
    public GameObject popupPanelEvent;      // �̺�Ʈ �г�
    public GameObject popupPanelArtifactMount;  // ��Ƽ��Ʈ ���� �г�
    public GameObject popupPanelShop;       // ����
    public GameObject popupPanelAttributeLV;

    //public GameObject popupPanelUIBar;

    public Transform contentArea; // �˾� �� ������Ʈ�� ������ ��ġ

    public ArtifactMountManager artifactMountManager;

    void Start()
    {
        DeactivePanels(); // ������ �� �˾� ��Ȱ��ȭ
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

            case "AttributeLV":
                popupPanelAttributeLV.SetActive(true);
                break;

            default:
                Debug.Log("�� �� ���� �޽����� �޾ҽ��ϴ�: " + message);
                break;
        }

        //popupPanelUIBar.SetActive(false);
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

            case "AttributeLV":
                popupPanelAttributeLV.SetActive(false);
                break;

            default:
                Debug.Log("�� �� ���� �޽����� �޾ҽ��ϴ�: " + message);
                break;
        }

        //popupPanelUIBar.SetActive(true);
    }

    void ClearPopupContents()
    {
        // �˾��� ���빰�� ����
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
        popupPanelAttributeLV.SetActive(false);
    }
}

