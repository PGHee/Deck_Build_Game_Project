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
    public int bonusDamage;
    public float bonusAttack;
    public float bonusCrystal;
    public float bonusAttributeExperience;
    public bool execution;
    public bool doubleLifeSteal;
    public int bonusShield;
    public int bonusHeal;
    public int bonusDraw;



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
