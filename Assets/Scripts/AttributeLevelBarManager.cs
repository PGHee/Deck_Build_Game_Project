using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class AttributeLevelBarManager : MonoBehaviour
{

    public Image[] shapeImage; // 채워나갈 이미지
    public float[] fillAmount; // 채워질 비율 (0.0 ~ 1.0)
    public TextMeshProUGUI[] LevelText;
    public int[] experienceRequired;
    //public int[] attributeMastery;
    //public int[] attributeExperience;

    public PlayerState plyerState;

    // Start is called before the first frame update
    void Start()
    {
        experienceRequired = new int[] { 1, 2, 6, 12, 25, 56, 117, 252, 252};
    }

    public void UpdateAttributeLevelImage()
    {
        fillAmount = new float[10];
        List<int> attributeLVList = new List<int>(plyerState.attributeMastery.Values);
        List<int> attributeEXPList = new List<int>(plyerState.attributeExperience.Values);

        for (int i = 0; i < 10; i++)
        {
            fillAmount[i] = (float)((float)attributeEXPList[i] / (float)experienceRequired[attributeLVList[i] - 1]);
            if (fillAmount[i] == 0.0f)
            {
                fillAmount[i] = +0.05f;
            }

            if (attributeLVList[i] == 9) fillAmount[i] = 1.0f;

            shapeImage[i].fillAmount = fillAmount[i];
        }

        for (int i = 0; i < 10; i++)
        {
            if (attributeLVList[i] == 9)
            {
                LevelText[i].text = "[9] MAX";
            }
            else if (attributeLVList[i] == 10)
            {
                LevelText[i].text = "[10] SUPERIOR";
            }
            else
            {
                LevelText[i].text = "[" + attributeLVList[i] + "] " + attributeEXPList[i] + "/" + experienceRequired[attributeLVList[i] - 1] + "";
            }          
        }
    }

    public void UpdateAttributeLevel()
    {
        plyerState.AddAttributeExperience(PlayerState.AttributeType.Fire, 100);
    }
}

