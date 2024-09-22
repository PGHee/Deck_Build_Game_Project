using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInfo : MonoBehaviour
{
    public string portalName;
    public GameObject gameDirector;
    private PopupManager popupManager;
    // Start is called before the first frame update
    void Start()
    {
        popupManager = FindObjectOfType<PopupManager>();
    }

    void OnMouseDown()
    {
        if (!popupManager.IsPanelOpen())
        {
            gameDirector = GameObject.Find("GameDirector");
            gameDirector.GetComponent<GameDirector>().OnPortalEntered(portalName);
            GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
            gameDirector.GetComponent<GameDirector>().currentMapName = portalName;
            foreach (GameObject portal in portals)
            {
                Destroy(portal);
            }
        }    
    }
}
