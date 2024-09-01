using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public int currentStage;       // 현재 스테이지 번호
    public int currentMap;         // 현재 맵 번호
    public string currentMapName;
    private MonsterSpawnManager spawnManager;
    private TurnManager turnManager;
    private SystemMessage message;
    private TitleMessageManager titleMessage;
    private PopupManager popupManager;

    public StageManager stageManager;
    public GameObject endTurnButton;

    void Start()
    {
        message = FindObjectOfType<SystemMessage>();
        titleMessage = FindObjectOfType<TitleMessageManager>();
        spawnManager = FindObjectOfType<MonsterSpawnManager>();
        turnManager = TurnManager.instance;
        popupManager = FindObjectOfType<PopupManager>();
        InitGame();
    }

    /* 
    현재 Start()에 넣으면 스폰 매니저의 Start()가 씹히는 문제가 있긴하나, 어차피 포탈을 탄 경우에 전투가 시작될 것이라 상관 없을 듯
    update함수는 안쓰는 것이 과부하를 적게 일으키나, 현재 포탈이 연결되지 않은 상태라 update로 구현되었고, 삭제 예정
    아래의 각 함수들을 포탈에 연결하여 호출하는 방식으로 작성해주길 바람.
    이어서 마을과 이벤트 맵을 호출하는 함수들도 아래에 있는 함수에 연결해주어야 함.
    */

    void Update()
    {
        if (Input.GetKeyDown("q")) StartCoroutine(StartNormalBattle());
        if (Input.GetKeyDown("w")) StartCoroutine(StartEliteBattle());
        if (Input.GetKeyDown("e")) StartCoroutine(StartBossBattle());
    }

    void InitGame()
    {
        currentStage = 1;
        currentMap = 1;
        titleMessage.ShowTitleMessage($"{currentStage} - {currentMap}");
    }

    public void OnPortalEntered(string portalType)
    {
        switch (portalType)
        {
            case "NormalBattle":
                StartCoroutine(StartNormalBattle());
                break;
            case "EliteBattle":
                StartCoroutine(StartEliteBattle());
                break;
            case "BossBattle":
                StartCoroutine(StartBossBattle());
                break;
            case "Village":
                EnterVillage();
                break;
            case "Event":
                StartEvent();
                break;
        }
        // 맵 이동 후 다음 맵으로 전환
        currentMap++;
        if (currentMap > 10)
        {
            currentMap = 1;
            currentStage++;
        }
        titleMessage.ShowTitleMessage($"{currentStage} - {currentMap}");
    }

    IEnumerator StartNormalBattle()
    {
        message.ShowSystemMessage("전투 시작!");
        yield return new WaitForSeconds(1);
        endTurnButton.SetActive(true);
        spawnManager.SpawnNormalMonsters();
        turnManager.StartBattle();
    }

    IEnumerator StartEliteBattle()
    {
        message.ShowSystemMessage("정예 몬스터 등장! 전투 시작");
        yield return new WaitForSeconds(1);
        endTurnButton.SetActive(true);
        spawnManager.SpawnEliteMonster();
        turnManager.StartBattle();
    }

    IEnumerator StartBossBattle()
    {
        message.ShowSystemMessage("보스 등장!! 전투 시작");
        yield return new WaitForSeconds(1);
        endTurnButton.SetActive(true);
        spawnManager.SpawnBossMonster();
        turnManager.StartBattle();
    }

    void EnterVillage()
    {
        Debug.Log("Entering Village");
        popupManager.ShowPopup("VillageChief");
    }

    void StartEvent()
    {
        Debug.Log("Triggering Event");
        stageManager.GenerateEventOpener();
    }

    void EndGame()
    {
        Debug.Log("Game Over");
        // 게임 종료 로직
    }
}
