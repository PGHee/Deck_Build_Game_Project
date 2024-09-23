using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public int[] deckArrayOrigin;
    public int[] deckArray;
    public int deckCount;

    public int[] graveArray;
    public int graveCount;

    public CardGenerator cardGenerator;
    public CardRewardManager cardRewardManager;
    public HandControl handController;

    // Start is called before the first frame update
    void Start()
    {
        deckArrayOrigin = new int[] {801,802};

        deckArray = CopyOrigin2Deck();

        DeckRandomSuffle(deckArray);
        handController = FindObjectOfType<HandControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            CardDraw();
        }
    }

    // 덱 랜덤 셔플
    public void DeckRandomSuffle(int[] array) 
    {
        for (int i = 0; i < array.Length; i++)
        {
            int temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    //덱 목록 출력(디버깅)
    public void DeckArrayPrint(int[] array)
    {
        int num = 0;
        for(int i = 0; i<array.Length; i++)
        {
            num += array[i] * (int)Mathf.Pow(10f, (float)(array.Length-i-1));
        }
    }

    // 덱에서 카드 한장 뽑아 생성
    public void CardDraw()
    {
        // 덱에 카드가 없으면
        if (deckArray.Length < 1)
        {
            deckArray = ReloadDeck(); // 묘지 전부 덱으로 이동(셔플ㅇ)
        }

        //Debug.Log(array[array.Length - 1]);

        // 핸드의 카드 수가 최대값보다 적으면
        GameObject[] hands = GameObject.FindGameObjectsWithTag("CardInHand");
        if (hands.Length < 9)
        {
            //카드 생성
            cardGenerator.DrawFromDeck(deckArray[deckArray.Length - 1]);
        }
        else
        {
            // 생성될 카드의 번호를 graveArray로 보냄
            graveArray = Card2Grave(deckArray[deckArray.Length - 1]);
        }
        // 생성 or 묘지로 간 카드를 deckArray에서 삭제
        deckArray = DelCardFromDeck(deckArray, deckArray.Length - 1);
        DeckArrayPrint(deckArray);

        handController.HandSort(null, true);
    }

    // 덱에서 카드 생성(빛속성 숙련도 효과)
    public void CardSearch(int ind)
    {
        // 묘지에 카드가 없으면
        if (deckArray.Length < 1)
        {
            Debug.Log("Empty Deck");
        }
        else
        {
            GameObject[] hands = GameObject.FindGameObjectsWithTag("CardInHand");
            // 핸드의 카드 수가 최대값보다 적으면
            if (hands.Length < 9)
            {
                //카드 생성
                cardGenerator.DrawFromDeck(deckArray[ind]);
            }
            else
            {
                Debug.Log("Full Hand");
                graveArray = Card2Grave(deckArray[ind]);
            }
            deckArray = DelCardFromDeck(deckArray, ind);
        }
    }

    // 묘지에서 카드 생성(암속성 숙련도 효과)
    public void CardSalvage(int ind)
    {
        // 묘지에 카드가 없으면
        if (graveArray.Length < 1)
        {
            Debug.Log("Empty Grave");
        }
        else
        {
            GameObject[] hands = GameObject.FindGameObjectsWithTag("CardInHand");
            // 핸드의 카드 수가 최대값보다 적으면
            if (hands.Length < 9)
            {
                //카드 생성
                cardGenerator.DrawFromDeck(graveArray[ind]);
                graveArray = DelCardFromDeck(graveArray, ind);
            }
            else
            {
                Debug.Log("Full Hand");
            }           
        }        
    }

    // deckArray에서 카드 삭제
    public int[] DelCardFromDeck(int[] array, int ind)
    {
        int[] dest = new int[array.Length - 1];
        for (int i = 0, j = 0; i < array.Length; i++)
        {
            if (i == ind) continue;
            dest[j++] = array[i];
        }
        return dest;
    }

    // graveArray에 인덱스 추가
    public int[] Card2Grave (int cardprefabnum)
    {
        int[] dest = new int[graveArray.Length + 1];
        for (int i = 0, j = 0; i < graveArray.Length; i++)
        {
            dest[j++] = graveArray[i];
        }
        dest[dest.Length - 1] = cardprefabnum;
        return dest;
    }

    // 묘지의 카드들을 덱으로 
    public int[] Grave2Deck()
    {
        int[] dest = new int[graveArray.Length + deckArray.Length];
        int ind = 0;
        for (int i = 0; i < graveArray.Length; i++)
            {
                dest[ind++] = graveArray[i];
            }
        for (int j = 0; j < deckArray.Length; j++)
        {
            dest[ind++] = deckArray[j];
        }
        DeckRandomSuffle(dest);
        return dest;
    }

    // 묘지의 카드를 덱으로 이동(덱이 비었을 경우에만 사용)
    public int[] ReloadDeck()
    {
        int[] dest = new int[graveArray.Length];
        for (int i = 0, j = 0; i < graveArray.Length; i++)
        { 
            dest[j++] = graveArray[i];
        }
        DeckRandomSuffle(dest); // 덱 랜덤 셔플
        ClearGrave();           // 묘지 리셋
        return dest;
    }

    // 묘지 리셋
    void ClearGrave()
    {
        graveArray = new int[] { };
    }

    // 덱오리진, 덱, 묘지에 카드를 추가할 때 사용가능
    public int[] AddCard(int[] array, int cardprefabnum)
    {
        int[] dest = new int[array.Length + 1];
        for (int i = 0, j = 0; i < array.Length; i++)
        {
            dest[j++] = array[i];
        }
        dest[dest.Length - 1] = cardprefabnum;
        return dest;
    }

    public int[] CopyOrigin2Deck()
    {
        int[] dest = new int[deckArrayOrigin.Length];
        for (int i = 0; i < deckArrayOrigin.Length; i++)
        {
            dest[i] = deckArrayOrigin[i];
        }
        return dest;
    }
    // 카드 보상 덱에 추가
    public void AddRewardCard(int num)
    {
        int[] dest = new int[deckArrayOrigin.Length + 1];
        for (int i = 0, j = 0; i < deckArrayOrigin.Length; i++)
        {
            dest[j++] = deckArrayOrigin[i];
        }
        dest[dest.Length - 1] = num;

        deckArrayOrigin = new int[dest.Length];
        for (int j = 0; j < dest.Length; j++)
        {
            deckArrayOrigin[j] = dest[j];
        }
    }

    public void AddRewardCard1()
    {
        int[] rewardCardIndex = cardRewardManager.cardPrefabNum;
        int[] dest = new int[deckArrayOrigin.Length + 1];
        for (int i = 0, j = 0; i < deckArrayOrigin.Length; i++)
        {
            dest[j++] = deckArrayOrigin[i];
        }
        dest[dest.Length - 1] = rewardCardIndex[0];

        deckArrayOrigin = new int[dest.Length];
        for (int j = 0; j < dest.Length; j++)
        {
            deckArrayOrigin[j] = dest[j];
        }
    }

    public void AddRewardCard2()
    {
        int[] rewardCardIndex = cardRewardManager.cardPrefabNum;
        int[] dest = new int[deckArrayOrigin.Length + 1];
        for (int i = 0, j = 0; i < deckArrayOrigin.Length; i++)
        {
            dest[j++] = deckArrayOrigin[i];
        }
        dest[dest.Length - 1] = rewardCardIndex[1];

        deckArrayOrigin = new int[dest.Length];
        for (int j = 0; j < dest.Length; j++)
        {
            deckArrayOrigin[j] = dest[j];
        }
    }

    public void AddRewardCard3()
    {
        int[] rewardCardIndex = cardRewardManager.cardPrefabNum;
        int[] dest = new int[deckArrayOrigin.Length + 1];
        for (int i = 0, j = 0; i < deckArrayOrigin.Length; i++)
        {
            dest[j++] = deckArrayOrigin[i];
        }
        dest[dest.Length - 1] = rewardCardIndex[2];

        deckArrayOrigin = new int[dest.Length];
        for (int j = 0; j < dest.Length; j++)
        {
            deckArrayOrigin[j] = dest[j];
        }
    }

    public void DeleteOriginCard(int cardNum)
    {
        int deleteIndex = 9999;
        for (int i = 0; i < deckArrayOrigin.Length; i++)
        {
            if (deckArrayOrigin[i] == cardNum)
            {
                deleteIndex = i;
                break;
            }
            else deleteIndex = 9999;
            Debug.Log("None card");
        }

        if (deleteIndex < 9999)
        {
            int[] dest = new int[deckArrayOrigin.Length - 1];
            for (int i = 0, j = 0; i < deckArrayOrigin.Length; i++)
            {
                if (i == deleteIndex) continue;
                dest[j++] = deckArrayOrigin[i];
            }
            deckArrayOrigin = dest;
        }
    }

    public void TurnStartCard()
    {
        for(int i = 0; i <  4; i++) // change this number to adjust draw
        {
            CardDraw();
        }
    }
}
