using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSynthsisOpener : MonoBehaviour
{
    public GameObject artifactSynthesisManager;
    public GameObject popupManager;

    void OnMouseDown()
    {
        Debug.Log("artifact synthesis open!!!");
        popupManager = GameObject.Find("PopupManager");
        popupManager.GetComponent<PopupManager>().ShowPopup("ArtifactSynthesis");

        artifactSynthesisManager = GameObject.Find("ArtifactSynthesisManager");
        artifactSynthesisManager.GetComponent<ArtifactSynthesisManager>().ArtifactListUp();
    }
}
