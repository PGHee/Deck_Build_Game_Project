using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class BattleRewardManager : MonoBehaviour
{
    public PlayerState playerState;
    public DynamicButtonManager dynamicButtonManager;
    public RewardManager rewardManager;
    public GameObject[] clearButtonsBattleReward;
    public UIBar uiBar;

    public int playerLevel;
    public Dictionary<PlayerState.AttributeType, int> attributeMastery;

    public int[] rewardCardNums;
    public int[] rewardArtifactNums;
    public int rewardCrystal;
    public int rewardEXP;

    public int battleRewardStep;
    private ArtifactMountManager artifactMountManager;
    private GameDirector gameDirector;


    // Start is called before the first frame update
    void Start()
    {
        rewardCardNums = new int[3];
        rewardArtifactNums = new int[3];
        battleRewardStep = 0;
        rewardCrystal = 0;
        artifactMountManager = FindObjectOfType<ArtifactMountManager>();
        gameDirector = FindObjectOfType<GameDirector>();
    }

    public void RestartBattleReward()
    {
        GetClearButtonBattleReward();    

        GameObject crystalAmount = GameObject.Find("CrystalAmount");
        crystalAmount.GetComponent<TextMeshProUGUI>().text = "";    // reset crystal text

        if (battleRewardStep == 0)
        {
            SetCardsToBattleReward();
        }
        else if(battleRewardStep == 1)
        {
            GameObject gameDirector = GameObject.Find("GameDirector");
            string mapName = gameDirector.GetComponent<GameDirector>().currentMapName;
            if( mapName == "BossBattle" || mapName == "EliteBattle")
            {
                SetArtifactsToBattleReward();
            }
            else
            {
                SetCrystalToBattleReward();
                battleRewardStep = 3;
            }
            
        }
        else if(battleRewardStep == 2)
        {
            SetCrystalToBattleReward();
        }
        else
        {
            battleRewardStep = 0;
            GameObject popupManager = GameObject.Find("PopupManager");
            popupManager.GetComponent<PopupManager>().ClosePopup("BattleReward");

            GameObject stageManager = GameObject.Find("StageManager");
            stageManager.GetComponent<StageManager>().GenerateRandomPortal();
        }

        foreach (GameObject button in clearButtonsBattleReward)
        {
            UnBlurClearButtonBattleReward(button);
        }
    }

    public void GetPlayerState()
    {
        playerLevel = playerState.level;
        attributeMastery = playerState.attributeMastery;
    }

    public void FeedbackPlayerState()
    {

    }

    public void GetClearButtonBattleReward()
    {
        clearButtonsBattleReward = GameObject.FindGameObjectsWithTag("ClearButton");
    }

    public void ClearButtonBattleReward(GameObject button)
    {
        Color color = button.GetComponent<Image>().color;
        color.a = 0.0f;

        button.GetComponent<Image>().color = color;
    }

    public void BlurClearButtonBattleReward(GameObject button)
    {
        Color color = button.GetComponent<Image>().color;
        color.a = 0.5f;

        button.GetComponent<Image>().color = color;
    }

    public void UnBlurClearButtonBattleReward(GameObject button)
    {
        Color color = button.GetComponent<Image>().color;
        color.a = 1.0f;

        button.GetComponent<Image>().color = color;
    }

    public int RandomSelectCard()
    {
        GetPlayerState();

        List<int> attributeLevelList = new List<int>(attributeMastery.Values);

        List<int> attributeRatioList = new List<int>();
        for (int j = 0; j < attributeLevelList.Count; j++)
        {
            for(int k = 0; k < attributeLevelList[j]; k++)
            {
                if(j != 3) // 메탈 속성 제외
                {
                    attributeRatioList.Add(j);
                }  
            }
        }

        int randomAtrribute = attributeRatioList[Random.Range(0, attributeRatioList.Count)]; // 랜덤 선택 된 속성
        Debug.Log(randomAtrribute);
        int randomGrade = Random.Range(0, attributeLevelList[randomAtrribute]) * 3 + Random.Range(0, 2);// 랜덤 선택된 등급(최대 조절 전)
        if(randomGrade > attributeLevelList[randomAtrribute] * 2 - 1) // 속성 레벨보다 등급이 높을 경우 조정
        {
            randomGrade = attributeLevelList[randomAtrribute] * 2 - Random.Range(0, 2);
        }

        if(attributeLevelList[randomAtrribute] == 10)
        {
            randomGrade = Random.Range(17, 19); // 초월을 했을 경우 17,18번 카드만 나오게 조정(임시)
        }
        Debug.Log(randomGrade);

        return randomAtrribute * 100 + randomGrade;

        //-------------------------------------------------------------------------------------------------
    }

    public int RandomSelectArtifact()
    {
        return Random.Range(1, 8);
    }

    public void SetCardsToBattleReward()
    {
        GetClearButtonBattleReward();

        for (int i = 0; i < 3; i++)
        {
            clearButtonsBattleReward[i].GetComponent<UIToolTip>().openTooltip = false;
        }

        UnClearButton(0);
        UnClearButton(1);
        UnClearButton(2);

        for (int i = 0; i < 3; i++)
        {
            int rewardCardNum = RandomSelectCard();

            clearButtonsBattleReward[i].GetComponent<BattleRewardButtonManager>().ResetNumber();

            clearButtonsBattleReward[i].GetComponent<BattleRewardButtonManager>().cardPrefabNum = rewardCardNum;
            rewardCardNums[i] = rewardCardNum;

            dynamicButtonManager.CardSpriteToButton(rewardCardNum, clearButtonsBattleReward[i]);

        }

    }

    public void SetArtifactsToBattleReward()
    {
        GetClearButtonBattleReward();

        ButtonShapeChange();

        for (int i = 0; i < 3; i++)
        {
            int rewardArtifactNum = RandomSelectArtifact();

            clearButtonsBattleReward[i].GetComponent<BattleRewardButtonManager>().ResetNumber();

            dynamicButtonManager.ArtifactSpriteToButton(rewardArtifactNum, clearButtonsBattleReward[i]);

            clearButtonsBattleReward[i].GetComponent<BattleRewardButtonManager>().artifactPrefabNum = rewardArtifactNum;
            rewardArtifactNums[i] = rewardArtifactNum;

            clearButtonsBattleReward[i].GetComponent<UIToolTip>().descriptionTextArtifact = artifactMountManager.GetArtifactText(rewardArtifactNum - 1);
            clearButtonsBattleReward[i].GetComponent<UIToolTip>().isArtifact = true;
            clearButtonsBattleReward[i].GetComponent<UIToolTip>().openTooltip = true;
        }
    }

    public void SetCrystalToBattleReward()
    {
        for (int i = 0; i < 3; i++)
        {
            clearButtonsBattleReward[i].GetComponent<BattleRewardButtonManager>().ResetNumber();
            clearButtonsBattleReward[i].GetComponent<UIToolTip>().isArtifact = false;
            clearButtonsBattleReward[i].GetComponent<UIToolTip>().openTooltip = false;
        }

        ButtonShapeReset();
        
        GameObject gameDirector = GameObject.Find("GameDirector");
        rewardCrystal = gameDirector.GetComponent<GameDirector>().currentStage * 10 * rewardCrystal;

        rewardEXP = gameDirector.GetComponent<GameDirector>().currentStage * 10 * rewardEXP;

        string mapName = gameDirector.GetComponent<GameDirector>().currentMapName;

        if (mapName == "BossBattle")
        {
            rewardCrystal = rewardCrystal * 10;
            rewardEXP = rewardEXP * 10;
        }
        else if (mapName == "EliteBattle")
        {
            rewardCrystal = rewardCrystal * 2;
            rewardEXP = rewardEXP * 2;
        }


        GameObject crystalAmount = GameObject.Find("CrystalAmount");
        crystalAmount.GetComponent<TextMeshProUGUI>().text = "[ " + rewardCrystal + " 마석 ]";

        clearButtonsBattleReward[0].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/UI/ClearBox");
        clearButtonsBattleReward[1].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Icons/UIBar_Crystal");
        clearButtonsBattleReward[2].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/UI/ClearBox");

        //ClearButton(0);
        //ClearButton(2);

    }

    public void GetBattleReward(int buttonNum)
    {
        if (battleRewardStep == 0)
        {
            rewardManager.GetReward("Card", rewardCardNums[buttonNum] + 1, 0);
        }
        else if(battleRewardStep == 1)
        {
            rewardManager.GetReward("Artifact", rewardArtifactNums[buttonNum], 0);
        }
        else
        {
            playerState.GainCrystal(rewardCrystal);
            playerState.AddExperience(rewardEXP);
            uiBar.UpdateUIBar();
        }

        battleRewardStep++;

        RestartBattleReward();

        GameObject card2des = GameObject.FindWithTag("CardInHand");

        if(card2des != null)
        {
            Destroy(card2des);
        }
    }

    public void RefuseBattleReward()
    {
        battleRewardStep++;

        RestartBattleReward();
    }

    public void ButtonShapeChange()
    {
        if(clearButtonsBattleReward != null)
        {
            foreach(var button in clearButtonsBattleReward)
            {
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 32);
                rectTransform.localEulerAngles = new Vector3(0, 0, 45);
            }
        }
    }

    public void ButtonShapeReset()
    {
        if (clearButtonsBattleReward != null)
        {
            foreach (var button in clearButtonsBattleReward)
            {
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 100);
                rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    public void ClearButton(int buttonNum)
    {
        GetClearButtonBattleReward();
         
        Color color = clearButtonsBattleReward[buttonNum].GetComponent<Image>().color;
        color.a = 0.0f;

        clearButtonsBattleReward[buttonNum].GetComponent<Image>().color = color;
        Debug.Log("is it clear?");
    }

    public void UnClearButton(int buttonNum)
    {
        GetClearButtonBattleReward();

        Color color = clearButtonsBattleReward[buttonNum].GetComponent<Image>().color;
        color.a = 255.0f;

        clearButtonsBattleReward[buttonNum].GetComponent<Image>().color = color;
    }

    public int EliteCardBonusRatio(int cardLVNum, int attributeLV)
    {
        if (gameDirector.currentMapName == "EliteBattle" || gameDirector.currentMapName == "BossBattle")
        {
            cardLVNum = cardLVNum + Random.Range(0, (attributeLV / 3));
            if (cardLVNum > attributeLV * 2)
            {
                return (attributeLV * 2 - Random.Range(0, 2));
            }
            else
            {
                return cardLVNum;
            }
        }
        else
        {
            return cardLVNum;
        }
    }
}
