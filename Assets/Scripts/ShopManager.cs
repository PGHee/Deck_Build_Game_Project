using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

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

    public bool buyHP;

    public TextMeshProUGUI[] PriceText;

    // Start is called before the first frame update
    void Start()
    {
        sellCardNums = new int[5];
        sellCardPrices = new int[5];
        sellArtifactNums = new int[3];
        sellArtifactPrices = new int[3];
        buyHP = false;
    }

    public void RestartShop()
    {
        SetCardsToShop();
        SetArtifactsToShop();
        buyHP = true;

        GetClearButtonShop();
        foreach(GameObject button in clearButtonsShop)
        {
            UnBlurClearButtonShop(button);
        }
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
            }
        }

        return (int)(9 * 100 + Random.Range(0, attributeLevelList[9] * 2));
    }

    public void SetCardsToShop()
    {
        GetClearButtonShop();

        for (int i = 0; i < 5; i++)
        {
            int sellCardNum = RandomSelectCard();
    
            clearButtonsShop[i].GetComponent<ShopButtonManager>().cardPrefabNum = sellCardNum;
            sellCardNums[i] = sellCardNum;
            sellCardPrices[i] = ((int)(sellCardNum % 100) / 2 + 1) * 5;

            dynamicButtonManager.CardSpriteToButton(sellCardNum, clearButtonsShop[i]); // 일러만 가져오도록 수정 필요
            PriceText[i].text = "[" + sellCardNum + " / " + sellCardPrices[i] + "]";
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

    public void UnBlurClearButtonShop(GameObject button)
    {
        Color color = button.GetComponent<Image>().color;
        color.a = 1.0f;

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

    public int RandomSelectArtifact()
    {
        return Random.Range(1, 4);
    }

    public void SetArtifactsToShop()
    {
        GetClearButtonShop();

        for (int i = 0; i < 3; i++)    // �Ǹ��ϴ� ī�� �� ��ŭ �ݺ� ���� ����, ���� max�� 15����
        {
            int sellArtifactNum = RandomSelectArtifact();
            dynamicButtonManager.ArtifactSpriteToButton(sellArtifactNum, clearButtonsShop[i + 5]); // 일러만 가져오도록 수정 필요

            clearButtonsShop[i + 5].GetComponent<ShopButtonManager>().artifactPrefabNum = sellArtifactNum;
            sellArtifactNums[i] = sellArtifactNum;
            sellArtifactPrices[i] = 100;

            PriceText[i + 5].text = "[" + sellArtifactNum + " / " + sellArtifactPrices[i] + "]";
        }
    }

    public void BuyArtifact(int buttonNum)
    {
        if (sellArtifactNums[buttonNum] > 0)
        {
            rewardManager.GetReward("Artifact", sellArtifactNums[buttonNum], sellArtifactPrices[buttonNum]);
            sellArtifactNums[buttonNum] = 0;
            BlurClearButtonShop(clearButtonsShop[buttonNum + 5]); //아티팩트 버튼은 5번부터 시작함
        }
        else
        {
            Debug.Log("You already bought this Artifact");
        }
    }

    public void BuyHeal(int buttonNum)
    {
        if (buyHP == true && playerState.crystal >= 25 * (buttonNum + 1))
        {
            rewardManager.GetReward("HP", 25 * (buttonNum + 1), 25 * (buttonNum + 1));

            // 모든 힐 구매 버튼 블러
            BlurClearButtonShop(clearButtonsShop[8]);
            BlurClearButtonShop(clearButtonsShop[9]);
            BlurClearButtonShop(clearButtonsShop[10]);

            buyHP = false;
        }
        else
        {
            Debug.Log("HP Unable");
        }
    }
}

