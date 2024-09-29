using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopOpener : MonoBehaviour
{
    public GameObject shopManager;
    public GameObject popupManager;

    void OnMouseDown()
    {
        Debug.Log("shop open!!!");
        popupManager = GameObject.Find("PopupManager");
        popupManager.GetComponent<PopupManager>().ShowPopup("Shop");

        shopManager = GameObject.Find("ShopManager");
        shopManager.GetComponent<ShopManager>().RestartShop();
        //shopManager.GetComponent<ShopManager>().SetCardsToShop();
        //shopManager.GetComponent<ShopManager>().SetArtifactsToShop();

    }
}
