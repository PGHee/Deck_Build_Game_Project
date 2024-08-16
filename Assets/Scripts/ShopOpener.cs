using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopOpener : MonoBehaviour
{
    public GameObject shopManager;
    public GameObject popupManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("shop open!!!");
        popupManager = GameObject.Find("PopupManager");
        popupManager.GetComponent<PopupManager>().ShowPopup("Shop");

        shopManager = GameObject.Find("ShopManager");
        shopManager.GetComponent<ShopManager>().SetCardsToShop();
    }
}
