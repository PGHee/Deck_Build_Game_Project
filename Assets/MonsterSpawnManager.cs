using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints; // 몬스터가 스폰될 위치들
    public GameObject[] monsterPrefabs; // 몬스터 프리팹 배열

    void Start()
    {
        monsterPrefabs = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            monsterPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Monster{i + 1}");
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
            int monsterPrefabIndex = Random.Range(0, monsterPrefabs.Length);
            GameObject monsterInstance = Instantiate(monsterPrefabs[monsterPrefabIndex], spawnPoints[spawnIndex].transform.position, Quaternion.identity);

            // 다음 스폰 지점 인덱스로 이동
            spawnIndex++;
            if (spawnIndex >= spawnPoints.Length)
            {
                spawnIndex = 0; // 인덱스 초기화
            }
        }
    }
}
