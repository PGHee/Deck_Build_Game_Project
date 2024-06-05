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

    // 덱 랜덤 셔플
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
    
    //덱 목록 출력(디버깅)
    void DeckArrayPrint(int[] array)
    {
        int num = 0;
        for(int i = 0; i<array.Length; i++)
        {
            num += array[i] * (int)Mathf.Pow(10f, (float)(array.Length-i-1));
        }
        Debug.Log(num);
    }

    // 덱에서 카드 한장 뽑아 생성
    void CardDraw(int[] array)
    {
        Debug.Log(array[array.Length - 1]);

        // 핸드의 카드 수가 최대값보다 적으면
        if (cardGenerator.cardNum < 9)
        {
            //카드 생성
            cardGenerator.DrawFromDeck(array[array.Length - 1]);
        }
        else
        {
            // 생성될 카드의 번호를 graveArray로 보냄
            graveArray = Card2Grave(array[array.Length - 1]);
        }
        // 생성 or 묘지로 간 카드를 deckArray에서 삭제
        deckArray = DelCardFromDeck(deckArray, deckArray.Length - 1);
        DeckArrayPrint(deckArray);
    }

    // deckArray에서 카드 삭제
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

    // graveArray에 인덱스 추가
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
