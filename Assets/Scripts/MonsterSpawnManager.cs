using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    /* 
    현재 스테이지 별로 몬스터 프리팹을 달리 두어 소환할 수 있도록 만들 예정임
    주석 처리된 부분이 스테이지 별 몬스터 스폰 코드의 초안
    일반 몬스터, 정예 몬스터, 보스 몬스터로 구분하여 SpawnMonsters(), SpawnElite(), SpawnBoss() 함수를 제작할 예정임
    각 몬스터는 StageManager의 stageNum 값을 받아오는 것으로 해당 스테이지의 몬스터 중에 랜덤 스폰될 예정
    */
    private StageManager stageManager;

    public GameObject[] spawnPoints;            // 몬스터가 스폰될 위치들
    public GameObject[] monsterPrefabs;         // 삭제 예정
    // public GameObject[,] monsterPrefabs;     // 몬스터 프리팹 배열
    public GameObject[,] elitePrefabs;          // 정예몬스터 프리팹 배열
    public GameObject[] bossPrefabs;

    void Start()
    {
        stageManager = FindObjectOfType<StageManager>();

        monsterPrefabs = new GameObject[5];
        // monsterPrefabs = new GameObject[5, 5];
        elitePrefabs = new GameObject[5, 3];
        bossPrefabs = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            monsterPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Monster{i + 1}");       // 삭제 예정
            //for (int j = 0; j < 5; j++) monsterPrefabs[i, j] = Resources.Load<GameObject>($"Prefabs/Monster{i + 1}{j + 1}");
            //for (int j = 0; j < 3; j++) elitePrefabs[i, j] = Resources.Load<GameObject>($"Prefabs/Elite{i + 1}{j + 1}");
            //bossPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Boss{i + 1}");
        }
        // 전투 시작 시 몬스터 스폰
        SpawnMonsters();
    }

    void SpawnMonsters()
    {
        // 스폰할 몬스터 수를 1에서 3 사이로 랜덤 설정
        int monsterCount = Random.Range(1, 4);

        int spawnIndex = 0; // 현재 스폰 지점 인덱스

        // 현재 스폰 지점에서 소환될 몬스터를 결정하고 소환
        for (int i = 0; i < monsterCount; i++)
        {
            // 스폰할 몬스터 프리팹 랜덤 선택
            int monsterPrefabIndex = Random.Range(0, 5);
            int stageNum = 1; //stageManager.stageInfos.stageNum;

            // 삭제 예정
            GameObject monsterInstance = Instantiate(monsterPrefabs[monsterPrefabIndex], spawnPoints[spawnIndex].transform.position, Quaternion.identity);
            // GameObject monsterInstance = Instantiate(monsterPrefabs[stageNum, monsterPrefabIndex], spawnPoints[spawnIndex].transform.position, Quaternion.identity);

            // 다음 스폰 지점 인덱스로 이동
            spawnIndex++;
            if (spawnIndex >= spawnPoints.Length)
            {
                spawnIndex = 0; // 인덱스 초기화
            }
        }
    }
}
