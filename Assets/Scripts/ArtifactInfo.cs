using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactInfo : MonoBehaviour
{
    public PlayerState playerstate;

    public ArtifactManager.ActiveEffect activeEffectInfo;
    public int activeCoefInfo;
    public List<EffectType> passiveListInfo;
    public List<float> passiveCoefListInfo;
    public GameObject artifactManager;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        artifactManager = GameObject.Find("ArtifactManager");
        artifactManager.GetComponent<ArtifactManager>().ActiveEffectApply();
    }
}
