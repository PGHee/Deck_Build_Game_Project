using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInfo : MonoBehaviour
{
    public string portalName;
    public GameObject gameDirector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("portal spawned");

        gameDirector = GameObject.Find("GameDirector");
        gameDirector.GetComponent<GameDirector>().OnPortalEntered(portalName);
        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
        foreach(GameObject portal in portals)
        {
            Destroy(portal);
        }
    }
}
