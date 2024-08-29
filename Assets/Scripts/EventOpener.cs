using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOpener : MonoBehaviour
{
    public GameObject eventManager;
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

        eventManager = GameObject.Find("EventManager");
        eventManager.GetComponent<EventManager>().SetEvent();
    }
}
