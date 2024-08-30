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

    public int playerLevel;
    public Dictionary<PlayerState.AttributeType, int> attributeMastery;

    public int[] rewardCardNums;
    public int[] rewardArtifactNums;
    public int rewardCrystal;

    public int battleRewardStep;

    // Start is called before the first frame update
    void Start()
    {
        rewardCardNums = new int[3];
        rewardArtifactNums = new int[3];
        battleRewardStep = 0;
        rewardCrystal = 100;
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
                return (int)(i * 100 + Random.Range(0, attributeLevelList[i] * 2));
            }
        }

        return (int)(9 * 100 + Random.Range(0, attributeLevelList[9] * 2));
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

            dynamicButtonManager.CardSpriteToButton(rewardCardNum, clearButtonsBattleReward[i]); // 일러만 가져오도록 수정 필요
        }

    }

    public void SetArtifactsToBattleReward()
    {
        GetClearButtonBattleReward();

        for (int i = 0; i < 3; i++)
        {
            int rewardArtifactNum = RandomSelectArtifact();

            clearButtonsBattleReward[i].GetComponent<BattleRewardButtonManager>().ResetNumber();

            dynamicButtonManager.ArtifactSpriteToButton(rewardArtifactNum, clearButtonsBattleReward[i]); // 일러만 가져오도록 수정 필요

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
        
        GameObject gameDirector = GameObject.Find("GameDirector");
        rewardCrystal = gameDirector.GetComponent<GameDirector>().currentStage * 10 * rewardCrystal;


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
            rewardManager.GetReward("Card", rewardCardNums[buttonNum], 0);
        }
        else if(battleRewardStep == 1)
        {
            rewardManager.GetReward("Artifact", rewardArtifactNums[buttonNum], 0);
        }
        else
        {
            playerState.crystal = playerState.crystal + rewardCrystal;
        }

        battleRewardStep++;

        RestartBattleReward();
    }

    public void RefuseBattleReward()
    {
        battleRewardStep++;

        RestartBattleReward();
    }
}
