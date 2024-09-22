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
    public int cardListPage;
    public int[] cardList;
    public int pageCardNum;
    public GameObject[] clearButtonsCard;
    public GameObject[] cardPrefabs;

    public string deckTypeListUP;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = FindObjectOfType<GameDirector>();
        deckManager = FindObjectOfType<DeckManager>();
        dynamicButtonManager = FindObjectOfType<DynamicButtonManager>();

        cardListPage = 0;
        search = true;
    }

    public void CardListUp(string deckType)
    {
        search = true;
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

            default:
                Debug.Log("Wrong Deck Type To List Up");
                break;
        }
        deckTypeListUP = deckType; // 현재 표시되는 덱 타입을 저장

        if (cardList.Length > 0)
        {
            SetAllButtonClear();

            GetClearButtonCard();

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
        }
    }

    public void GetClearButtonCard() // uiâ�� ���� ��ư���� ����
    {
        clearButtonsCard = GameObject.FindGameObjectsWithTag("ClearButtonBook");
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
                deckManager.CardSearch(15 * cardListPage + ButtonNum);
                search = false;
            }
            else
            {
                Debug.Log(" already search card");
            }    
        }
    }
}
