using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameDirector : MonoBehaviour
{
    public int currentStage;       // 현재 스테이지 번호
    public int currentMap;         // 현재 맵 번호
    public int enterFight;
    public int enterEvent;
    public int killMonster;
    public int killElite;
    public int killBoss;

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
    private EventManager eventManager;
    private BackgroundManager backgroundManager;
    public GameObject settings;
    private EndingPanelManager endingPanelManager;
    public GameObject startPanel;
    private UISoundManager uiSound;
    
    void Start()
    {
        message = FindObjectOfType<SystemMessage>();
        titleMessage = FindObjectOfType<TitleMessageManager>();
        spawnManager = FindObjectOfType<MonsterSpawnManager>();
        turnManager = TurnManager.instance;
        popupManager = FindObjectOfType<PopupManager>();
        deckManager = FindObjectOfType<DeckManager>();
        eventManager = FindObjectOfType<EventManager>();
        backgroundManager = FindObjectOfType<BackgroundManager>();
        endingPanelManager = FindObjectOfType<EndingPanelManager>();
        uiSound = FindObjectOfType<UISoundManager>();
    }
    
    public void StartButton()
    {
        uiSound.PlaySound("UI");
        startPanel.SetActive(false);
        InitGame();
    }

    void InitGame()
    {
        currentStage = 1;
        currentMap = 1;
        enterFight = 0;
        enterEvent = 0;
        killMonster = 0;
        killElite = 0;
        killBoss = 0;
        titleMessage.ShowTitleMessage($"{currentStage} - {currentMap}");

        popupManager.ShowPopup("StartDeck");
        message.ShowSystemMessage("3가지 속성 선택");
        settings.SetActive(true);
        settings.SetActive(false);
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
        backgroundManager.ChangeBackground(currentStage - 1);
        if(currentMap == 10) backgroundManager.ChangeToBossMusic(currentStage);
        titleMessage.ShowTitleMessage($"{currentStage} - {currentMap}");

        deckManager.deckArray = deckManager.CopyOrigin2Deck();
        deckManager.DeckRandomSuffle(deckManager.deckArray);
        deckManager.ClearGrave();
    }

    IEnumerator StartNormalBattle()
    {
        message.ShowSystemMessage("전투 시작!");
        enterFight++;
        yield return new WaitForSeconds(1);
        endTurnButton.SetActive(true);
        spawnManager.SpawnNormalMonsters();
        turnManager.StartBattle();
    }

    IEnumerator StartEliteBattle()
    {
        message.ShowSystemMessage("정예 몬스터 등장! 전투 시작");
        enterFight++;
        yield return new WaitForSeconds(1);
        endTurnButton.SetActive(true);
        spawnManager.SpawnEliteMonster();
        turnManager.StartBattle();
    }

    IEnumerator StartBossBattle()
    {
        message.ShowSystemMessage("보스 등장!! 전투 시작");
        enterFight++;
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
        yield return new WaitForSeconds(2);
        titleMessage.ShowTitleMessage($"마을");
    }

    IEnumerator StartEvent()
    {
        Debug.Log("Triggering Event");
        enterEvent++;
        yield return new WaitForSeconds(1);
        popupManager.ShowPopup("Event");
        eventManager.GetComponent<EventManager>().SetEvent();
        yield return new WaitForSeconds(2);
        titleMessage.ShowTitleMessage($"이벤트");
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSeconds(0.5f);
        endingPanelManager.ActivateEndingPanel();
    }  

    public void KillCount(string Monster)
    {
        switch(Monster)
        {
            case "Monster":
                killMonster++;
                break;
            case "Elite":
                killElite++;
                break;
            case "Bos":
                killBoss++;
                break;
        }
    }
}
