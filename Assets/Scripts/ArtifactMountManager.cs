using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactMountManager : MonoBehaviour
{
    public ArtifactManager artifactManager;

    public int ArtifactListPage;
    public int[] artifactInvenList;
    public int pageArtifactNum;
    public GameObject[] clearButtonsArtifact;
    public GameObject[] artifactPrefabs;




    // Start is called before the first frame update
    void Start()
    {
        ArtifactListPage = 0; // 기본 페이지는 0번
        artifactPrefabs = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            artifactPrefabs[i] = Resources.Load<GameObject>($"Prefabs/artifact{i + 1}");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ArtifactListUp() // 아티팩트 인벤토리를 ui창에 나열
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
            // 현재 색상을 가져와 알파 값만 변경
            Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
            color.a = 255.0f;

            // 변경된 색상을 다시 설정
            clearButtonsArtifact[i].GetComponent<Image>().color = color;
        }
    }

    public void GetClearButtonArtifact() // ui창의 투명 버튼들을 지정
    {
        clearButtonsArtifact = GameObject.FindGameObjectsWithTag("ClearButton");
    }

    public void SetPageNext() // 다음 페이지
    {
        ArtifactListPage++;
        ArtifactListUp();
    }

    public void SetPageBefore() // 이전 페이지
    {
        ArtifactListPage--;
        if (ArtifactListPage < 0)
        {
            ArtifactListPage = 0;
        }
        ArtifactListUp();
    }

    public void SetAllButtonClear() // 아티팩트 나열 시 남는 버튼, 초기 버튼 투명화
    {
        GetClearButtonArtifact();

        for(int i = 0; i < clearButtonsArtifact.Length; i++)
        {
            // 현재 색상을 가져와 알파 값만 변경
            Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
            color.a = 0.0f;

            // 변경된 색상을 다시 설정
            clearButtonsArtifact[i].GetComponent<Image>().color = color;
        }
    }

    public void MountArtifact(int ButtonNum)
    {
        if (ButtonNum < pageArtifactNum)
        {
            // 기존에 장착된 아티팩트 오브젝트 제거
            GameObject artifactMounted = GameObject.FindWithTag("Artifact");
            if (artifactMounted != null)
            {
                Destroy(artifactMounted);
            }
            // 씬에 아티팩트 배치
            artifactManager.GenerateArtifact(artifactInvenList[15 * ArtifactListPage + ButtonNum]);
            artifactManager.GetArtifactInfo();

            GameObject artifactMountImage = GameObject.Find("Artifact_Mount_Sprite");

            artifactMountImage.GetComponent<Image>().sprite = artifactPrefabs[artifactInvenList[15 * ArtifactListPage + ButtonNum] - 1].GetComponent<SpriteRenderer>().sprite;

            Color color = artifactMountImage.GetComponent<Image>().color;
            color.a = 255.0f;

            // 변경된 색상을 다시 설정
            artifactMountImage.GetComponent<Image>().color = color;
        }
        
    }
}
