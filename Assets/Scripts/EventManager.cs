using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class EventManager : MonoBehaviour
{
    public PlayerState playerState;
    public DeckManager deckManager;
    public ArtifactManager artifactManager;
    public RewardManager rewardManager;
    private SystemMessage message;

    public int eventNum;
    public GameObject eventPrefab;
    public GameObject[] eventPrefabs;

    public GameObject eventImage;
    public TextMeshProUGUI eventText;

    public string eventInput;
    public string eventOutput;

    public int eventInputNum;
    public int eventOutputNum;

    public bool eventON;

    void Start()
    {
        message = FindObjectOfType<SystemMessage>();
        eventON = true;
    }


    public void RandomSelectEvent()
    {
        // "Prefabs"는 Resources 폴더 내의 서브 폴더 이름입니다.
        eventPrefabs = Resources.LoadAll<GameObject>($"Prefabs/Event");
        Debug.Log(eventPrefabs.Length);
        List<int> chanceList = new List<int>();

        for(int i = 0; i < eventPrefabs.Length; i++)
        {
            for (int j = 0; j < eventPrefabs[i].GetComponent<EventInfo>().eventChance; j++)
            {
                chanceList.Add(i + 1);
            }
        }
        eventNum = chanceList[Random.Range(0, chanceList.Count)];
        
        eventPrefab = Resources.Load<GameObject>($"Prefabs/Event/event{eventNum}");
    }

    public void SetEvent()
    {
        RandomSelectEvent();

        eventImage.GetComponent<Image>().sprite = eventPrefab.GetComponent<EventInfo>().eventImage;
        eventText.text = eventPrefab.GetComponent<EventInfo>().eventText;

        eventInput = eventPrefab.GetComponent<EventInfo>().eventInput;
        eventOutput = eventPrefab.GetComponent<EventInfo>().eventOutput;
        eventInputNum = eventPrefab.GetComponent<EventInfo>().eventInputNum;
        eventOutputNum = eventPrefab.GetComponent<EventInfo>().eventOutputNum;

        eventON = true;

    }

    public void ResetEvent()
    {

    }

    public void AcceptEvent()
    {
        if (eventInput.Length > 0 && eventON)
        {
            switch (eventInput)
            {
                case "Crystal":
                    if(playerState.crystal >= eventInputNum)
                    {
                        rewardManager.GetReward(eventOutput, eventOutputNum, eventInputNum);
                        //message.ShowSystemMessage("-" + eventInput + "");
                        eventText.text = eventPrefab.GetComponent<EventInfo>().eventOutputText;
                        eventON = false;
                    }
                    else message.ShowSystemMessage("크리스탈이 부족해.");

                    break;

                case "Card":
                    if (deckManager.deckArrayOrigin.Contains(eventInputNum))
                    {
                        deckManager.DeleteOriginCard(eventInputNum);
                        rewardManager.GetReward(eventOutput, eventOutputNum, 0);
                        eventText.text = eventPrefab.GetComponent<EventInfo>().eventOutputText;
                        eventON = false;
                        if (eventInputNum != 0)
                        {
                            //message.ShowSystemMessage("-" + eventInput + "");
                        }  
                    }
                    else message.ShowSystemMessage("맞는 카드가 없어");
                    break;

                case "Artifact":
                    if (artifactManager.artifactInvenList.Contains(eventInputNum))
                    {
                        artifactManager.DeleteArtifact2Inven(eventInputNum);
                        rewardManager.GetReward(eventOutput, eventOutputNum, 0);
                        eventText.text = eventPrefab.GetComponent<EventInfo>().eventOutputText;
                        eventON = false;
                        //message.ShowSystemMessage("-" + eventInput + "");
                    }
                    else message.ShowSystemMessage("맞는 아티팩트가 없어");
                    break ;

                case "HP":
                    if(playerState.currentHealth > eventInputNum)
                    {
                        playerState.TakeDamage(eventInputNum);
                        rewardManager.GetReward(eventOutput, eventOutputNum, 0);
                        eventText.text = eventPrefab.GetComponent<EventInfo>().eventOutputText;
                        eventON = false;
                        //message.ShowSystemMessage("-" + eventInput + "");
                    }
                    else message.ShowSystemMessage("체력이 부족해");
                    break ;

                default:
                    Debug.Log("unknown inputcode");
                    break;
            }
        }
        else
        {
            message.ShowSystemMessage("이미 지나간 일이야");
        }
    }

    public void RefuseEvent()
    {

    }
}
