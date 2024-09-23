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
    private SystemMessage message;
    public GameObject[] clearButtonsShop;

    public int playerLevel;
    public Dictionary<PlayerState.AttributeType, int> attributeMastery;

    public int[] sellCardNums;
    public int[] sellCardPrices;

    public int[] sellArtifactNums;
    public int[] sellArtifactPrices;

    public bool buyHP;
    public bool shopFirstOpen;

    public TextMeshProUGUI[] PriceText;

    // Start is called before the first frame update
    void Start()
    {
        sellCardNums = new int[5];
        sellCardPrices = new int[5];
        sellArtifactNums = new int[3];
        sellArtifactPrices = new int[3];
        buyHP = false;
        shopFirstOpen = true;
        message = FindObjectOfType<SystemMessage>();
    }

    public void RestartShop()
    {
        if (shopFirstOpen)
        {
            SetCardsToShop();
            SetArtifactsToShop();
            buyHP = true;

            GetClearButtonShop();
            foreach (GameObject button in clearButtonsShop)
            {
                UnBlurClearButtonShop(button);
            }

            shopFirstOpen = false;
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

        for (int i = 0; i < 10; i++)
        {
            randomNum = randomNum - attributeLevelList[i];

            if (randomNum <= 0)
            {
                if (attributeLevelList[i] == 10)
                {
                    return (int)(i * 100 + Random.Range(0, 9 * 2 + 1));
                }
                else
                {
                    return (int)(i * 100 + Random.Range(0, attributeLevelList[i] * 2 + 1));
                }
            }
        }
        return (int)(Random.Range(0, 10) * 100 + Random.Range(0, attributeLevelList[9] * 2 + 1));
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

            if (sellCardPrices[buttonNum] <= playerState.crystal)
            {
                rewardManager.GetReward("Card", sellCardNums[buttonNum], sellCardPrices[buttonNum]);
                sellCardNums[buttonNum] = 0;

                message.ShowSystemMessage("구매 감사합니다!");
            }
            else
            {
                message.ShowSystemMessage("지갑이 얇아 보이는걸?");
            }               
        }
        else
        {
            message.ShowSystemMessage("이런! 품절이야");
        }

        if (sellCardNums[buttonNum] == 0)
        {
            BlurClearButtonShop(clearButtonsShop[buttonNum]);
        }       
    }

    public int RandomSelectArtifact()
    {
        return Random.Range(1, 4);
    }

    public void SetArtifactsToShop()
    {
        GetClearButtonShop();

        for (int i = 0; i < 7; i++)    // �Ǹ��ϴ� ī�� �� ��ŭ �ݺ� ���� ����, ���� max�� 15����
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
            if (sellArtifactPrices[buttonNum] <= playerState.crystal)
            {
                rewardManager.GetReward("Artifact", sellArtifactNums[buttonNum], sellArtifactPrices[buttonNum]);
                sellArtifactNums[buttonNum] = 0;
                message.ShowSystemMessage("다른것도 어떠신가요?");
            }
            else
            {
                message.ShowSystemMessage("흠....");
            }                    
        }
        else
        {
            message.ShowSystemMessage("그건 품절이야");
        }

        if (sellArtifactNums[buttonNum] == 0)
        {
            BlurClearButtonShop(clearButtonsShop[buttonNum + 5]); //아티팩트 버튼은 5번부터 시작함
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

            message.ShowSystemMessage("입맛엔 맞으시나요?");
        }
        else
        {
            message.ShowSystemMessage("재고가 없어");
        }
    }

    public void trueShopFirstOpen()
    {
        shopFirstOpen = true;
    }
}

