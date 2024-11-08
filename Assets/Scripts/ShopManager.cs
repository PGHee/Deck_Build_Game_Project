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
    private ArtifactMountManager artifactMountManager;
    private GameDirector gameDirector;
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
        artifactMountManager = FindObjectOfType<ArtifactMountManager>();
        gameDirector = FindObjectOfType<GameDirector>();
    }

    public void RestartShop()
    {
        if (shopFirstOpen)
        {
            SetCardsToShop();
            SetArtifactsToShop();
            SetRest();
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

        List<int> attributeRatioList = new List<int>();
        for (int j = 0; j < attributeLevelList.Count; j++)
        {
            for (int k = 0; k < attributeLevelList[j]; k++)
            {
                if (j != 3) // 메탈 속성 제외
                {
                    attributeRatioList.Add(j);
                }
            }
        }

        int randomAtrribute = attributeRatioList[Random.Range(0, attributeRatioList.Count)]; // 랜덤 선택 된 속성
        Debug.Log(randomAtrribute);
        int randomGrade = Random.Range(0, attributeLevelList[randomAtrribute]) * 3 + Random.Range(0, 2);// 랜덤 선택된 등급(최대 조절 전)
        if (randomGrade > attributeLevelList[randomAtrribute] * 2 - 1) // 속성 레벨보다 등급이 높을 경우 조정
        {
            randomGrade = attributeLevelList[randomAtrribute] * 2 - Random.Range(0, 2);
        }

        if (attributeLevelList[randomAtrribute] == 10)
        {
            randomGrade = Random.Range(17, 19); // 초월을 했을 경우 17,18번 카드만 나오게 조정(임시)
        }
        Debug.Log(randomGrade);

        return randomAtrribute * 100 + randomGrade;
    }

    public void SetCardsToShop()
    {
        GetClearButtonShop();

        for (int i = 0; i < 5; i++)
        {
            int sellCardNum = RandomSelectCard();
    
            clearButtonsShop[i].GetComponent<ShopButtonManager>().cardPrefabNum = sellCardNum;
            sellCardNums[i] = sellCardNum;
            sellCardPrices[i] = ((int)(sellCardNum % 100) / 2 + 1) * (int)(5 + 5 * (gameDirector.currentStage - 1)* 0.5);

            dynamicButtonManager.CardSpriteToButton(sellCardNum, clearButtonsShop[i]); // 일러만 가져오도록 수정 필요

            GameObject cardPrefab = Resources.Load<GameObject>($"Prefabs/Card/{CardNameConverter.CardNumToCode(sellCardNum)}");
            //PriceText[i].text = "[" + cardPrefab.GetComponent<Card>().cardName + "]<br>[" + sellCardPrices[i] + " 마석]";
            AutoSizeText(PriceText[i], "[" + cardPrefab.GetComponent<Card>().cardName + "]<br>[" + sellCardPrices[i] + " 마석]");
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
                rewardManager.GetReward("Card", sellCardNums[buttonNum] + 1, sellCardPrices[buttonNum]);
                sellCardNums[buttonNum] = 0;

                UISoundManager.instance.PlaySound("Shop");
                //message.ShowSystemMessage("구매 감사합니다!");
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
        return Random.Range(1, 8);
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
            sellArtifactPrices[i] = (int)(100 + 100 * (gameDirector.currentStage - 1) * 0.5);

            //PriceText[i + 5].text = "[" + sellArtifactPrices[i] + " 마석]";
            AutoSizeText(PriceText[i + 5], "[" + sellArtifactPrices[i] + " 마석]");

            clearButtonsShop[i + 5].GetComponent<UIToolTip>().descriptionTextArtifact = artifactMountManager.GetArtifactText(sellArtifactNum - 1 );
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
                //message.ShowSystemMessage("다른것도 어떠신가요?");
                UISoundManager.instance.PlaySound("Shop");
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

    public void SetRest()
    {
        PriceText[8].text = "[25% 회복]<br>[" + (int)(25 + 25 * (gameDirector.currentStage - 1) * 0.5) + "마석]";
        PriceText[9].text = "[50% 회복]<br>[" + (int)(50 + 50 * (gameDirector.currentStage - 1) * 0.5) + "마석]";
        PriceText[10].text = "[75% 회복]<br>[" + (int)(75 + 75 * (gameDirector.currentStage - 1) * 0.5) + "마석]";
        PriceText[11].text = "[카드삭제]<br>[" + (int)(100 + 100 * (gameDirector.currentStage - 1) * 0.5) + "마석]";

    }

    public void BuyHeal(int buttonNum)
    {
        if (buyHP == true && playerState.crystal >= 25 * (buttonNum + 1))
        {
            rewardManager.GetReward("HP", 25 * (buttonNum + 1), 25 * (buttonNum + 1) * (int)(1 + 1 * (gameDirector.currentStage - 1) * 0.5));

            // 모든 힐 구매 버튼 블러
            BlurClearButtonShop(clearButtonsShop[8]);
            BlurClearButtonShop(clearButtonsShop[9]);
            BlurClearButtonShop(clearButtonsShop[10]);

            buyHP = false;
            UISoundManager.instance.PlaySound("Shop");
            //message.ShowSystemMessage("입맛엔 맞으시나요?");
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

    public void  AutoSizeText(TextMeshProUGUI textBox, string text)
    {
        textBox.text = text;
        RectTransform tooltipRectTransform = textBox.GetComponent<RectTransform>();
        tooltipRectTransform.pivot = new Vector2(0.5f, 0f); // 패널 하단 위치를 고정, 길이가 늘어나면 위쪽으로 확장

        // 텍스트 레이아웃을 즉시 갱신하여 크기를 가져옴
        LayoutRebuilder.ForceRebuildLayoutImmediate(textBox.rectTransform);

        // 텍스트의 높이에 따라 툴팁 패널의 높이를 조정
        float preferredHeight = textBox.preferredHeight;
        Vector2 newSize = new Vector2(tooltipRectTransform.sizeDelta.x, preferredHeight + 20f);
        tooltipRectTransform.sizeDelta = newSize;

        // 텍스트 RectTransform을 패널 크기에 맞춰 조정
        textBox.rectTransform.sizeDelta = new Vector2(tooltipRectTransform.sizeDelta.x - 20f, preferredHeight);
    }
}

