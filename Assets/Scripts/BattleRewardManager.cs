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


    // Start is called before the first frame update
    void Start()
    {
        rewardCardNums = new int[3];
        rewardArtifactNums = new int[3];
        battleRewardStep = 0;
        rewardCrystal = 0;
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
        return (int)(Random.Range(0,10) * 100 + Random.Range(0, attributeLevelList[9] * 2 + 1));
    }

    public int RandomSelectArtifact()
    {
        return Random.Range(1, 4);
    }

    public void SetCardsToBattleReward()
    {
        GetClearButtonBattleReward();

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

        }
    }

    public void SetCrystalToBattleReward()
    {
        for (int i = 0; i < 3; i++)
        {
            clearButtonsBattleReward[i].GetComponent<BattleRewardButtonManager>().ResetNumber();
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
        crystalAmount.GetComponent<TextMeshProUGUI>().text = "+" + rewardCrystal + " crystal";

        clearButtonsBattleReward[0].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/UI/amethyst");
        clearButtonsBattleReward[1].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/UI/amethyst");
        clearButtonsBattleReward[2].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/UI/amethyst");
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
}
