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
    public int artifactCost;
    public int bonusPoison;
    public float bonusAttack;
    public float bonusCrystal;
    public float bonusAttributeExperience;



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
