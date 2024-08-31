using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageChiefManager : MonoBehaviour
{
    public bool enterShop;
    public bool enterArtifactSynthesis;

    public GameObject shopButton;
    public GameObject artifactSynthesisButton;

    // Start is called before the first frame update
    void Start()
    {
        resetEnter();
    }

    public void resetEnter()
    {
        enterShop = true;
        enterArtifactSynthesis = true;
        shopButton.GetComponent<Button>().interactable = true;
        artifactSynthesisButton.GetComponent<Button>().interactable = true;
    }

    public void ShopEntered()
    {
        enterShop = false;
        shopButton.GetComponent<Button>().interactable = false;
    }

    public void ArtifactSynthesisEntered()
    {
        enterArtifactSynthesis= false;
        artifactSynthesisButton.GetComponent<Button>().interactable = false;
    }
}
