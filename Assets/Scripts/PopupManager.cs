using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public GameObject popupPanelBattleReward; // �˾� �г�
    public GameObject popupPanelPortal;     // ��Ż �г�
    public GameObject popupPanelEvent;      // �̺�Ʈ �г�
    public GameObject popupPanelArtifactMount;  // ��Ƽ��Ʈ ���� �г�
    public GameObject popupPanelArtifactSynthesis;  // ��Ƽ��Ʈ ���� �г�
    public GameObject popupPanelShop;       // ����
    public GameObject popupPanelAttributeLV;
    public GameObject popupPanelVillageChief;

    //public GameObject popupPanelUIBar;

    public Transform contentArea; // �˾� �� ������Ʈ�� ������ ��ġ

    public ArtifactMountManager artifactMountManager;
    public GameDirector gameDirector;
    public AttributeLevelBarManager attributeLevelBarManager;

    void Start()
    {
        DeactivePanels(); // ������ �� �˾� ��Ȱ��ȭ

        gameDirector = FindObjectOfType<GameDirector>();
        attributeLevelBarManager = FindObjectOfType<AttributeLevelBarManager>();
    }

    public void ShowPopup(string message)
    {

        switch (message)
        {
            case "BattleReward":
                popupPanelBattleReward.SetActive(true);
                break;

            case "Portal":
                popupPanelPortal.SetActive(true);
                break;

            case "Event":
                popupPanelEvent.SetActive(true);
                break;

            case "ArtifactMount":
                if (!(gameDirector.currentMapName.Contains("Battle")))
                {
                    popupPanelArtifactMount.SetActive(true);
                    artifactMountManager.UnmountArtifact();
                }
                else
                {
                    popupPanelAttributeLV.SetActive(true);

                    attributeLevelBarManager.UpdateAttributeLevelImage();
                }
                break;

            case "ArtifactSynthesis":
                popupPanelArtifactSynthesis.SetActive(true);
                artifactMountManager.UnmountArtifact();
                break;

            case "Shop":
                popupPanelShop.SetActive(true);
                break;

            case "AttributeLV":
                popupPanelAttributeLV.SetActive(true);
                break;

            case "VillageChief":
                popupPanelVillageChief.SetActive(true);
                break;


            default:
                Debug.Log("wrong message: " + message);
                break;
        }

        //popupPanelUIBar.SetActive(false);
    }

    public void ClosePopup(string message)
    {

        switch (message)
        {
            case "BattleReward":
                popupPanelBattleReward.SetActive(false);
                break;

            case "Portal":
                popupPanelPortal.SetActive(false);
                break;

            case "Event":
                popupPanelEvent.SetActive(false);
                break;

            case "ArtifactMount":
                popupPanelArtifactMount.SetActive(false);
                artifactMountManager.MountedArtifactBuffApply();
                break;

            case "ArtifactSynthesis":
                popupPanelArtifactSynthesis.SetActive(false);
                break;

            case "Shop":
                popupPanelShop.SetActive(false);
                break;

            case "AttributeLV":
                popupPanelAttributeLV.SetActive(false);
                break;

            case "VillageChief":
                popupPanelVillageChief.SetActive(false);
                break;

            default:
                Debug.Log("wrong map message: " + message);
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
        popupPanelBattleReward.SetActive(false);
        popupPanelPortal.SetActive(false);
        popupPanelEvent.SetActive(false);
        popupPanelArtifactMount.SetActive(false);
        popupPanelArtifactSynthesis.SetActive(false);
        popupPanelShop.SetActive(false);
        popupPanelAttributeLV.SetActive(false);
        popupPanelVillageChief.SetActive(false);
    }
}

