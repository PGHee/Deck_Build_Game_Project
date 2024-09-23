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

    // �� ���� ����
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

    //�� ��� ���(�����)
    public void DeckArrayPrint(int[] array)
    {
        int num = 0;
        for(int i = 0; i<array.Length; i++)
        {
            num += array[i] * (int)Mathf.Pow(10f, (float)(array.Length-i-1));
        }
    }

    // ������ ī�� ���� �̾� ����
    public void CardDraw()
    {
        // ���� ī�尡 ������
        if (deckArray.Length < 1)
        {
            deckArray = ReloadDeck(); // ���� ���� ������ �̵�(���ä�)
        }

        //Debug.Log(array[array.Length - 1]);

        // �ڵ��� ī�� ���� �ִ밪���� ������
        GameObject[] hands = GameObject.FindGameObjectsWithTag("CardInHand");
        if (hands.Length < 9)
        {
            //ī�� ����
            cardGenerator.DrawFromDeck(deckArray[deckArray.Length - 1]);
        }
        else
        {
            // ������ ī���� ��ȣ�� graveArray�� ����
            graveArray = Card2Grave(deckArray[deckArray.Length - 1]);
        }
        // ���� or ������ �� ī�带 deckArray���� ����
        deckArray = DelCardFromDeck(deckArray, deckArray.Length - 1);
        DeckArrayPrint(deckArray);

        handController.HandSort(null, true);
    }

    // ������ ī�� ����(���Ӽ� ���õ� ȿ��)
    public void CardSearch(int ind)
    {
        // ������ ī�尡 ������
        if (deckArray.Length < 1)
        {
            Debug.Log("Empty Deck");
        }
        else
        {
            GameObject[] hands = GameObject.FindGameObjectsWithTag("CardInHand");
            // �ڵ��� ī�� ���� �ִ밪���� ������
            if (hands.Length < 9)
            {
                //ī�� ����
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

    // �������� ī�� ����(�ϼӼ� ���õ� ȿ��)
    public void CardSalvage(int ind)
    {
        // ������ ī�尡 ������
        if (graveArray.Length < 1)
        {
            Debug.Log("Empty Grave");
        }
        else
        {
            GameObject[] hands = GameObject.FindGameObjectsWithTag("CardInHand");
            // �ڵ��� ī�� ���� �ִ밪���� ������
            if (hands.Length < 9)
            {
                //ī�� ����
                cardGenerator.DrawFromDeck(graveArray[ind]);
                graveArray = DelCardFromDeck(graveArray, ind);
            }
            else
            {
                Debug.Log("Full Hand");
            }           
        }        
    }

    // deckArray���� ī�� ����
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

    // graveArray�� �ε��� �߰�
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

    // ������ ī����� ������ 
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

    // ������ ī�带 ������ �̵�(���� ����� ��쿡�� ���)
    public int[] ReloadDeck()
    {
        int[] dest = new int[graveArray.Length];
        for (int i = 0, j = 0; i < graveArray.Length; i++)
        { 
            dest[j++] = graveArray[i];
        }
        DeckRandomSuffle(dest); // �� ���� ����
        ClearGrave();           // ���� ����
        return dest;
    }

    // ���� ����
    void ClearGrave()
    {
        graveArray = new int[] { };
    }

    // ��������, ��, ������ ī�带 �߰��� �� ��밡��
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
    // ī�� ���� ���� �߰�
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
