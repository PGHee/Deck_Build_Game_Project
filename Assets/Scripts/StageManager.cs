using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public int[] portalNums;
    public StageInfo stageInfos;

    // Start is called before the first frame update
    void Start()
    {
        stageInfos = new StageInfo(1, new int[] { 10,20,30,40,50}, null, 1, new int[] { 1,2,3,4,5});
        portalNums = new int[2];
        stageInfos.stageMonsters = stageMonstersList();
        
    }

    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            GenerateShopOpener();
        }
    }

    public void GenerateShopOpener()
    {
        GameObject shopOpener = Resources.Load<GameObject>($"Prefabs/ShopOpener");
        GameObject go = Instantiate(shopOpener);
        go.transform.position = new Vector3(5 , 0, -2);
        go.name = "shopOpener";
    }

    public void DestroyShopOpener()
    {
        Destroy(GameObject.Find("shopOpener"));
    }


    //---------------------------------------------------------------------------
    public GameObject[][] stageMonstersList()
    {
        GameObject[] dest = new GameObject[1];
        GameObject[][] dest2 = new GameObject[5][];
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 1; i++)
            {
                dest[i] = Resources.Load<GameObject>($"Prefabs/Test_monster");
            }
            dest2[j] = dest;
        }
        return dest2;
    }

    public void GenerateRandomPortal()
    {
        portalNums[0] = stageInfos.mapList[Random.Range(0, stageInfos.mapList.Length)];
        portalNums[1] = stageInfos.mapList[Random.Range(0, stageInfos.mapList.Length)];
    }

    public void printStageInfo()
    {
        Debug.Log(stageInfos.stageNum);
        Debug.Log(stageInfos.stageDropGold);
    }
}
