using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterSpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints;            // 몬스터가 스폰될 위치들
    public GameObject[,] monsterPrefabs;     // 몬스터 프리팹 배열
    public GameObject[,] elitePrefabs;          // 정예몬스터 프리팹 배열
    public GameObject[] bossPrefabs;            // 보스몬스터 프리팹 배열
    public GameObject hpBarPrefab;              // HP 바 프리팹
    public GameObject turnActionPrefab;

    void Start()
    {
        monsterPrefabs = new GameObject[5, 4];
        elitePrefabs = new GameObject[5, 2];
        bossPrefabs = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++) monsterPrefabs[i, j] = Resources.Load<GameObject>($"Prefabs/Monster/Monster{i + 1}{j + 1}");
            for (int j = 0; j < 2; j++) elitePrefabs[i, j] = Resources.Load<GameObject>($"Prefabs/Monster/Elite{i + 1}{j + 1}");
            bossPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Monster/Boss{i + 1}");
        }
        // 전투 시작 시 몬스터 스폰
        SpawnNormalMonsters();
        //SpawnEliteMonster();
        //SpawnBossMonster();
    }

    void SpawnNormalMonsters()
    {
        int monsterCount = Random.Range(1, 4);                      // 스폰할 몬스터 수를 1에서 3 사이로 랜덤 설정
        int spawnIndex = 0;                                         // 현재 스폰 지점 인덱스
        int stageNum = 1;                                           // 스테이지 번호
        for (int i = 0; i < monsterCount; i++)                      // 현재 스폰 지점에서 소환될 몬스터를 결정하고 소환
        {
            int monsterPrefabIndex = Random.Range(0, 4);            // 스폰할 몬스터 프리팹 랜덤 선택
            GameObject monsterInstance = Instantiate(monsterPrefabs[stageNum - 1, monsterPrefabIndex], spawnPoints[spawnIndex].transform.position, Quaternion.identity);
            AddHPBar(monsterInstance);                              // 몬스터에 HP 바 추가
            AddAction(monsterInstance);                             // 몬스터에 액션 바 추가
            spawnIndex++;                                           // 다음 스폰 지점 인덱스로 이동
            if (spawnIndex >= spawnPoints.Length) spawnIndex = 0;   // 인덱스 초기화
        }
    }

    public void SpawnEliteMonster()
    {
        int stageNum = 1; // 스테이지 번호
        if (stageNum == 5)
        {
            // 5스테이지에서는 엘리트 몬스터 1번과 2번이 같이 출현
            GameObject eliteInstance1 = Instantiate(elitePrefabs[stageNum - 1, 0], spawnPoints[0].transform.position, Quaternion.identity);
            GameObject eliteInstance2 = Instantiate(elitePrefabs[stageNum - 1, 1], spawnPoints[1].transform.position, Quaternion.identity);
            AddHPBar(eliteInstance1);
            AddAction(eliteInstance1);
            AddHPBar(eliteInstance2);
            AddAction(eliteInstance2);
        }
        else
        {
            // 다른 스테이지에서는 엘리트 몬스터 1마리와 일반 몬스터 1마리가 출현
            int monsterPrefabIndex = Random.Range(0, 4);
            GameObject monsterInstance = Instantiate(monsterPrefabs[stageNum - 1, monsterPrefabIndex], spawnPoints[0].transform.position, Quaternion.identity);
            AddHPBar(monsterInstance);
            AddAction(monsterInstance);
            int eliteIndex = Random.Range(0, 2);
            GameObject eliteInstance = Instantiate(elitePrefabs[stageNum - 1, eliteIndex], spawnPoints[1].transform.position, Quaternion.identity);
            AddHPBar(eliteInstance);
            AddAction(eliteInstance);
        }
    }

    public void SpawnBossMonster()
    {
        int stageNum = 1;                                           // 스테이지 번호
        GameObject bossInstance = Instantiate(bossPrefabs[stageNum - 1], spawnPoints[0].transform.position, Quaternion.identity);
        AddHPBar(bossInstance);
        AddAction(bossInstance);
    }

    void AddHPBar(GameObject monsterInstance)
    {
        MonsterState monster = monsterInstance.GetComponent<MonsterState>();
        GameObject hpBarInstance = Instantiate(hpBarPrefab, monsterInstance.transform.position, Quaternion.identity);   // HP 바 설정
        HPBar hpBar = hpBarInstance.GetComponent<HPBar>();
        TextMeshProUGUI healthText = hpBarInstance.GetComponentInChildren<TextMeshProUGUI>();           // HPText 오브젝트를 찾아서 할당
        if (healthText != null) hpBar.healthText = healthText;
        hpBar.Initialize(monsterInstance.transform, monster.maxHealth, monster.shield, monster.poisonStacks, new Vector3(0, 0, 0));
        monster.SetHPBar(hpBar);
        Transform buffIconPanel = hpBarInstance.transform.Find("Canvas/BuffIconPanel");     // BuffIconPanel과 DebuffIconPanel을 찾아서 할당
        Transform debuffIconPanel = hpBarInstance.transform.Find("Canvas/DebuffIconPanel");
        if (buffIconPanel != null && debuffIconPanel != null)
        {
            monster.buffIconPanel = buffIconPanel;
            monster.debuffIconPanel = debuffIconPanel;
        }
    }

    void AddAction(GameObject monsterInstance)
    {
        MonsterState monster = monsterInstance.GetComponent<MonsterState>();
        GameObject actionUIInstance = Instantiate(turnActionPrefab, monsterInstance.transform.position, Quaternion.identity);
        TurnActionUI actionUI = actionUIInstance.GetComponent<TurnActionUI>();
        TextMeshProUGUI monsterActionText = actionUIInstance.GetComponentInChildren<TextMeshProUGUI>();
        if(monsterActionText != null) actionUI.monsterActionText = monsterActionText;
        actionUI.Initialize(monsterInstance.transform, monster.selectedAction.actionType);
        monster.SetAction(actionUI);
    }
}
