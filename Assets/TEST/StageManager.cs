using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo
{
    public int stageNum;
    public int[] stageDropGold;
    public GameObject[][] stageMonsters;
    public int mapNum;
    public int[] mapList;

    public StageInfo(int stageNum, int[] stageDropGold, GameObject[][] stagemonsters, int mapNum, int[] mapList)
    {
        this.stageNum = stageNum;   // 스테이지 번호, 스테이지 넘어가면 +1 해줄것, 스테이지 관련 인덱스의 역할 
        this.stageDropGold = stageDropGold; // 해당 스테이지에서 드랍하는 골드의 리스트(5개 맵 전부 등록)
        this.stageMonsters = stagemonsters; // 각 스테이지에서 생성될 수 있는 몬스터 프리팹의 2중 배열
        this.mapNum = mapNum;
        this.mapList = mapList; // 이동 가능한 맵의 종류, 확률을 갯수로 조절
    }
}

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

        GenerateRandomPortal();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject[][] stageMonstersList()
    {
        GameObject[] dest = new GameObject[1];
        GameObject[][] dest2 = new GameObject[5][];
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 1; i++)  //반복은 몬스터 수에 따라 다르게 변경 가능
            {
                dest[i] = Resources.Load<GameObject>($"Prefabs/Test_monster");
            }
            dest2[j] = dest;
        }
        return dest2;
    }

    public void mapTransition()
    {
        // 실제 맵 이동을 구현
        // 각각의 버튼을 눌러 얻은 값을 참고하여 다음 맵으로 초기화
        // 몬스터 스폰을 제외한 나머지는 캔버스로 제작하여 popup
        // 
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
