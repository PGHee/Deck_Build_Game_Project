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
    public FadeManager fadeManager;
    private DeckManager deckManager;

    public StageManager stageManager;
    public GameObject endTurnButton;

    void Start()
    {
        message = FindObjectOfType<SystemMessage>();
        titleMessage = FindObjectOfType<TitleMessageManager>();
        spawnManager = FindObjectOfType<MonsterSpawnManager>();
        turnManager = TurnManager.instance;
        popupManager = FindObjectOfType<PopupManager>();
        deckManager = FindObjectOfType<DeckManager>();
        InitGame();
    }

    void InitGame()
    {
        currentStage = 1;
        currentMap = 1;
        titleMessage.ShowTitleMessage($"{currentStage} - {currentMap}");

        popupManager.ShowPopup("StartDeck");
    }

    public void OnPortalEntered(string portalType)
    {
        fadeManager.gameObject.SetActive(true);
        fadeManager.StartFadeInOut();
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
                StartCoroutine(EnterVillage());
                break;
            case "Event":
                StartCoroutine(StartEvent());
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

        deckManager.deckArray = deckManager.CopyOrigin2Deck();
        deckManager.DeckRandomSuffle(deckManager.deckArray);
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

    IEnumerator EnterVillage()
    {
        Debug.Log("Entering Village");
        yield return new WaitForSeconds(1);
        popupManager.ShowPopup("VillageChief");
    }

    IEnumerator StartEvent()
    {
        Debug.Log("Triggering Event");
        yield return new WaitForSeconds(1);
        stageManager.GenerateEventOpener();
    }

    void EndGame()
    {
        Debug.Log("Game Over");
        // 게임 종료 로직
    }
}
