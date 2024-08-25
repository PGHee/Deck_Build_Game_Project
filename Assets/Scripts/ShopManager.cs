using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public PlayerState playerState;
    public DynamicButtonManager dynamicButtonManager;
    public RewardManager rewardManager;
    public GameObject[] clearButtonsShop;

    public int playerLevel;
    public Dictionary<PlayerState.AttributeType, int> attributeMastery;

    public int[] sellCardNums;
    public int[] sellCardPrices;

    public int[] sellArtifactNums;
    public int[] sellArtifactPrices;

    // Start is called before the first frame update
    void Start()
    {
        sellCardNums = new int[5];
        sellCardPrices = new int[5];
        sellArtifactNums = new int[3];
        sellArtifactPrices = new int[3];
    }

    public void GetPlayerState() // �÷��̾� ���� ȹ��
    {
        playerLevel = playerState.level;
        attributeMastery = playerState.attributeMastery;
    } 

    public void FeedbackPlayerState()   // �÷��̾� ���� ��ȯ(�ֽ�ȭ)
    {

    }

    public int RandomSelectCard()  // ������ �� ī�� ���� ����
    {
        GetPlayerState();
        // �������� ������ ī����� ������ ��ȣ ���� 
        List<int> attributeLevelList = new List<int>(attributeMastery.Values);

        int sumLevel = attributeLevelList.Sum();
        int randomNum = Random.Range(1, sumLevel);
        Debug.Log("randomNum:" + randomNum + "");

        for(int i = 0; i < 10; i++)
        {
            randomNum = randomNum - attributeLevelList[i];

            if (randomNum <= 0)
            {
                return (int)(i * 100 + Random.Range(0, attributeLevelList[i] * 2));
                break;
            }
        }

        return (int)(9 * 100 + Random.Range(0, attributeLevelList[9] * 2));
    }

    public void SetCardsToShop()
    {
        GetClearButtonShop();

        for (int i = 0; i < 5; i++)    // �Ǹ��ϴ� ī�� �� ��ŭ �ݺ� ���� ����, ���� max�� 15����
        {
            int sellCardNum = RandomSelectCard();
            dynamicButtonManager.CardSpriteToButton(sellCardNum, clearButtonsShop[i]); // 일러만 가져오도록 수정 필요

            clearButtonsShop[i].GetComponent<ShopButtonManager>().cardPrefabNum = sellCardNum;
            sellCardNums[i] = sellCardNum;
            sellCardPrices[i] = ((int)(sellCardNum % 100) / 2 + 1) * 5; 
        }
        
    }

    public void GetClearButtonShop()
    {
        clearButtonsShop = GameObject.FindGameObjectsWithTag("ClearButton");
    }

    public void BlurClearButtonShop(GameObject button)
    {
        Color color = button.GetComponent<Image>().color;
        color.a = 0.5f;

        button.GetComponent<Image>().color = color;
    }

    public void BuyCard(int buttonNum)
    {
        if (sellCardNums[buttonNum] > 0)
        {
            rewardManager.GetReward("Card", sellCardNums[buttonNum], sellCardPrices[buttonNum]);
            sellCardNums[buttonNum] = 0;
            BlurClearButtonShop(clearButtonsShop[buttonNum]);
        }
        else
        {
            Debug.Log("You already bought this card");
        }    
    }
}
