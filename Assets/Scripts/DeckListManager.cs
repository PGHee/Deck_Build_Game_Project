using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckListManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameDirector gameDirector;
    public DynamicButtonManager dynamicButtonManager;

    public bool search;
    public bool delete;
    public bool normalSearch;
    public int cardListPage;
    public int[] cardList;
    public int pageCardNum;
    public GameObject[] clearButtonsCard;
    public GameObject[] cardPrefabs;

    public string deckTypeListUP;

    private SystemMessage message;
    private PlayerState playerState;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = FindObjectOfType<GameDirector>();
        deckManager = FindObjectOfType<DeckManager>();
        dynamicButtonManager = FindObjectOfType<DynamicButtonManager>();

        cardListPage = 0;
        search = true;
        normalSearch = false;
        message = FindObjectOfType<SystemMessage>();
        playerState = FindObjectOfType<PlayerState>();
    }

    public void CardListUp(string deckType)
    {
        search = true;
        delete = false;
        switch (deckType)
        {
            case "DeckArrayOrigin":
                cardList = deckManager.deckArrayOrigin;
                search = false;
                break;

            case "DeckArray":
                cardList = deckManager.deckArray;
                break;

            case "GraveArray":
                cardList = deckManager.graveArray;
                break ;

            case "DeleteCard":
                cardList = deckManager.deckArrayOrigin;
                search = false;
                delete = true;
                break;

            default:
                Debug.Log("Wrong Deck Type To List Up");
                break;
        }
        deckTypeListUP = deckType; // 현재 표시되는 덱 타입을 저장


        SetAllButtonClear();

        GetClearButtonCard();

        if (cardList.Length > 0)
        {
            if (cardList.Length > 15 * (cardListPage + 1))
            {
                pageCardNum = 15;
            }
            else
            {
                pageCardNum = cardList.Length % 15;
                cardListPage = (int)((cardList.Length - cardList.Length % 15) / 15);
            }

            for (int i = 0; i < pageCardNum; i++)
            {
                dynamicButtonManager.CardSpriteToButton(cardList[15 * cardListPage + i] - 1, clearButtonsCard[i]);
                clearButtonsCard[i].GetComponent<ShopButtonManager>().cardPrefabNum = cardList[15 * cardListPage + i] - 1;

                Color color = clearButtonsCard[i].GetComponent<Image>().color;
                color.a = 255.0f;

                clearButtonsCard[i].GetComponent<Image>().color = color;
            }
        }
        else
        {
            Debug.Log("Empty Array" + deckTypeListUP + "");
            message.ShowSystemMessage("빈 카드 더미 입니다.");
        }
    }

    public void GetClearButtonCard() // uiâ�� ���� ��ư���� ����
    {
        clearButtonsCard = GameObject.FindGameObjectsWithTag("ClearButtonBook");
        foreach(var card in clearButtonsCard)
        {
            card.GetComponent<ShopButtonManager>().cardPrefabNum = 9999;
        }
    }

    public void SetAllButtonClear() // ��Ƽ��Ʈ ���� �� ���� ��ư, �ʱ� ��ư ����ȭ
    {
        GetClearButtonCard();

        for (int i = 0; i < clearButtonsCard.Length; i++)
        {
            // ���� ������ ������ ���� ���� ����
            Color color = clearButtonsCard[i].GetComponent<Image>().color;
            color.a = 0.0f;

            // ����� ������ �ٽ� ����
            clearButtonsCard[i].GetComponent<Image>().color = color;
        }
    }

    public void SetPageNext() // ���� ������
    {
        cardListPage++;
        CardListUp(deckTypeListUP);
    }

    public void SetPageBefore() // ���� ������
    {
        cardListPage--;
        if (cardListPage < 0)
        {
            cardListPage = 0;
        }
        CardListUp(deckTypeListUP);
    }
    public void SpawnCardFromList(int ButtonNum)
    {
        if (ButtonNum < pageCardNum && gameDirector.currentMapName.Contains("Battle") && deckTypeListUP != "DeckArrayOrigin")
        {
            if (search)
            {
                if(deckTypeListUP == "DeckArray")
                {
                    if ((int)(deckManager.deckArray[15 * cardListPage + ButtonNum] / 100) == 6 || normalSearch)
                    {
                        deckManager.CardSearch(15 * cardListPage + ButtonNum);
                        search = false;
                        SetButtonConfirm(ButtonNum);
                        normalSearch = false;
                    }
                    else
                    {
                        if(!normalSearch) message.ShowSystemMessage("빛 속성 카드만 가져올 수 있습니다.");
                    }
                    
                }else if(deckTypeListUP == "GraveArray")
                {
                    if ((int)(deckManager.deckArray[15 * cardListPage + ButtonNum] / 100) == 7)
                    {
                        deckManager.CardSalvage(15 * cardListPage + ButtonNum);
                        search = false;
                        SetButtonConfirm(ButtonNum);
                    }
                    else
                    {
                        message.ShowSystemMessage("어둠 속성 카드만 가져올 수 있습니다.");
                    }
                }
                
            }
            else
            {
                message.ShowSystemMessage("이미 카드를 가져왔다.");
            }    
        }
        else if(ButtonNum < pageCardNum && gameDirector.currentMapName.Contains("Village") && deckTypeListUP != "DeckArrayOrigin")
        {
            if (delete)
            {
                if(playerState.crystal >= (int)(100 + 100 * (gameDirector.currentStage - 1) * 0.5))
                {
                    playerState.SpendCrystal((int)(100 + 100 * (gameDirector.currentStage - 1) * 0.5));
                    deckManager.deckArrayOrigin = deckManager.DelCardFromDeck(deckManager.deckArrayOrigin, 15 * cardListPage + ButtonNum);
                    delete = false;
                    message.ShowSystemMessage("카드 제거");
                    SetDeleteButtonConfirm(ButtonNum);
                }
                else
                {
                    message.ShowSystemMessage("크리스탈이 부족하다.");
                }
                
            }
            else
            {
                message.ShowSystemMessage("이미 카드를 제거했다.");
            }
        }
    }

    public void SetButtonConfirm(int buttonNum)
    {
        GetClearButtonCard();
        RectTransform rectTransform = clearButtonsCard[buttonNum].GetComponent<RectTransform>();
        GameObject go = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Item_Confirm"), rectTransform.position, Quaternion.identity, GameObject.Find("Panel_DeckList").GetComponent<RectTransform>());
        go.GetComponent<RectTransform>().position = rectTransform.position;
        go.name = "Item_Confirm";
    }

    public void SetDeleteButtonConfirm(int buttonNum)
    {
        GetClearButtonCard();
        RectTransform rectTransform = clearButtonsCard[buttonNum].GetComponent<RectTransform>();
        GameObject go = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Item_Deleted_Confirm"), rectTransform.position, Quaternion.identity, GameObject.Find("Panel_DeckList").GetComponent<RectTransform>());
        go.GetComponent<RectTransform>().position = rectTransform.position;
        go.name = "Item_Confirm";
    }

    public void DeleteButtonConfirm()
    {
        GameObject confirmed = GameObject.Find("Item_Confirm");
        if(confirmed != null) Destroy(GameObject.Find("Item_Confirm"));
    }
}
