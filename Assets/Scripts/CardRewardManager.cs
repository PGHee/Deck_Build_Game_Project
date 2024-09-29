using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRewardManager : MonoBehaviour
{

    public GameObject[] cardPrefabs;
    public int[] cardPrefabNum;
    public Transform cardPosition1;
    public Transform cardPosition2;
    public Transform cardPosition3;
    int[] playerElements = new int[] { 0, 0, 0 };
    int[] playerLevels = new int[] { 1, 1, 1, 2, 2, 3 };
    public DeckManager deckManager;

    void randomRewardCards(int[] elements, int[] levels)
    {
        cardPrefabs = new GameObject[3];
        cardPrefabNum = new int[3];

        for (int n = 0; n < 3; n++)
        {
            int randomIndexE = Random.Range(0, elements.Length);
            int randomIndexL = Random.Range(0, levels.Length);
            int randomIndex = playerElements[randomIndexE] * 10 + playerLevels[randomIndexL];
            cardPrefabNum[n] = randomIndex;
        }

        for (int i = 0; i < 3; i++)
        {
            cardPrefabs[i] = Resources.Load<GameObject>($"Prefabs/CardPrefab{cardPrefabNum[i]}");
        }
    }

    void spawnRewardCards()
    {
        GameObject go1 = Instantiate(cardPrefabs[0], cardPosition1);
        GameObject go2 = Instantiate(cardPrefabs[1], cardPosition2);
        GameObject go3 = Instantiate(cardPrefabs[2], cardPosition3);

        GameObject[] cards = new GameObject[3];
        cards = GameObject.FindGameObjectsWithTag("CardInHand");

        for (int i = 0; i < 3; i++)
        {
            cards[i].layer = 6;
        }
    }

    public void generateCardReward()
    {
        randomRewardCards(playerElements, playerLevels);
        spawnRewardCards();
    }

    public void DeleteRewardCards()
    {
        GameObject[] cardsD = new GameObject[13];
        cardsD = GameObject.FindGameObjectsWithTag("CardInHand");

        for (int i = 0; i < 13; i++)
        {
            if (cardsD[i] != null)
            {
                Destroy(cardsD[i]);
            }           
        }
    }
}
