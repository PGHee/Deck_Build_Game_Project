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

    public StageInfo(int stageNum, int[] stageDropGold, GameObject[][] stageMonsters, int mapNum, int[] mapList)
    {
        this.stageNum = stageNum;   // 스테이지 번호, 스테이지 넘어가면 +1 해줄것, 스테이지 관련 인덱스의 역할 
        this.stageDropGold = stageDropGold; // 해당 스테이지에서 드랍하는 골드의 리스트(5개 맵 전부 등록)
        this.stageMonsters = stageMonsters; // 각 스테이지에서 생성될 수 있는 몬스터 프리팹의 2중 배열
        this.mapNum = mapNum;
        this.mapList = mapList; // 이동 가능한 맵의 종류, 확률을 갯수로 조절
    }
}


public class GameManager : MonoBehaviour
{
    public StageManager stageManager;
    public DeckManager deckManager;
    public StageInfo stageInfos;

    // Start is called before the first frame update
    void Start()
    {
        stageInfos = new StageInfo(1, new int[] { 10, 20, 30, 40, 50 }, null, 1, new int[] { 1, 2, 3, 4, 5 });
        stageInfos.stageMonsters = stageMonstersList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBattle()
    {
        // 전투 정보 리셋
        stageInfos.stageMonsters = stageMonstersList(); // 스테이지 별 몬스터 배열 초기화

        // 디버프 초기화

        // 버프 초기화

        // 유물 초기화

        // 카드 초기화

        // 디버그용 출력 부분
    }

    public GameObject[][] stageMonstersList()
    {
        GameObject[] dest = new GameObject[1];
        GameObject[][] dest2 = new GameObject[5][];
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 1; i++)  //반복은 몬스터 수에 따라 다르게 변경 가능
            {
                dest[i] = Resources.Load<GameObject>($"Prefabs/Monster1");
            }
            dest2[j] = dest;
        }
        return dest2;
    }
}
