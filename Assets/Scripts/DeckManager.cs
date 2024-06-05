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

    // Start is called before the first frame update
    void Start()
    {
        deckArray = new int[] {0,0,0,0,1,1,1,1,2,2,2,2};
        DeckRandomSuffle(deckArray);
        DeckArrayPrint(deckArray);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            CardDraw(deckArray);
        }

        if (Input.GetKeyDown("a"))
        {
            deckArray = DelCardFromDeck(deckArray, deckArray.Length - 1);
            DeckArrayPrint(deckArray);
        }
    }

    // �� ���� ����
    void DeckRandomSuffle(int[] array) 
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
    void DeckArrayPrint(int[] array)
    {
        int num = 0;
        for(int i = 0; i<array.Length; i++)
        {
            num += array[i] * (int)Mathf.Pow(10f, (float)(array.Length-i-1));
        }
        Debug.Log(num);
    }

    // ������ ī�� ���� �̾� ����
    void CardDraw(int[] array)
    {
        Debug.Log(array[array.Length - 1]);

        // �ڵ��� ī�� ���� �ִ밪���� ������
        if (cardGenerator.cardNum < 9)
        {
            //ī�� ����
            cardGenerator.DrawFromDeck(array[array.Length - 1]);
        }
        else
        {
            // ������ ī���� ��ȣ�� graveArray�� ����
            graveArray = Card2Grave(array[array.Length - 1]);
        }
        // ���� or ������ �� ī�带 deckArray���� ����
        deckArray = DelCardFromDeck(deckArray, deckArray.Length - 1);
        DeckArrayPrint(deckArray);
    }

    // deckArray���� ī�� ����
    int[] DelCardFromDeck(int[] array, int ind)
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
    int[] Card2Grave (int ind)
    {
        int[] dest = new int[graveArray.Length + 1];
        for (int i = 0, j = 0; i < graveArray.Length; i++)
        {
            dest[j++] = graveArray[i];
        }
        dest[dest.Length - 1] = ind;
        return dest;
    }
}
