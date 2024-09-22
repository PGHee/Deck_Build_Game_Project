using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactMountManager : MonoBehaviour
{
    public ArtifactManager artifactManager;
    public GameDirector gameDirector;

    public int ArtifactListPage;
    public int[] artifactInvenList;
    public int pageArtifactNum;
    public GameObject[] clearButtonsArtifact;
    public GameObject[] artifactPrefabs;




    // Start is called before the first frame update
    void Start()
    {
        gameDirector = FindObjectOfType<GameDirector>();
        ArtifactListPage = 0; // �⺻ �������� 0��
        artifactPrefabs = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            artifactPrefabs[i] = Resources.Load<GameObject>($"Prefabs/artifact{i + 1}");
        }
    }

    public void ArtifactListUp() // ��Ƽ��Ʈ �κ��丮�� uiâ�� ����
    {
        if (!(gameDirector.currentMapName.Contains("Battle")))
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
                // ���� ������ ������ ���� ���� ����
                Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
                color.a = 255.0f;

                // ����� ������ �ٽ� ����
                clearButtonsArtifact[i].GetComponent<Image>().color = color;
            }
        }       
    }

    public void GetClearButtonArtifact() // uiâ�� ���� ��ư���� ����
    {
        clearButtonsArtifact = GameObject.FindGameObjectsWithTag("ClearButtonBook");
    }

    public void SetPageNext() // ���� ������
    {
        ArtifactListPage++;
        ArtifactListUp();
    }

    public void SetPageBefore() // ���� ������
    {
        ArtifactListPage--;
        if (ArtifactListPage < 0)
        {
            ArtifactListPage = 0;
        }
        ArtifactListUp();
    }

    public void SetAllButtonClear() // ��Ƽ��Ʈ ���� �� ���� ��ư, �ʱ� ��ư ����ȭ
    {
        GetClearButtonArtifact();

        for(int i = 0; i < clearButtonsArtifact.Length; i++)
        {
            // ���� ������ ������ ���� ���� ����
            Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
            color.a = 0.0f;

            // ����� ������ �ٽ� ����
            clearButtonsArtifact[i].GetComponent<Image>().color = color;
        }
    }

    public void MountArtifact(int ButtonNum)
    {
        if (ButtonNum < pageArtifactNum && !(gameDirector.currentMapName.Contains("Battle")))
        {
            // ������ ������ ��Ƽ��Ʈ ������Ʈ ����
            GameObject artifactMounted = GameObject.FindWithTag("Artifact");
            if (artifactMounted != null)
            {
                Destroy(artifactMounted);
            }
            
            artifactManager.GenerateArtifact(artifactInvenList[15 * ArtifactListPage + ButtonNum]);

            GameObject artifactMountImage = GameObject.Find("Artifact_Mount_Sprite");

            artifactMountImage.GetComponent<Image>().sprite = artifactPrefabs[artifactInvenList[15 * ArtifactListPage + ButtonNum] - 1].GetComponent<SpriteRenderer>().sprite;

            Color color = artifactMountImage.GetComponent<Image>().color;
            color.a = 255.0f;

            // ����� ������ �ٽ� ����
            artifactMountImage.GetComponent<Image>().color = color;
        }
    }

    public void UnmountArtifact()
    {
        // 아티팩트 장착 해제
        // 버그 방지를 위해 작동
        // 아티팩트 합성 창 열람 시 작동!!!! 
        GameObject mountedArtifact = GameObject.FindWithTag("Artifact");
        if (mountedArtifact != null)
        {
            Destroy(GameObject.FindWithTag("Artifact")); // 씬에 배치된 아티팩트 제거
            artifactManager.ArtifactBuffRemove();
        }
    }

    public void MountedArtifactBuffApply()
    {
        GameObject mountedArtifact = GameObject.FindWithTag("Artifact");
        if (mountedArtifact != null)
        {
            artifactManager.ArtifactBuffApply();
        }
    }
}
