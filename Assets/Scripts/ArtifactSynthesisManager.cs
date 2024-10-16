using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSynthesisManager : MonoBehaviour
{
    public ArtifactManager artifactManager;
    private ArtifactMountManager artifactMountManager;
    private SystemMessage message;

    public int ArtifactListPage;
    public int[] artifactInvenList;
    public int pageArtifactNum;
    public GameObject[] clearButtonsArtifact;
    public GameObject[] artifactPrefabs;

    public int[] ArtifactsToSynthesis;

    // Start is called before the first frame update
    void Start()
    {
        artifactMountManager = FindObjectOfType<ArtifactMountManager>();
        message = FindObjectOfType<SystemMessage>();

        ArtifactListPage = 0;
        artifactPrefabs = new GameObject[30];
        for (int i = 0; i < 7; i++)
        {
            artifactPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Artifact/artifact_{i + 1}");
        }

        for (int j = 10; j < 17; j++)
        {
            artifactPrefabs[j] = Resources.Load<GameObject>($"Prefabs/Artifact/artifact_{j + 1}");
        }

        for (int k = 20; k < 27; k++)
        {
            artifactPrefabs[k] = Resources.Load<GameObject>($"Prefabs/Artifact/artifact_{k + 1}");
        }

        ArtifactsToSynthesis = new int[3] {999, 999, 999};

    }

    public void ArtifactListUp() 
    {
        SetAllButtonClear();

        GetClearButtonArtifact();

        artifactInvenList = artifactManager.artifactInvenList;

        if (artifactInvenList.Length > 15 * (ArtifactListPage + 1))
        {
            pageArtifactNum = 15;
        }
        else
        {
            pageArtifactNum = artifactInvenList.Length % 15;
            ArtifactListPage = (int)((artifactInvenList.Length - artifactInvenList.Length % 15) / 15);
        }

        for (int i = 0; i < pageArtifactNum; i++)
        {
            clearButtonsArtifact[i].GetComponent<Image>().sprite = artifactPrefabs[artifactInvenList[15 * ArtifactListPage + i] - 1].GetComponent<SpriteRenderer>().sprite;
            
            Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
            color.a = 255.0f;

            
            clearButtonsArtifact[i].GetComponent<Image>().color = color;

            clearButtonsArtifact[i].GetComponent<UIToolTip>().descriptionTextArtifact = artifactMountManager.GetArtifactText(artifactInvenList[15 * ArtifactListPage + i] - 1);
            clearButtonsArtifact[i].GetComponent<UIToolTip>().openTooltip = true;
        }
    }

    public void GetClearButtonArtifact() 
    {
        clearButtonsArtifact = GameObject.FindGameObjectsWithTag("ClearButton");
    }

    public void SetPageNext() 
    {
        ArtifactListPage++;
        ArtifactListUp();
    }

    public void SetPageBefore() 
    {
        ArtifactListPage--;
        if (ArtifactListPage < 0)
        {
            ArtifactListPage = 0;
        }
        ArtifactListUp();
    }

    public void SetAllButtonClear() 
    {
        GetClearButtonArtifact();

        for (int i = 0; i < clearButtonsArtifact.Length; i++)
        {
            
            Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
            color.a = 0.0f;

            
            clearButtonsArtifact[i].GetComponent<Image>().color = color;

            clearButtonsArtifact[i].GetComponent<UIToolTip>().openTooltip = false;
        }
    }

    public void MountArtifactToSinthesis(int ButtonNum)
    {
        if (ButtonNum < pageArtifactNum)    // avoid error
        {
            for (int i = 0; i < 3; i++)
            {
                if (ArtifactsToSynthesis[i] == 999)
                {
                    ArtifactsToSynthesis[i] = 15 * ArtifactListPage + ButtonNum; // 합성 재료가 될 아티팩트의  인벤 리스트 번호

                    GameObject artifactSynthImage = GameObject.Find("Artifact_Synth_Sprite" + i +""); // find object to apply sprite (num is 0~2)

                    // apply sprite
                    artifactSynthImage.GetComponent<Image>().sprite = artifactPrefabs[artifactInvenList[15 * ArtifactListPage + ButtonNum] - 1].GetComponent<SpriteRenderer>().sprite;

                    // change alpha value
                    Color color = artifactSynthImage.GetComponent<Image>().color;
                    color.a = 255.0f;
                    artifactSynthImage.GetComponent<Image>().color = color;

                    break;
                }
            }

            GameObject artifactMounted = GameObject.FindWithTag("Artifact");    // 버그 방지를 위한 장착된 아티팩트 해제
            if (artifactMounted != null)
            {
                artifactMountManager.UnmountArtifact();
            }
        }
    }

    public void UnmountArtifactToSynthsis(int ImageNum)
    {
        if (ArtifactsToSynthesis[ImageNum] < 999)
        {
            GameObject artifactSynthImage = GameObject.Find("Artifact_Synth_Sprite" + ImageNum + ""); // find object to unapply sprite(num is 0~2)
            artifactSynthImage.GetComponent<Image>().sprite = null;

            ArtifactsToSynthesis[ImageNum] = 999;

            // change alpha value
            Color color = artifactSynthImage.GetComponent<Image>().color;
            color.a = 0.0f;
            artifactSynthImage.GetComponent<Image>().color = color;
        }
    }

    public void SynthesisArtifact()
    {
        if (ArtifactsToSynthesis[0] + ArtifactsToSynthesis[1] + ArtifactsToSynthesis[2] < 999)
        {
            int a = artifactInvenList[ArtifactsToSynthesis[0]];
            int b = artifactInvenList[ArtifactsToSynthesis[1]];
            int c = artifactInvenList[ArtifactsToSynthesis[2]];
            artifactManager.SynthesizeArtifact(a, b, c);

            int listNumCheck = artifactManager.artifactInvenList.Length;

            if (listNumCheck < artifactInvenList.Length)
            {
                UnmountArtifactToSynthsis(0);
                UnmountArtifactToSynthsis(1);
                UnmountArtifactToSynthsis(2);

                ArtifactListPage = 0;
                ArtifactListUp();
                message.ShowSystemMessage("합성 성공!");
            }
        }  
    }
}

