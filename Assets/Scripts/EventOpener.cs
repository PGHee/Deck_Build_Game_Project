using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOpener : MonoBehaviour
{
    public GameObject shopManager;
    public GameObject popupManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("Event open!!!");
        popupManager = GameObject.Find("PopupManager");
        popupManager.GetComponent<PopupManager>().ShowPopup("Event");

        shopManager = GameObject.Find("EventManager");
    }
}
