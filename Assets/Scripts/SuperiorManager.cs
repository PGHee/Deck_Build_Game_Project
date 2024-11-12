using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SuperiorManager : MonoBehaviour
{
    public float[] fillAmount;
    public Image[] shapeImage;
    public Image[] cardImage;
    public TextMeshProUGUI[] levelText;
    public PlayerState.AttributeType[] topAttributes;

    private PlayerState plyerState;
    private RewardManager rewardManager;
    private SystemMessage message;

    public bool superiorAvailable;

    // Start is called before the first frame update
    void Start()
    {
        plyerState = FindObjectOfType<PlayerState>();
        rewardManager = FindObjectOfType<RewardManager>();
        message = FindObjectOfType<SystemMessage>();
        superiorAvailable = true;
    }

    public void UpdateSuperior()
    {
        fillAmount = new float[2];
        topAttributes = new PlayerState.AttributeType[2];

        var topTwoKeys = plyerState.attributeMastery.OrderByDescending(pair => pair.Value)
                             .Take(2)
                             .Select(pair => pair.Key)
                             .ToList();

        var topTwoValues = plyerState.attributeMastery.OrderByDescending(pair => pair.Value)
                             .Take(2)
                             .Select(pair => pair.Value)
                             .ToList();

        //shapeImage[0].fillAmount = (float)topTwoValues[0] / (float)9;
        fillAmount[0] = (float)topTwoValues[0] / (float)9;
        topAttributes[0] = topTwoKeys[0];

        //shapeImage[1].fillAmount = (float)topTwoValues[1] / (float)9;
        fillAmount[1] = (float)topTwoValues[1] / (float)9;
        topAttributes[1] = topTwoKeys[1];

        ChangeColor(shapeImage[0], cardImage[0], topTwoKeys[0]);
        ChangeColor(shapeImage[1], cardImage[1], topTwoKeys[1]);
        levelText[0].text = "" + topTwoValues[0] + "";
        levelText[1].text = "" + topTwoValues[1] + "";

    }

    public void ChangeColor(Image halfCircle, Image cardImage, PlayerState.AttributeType arrtibute)
    {
        switch (arrtibute)
        {
            case PlayerState.AttributeType.Fire:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Fire");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Fi_19");
                break;

            case PlayerState.AttributeType.Water:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Water");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Wa_19");
                break;

            case PlayerState.AttributeType.Wood:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Tree");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Tr_19");
                break;

            case PlayerState.AttributeType.Earth:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Ground");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Gr_19");
                break;

            case PlayerState.AttributeType.Lightning:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Thunder");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Th_19");
                break;

            case PlayerState.AttributeType.Wind:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Wind");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Wi_19");
                break;

            case PlayerState.AttributeType.Light:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Light");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Li_19");
                break;

            case PlayerState.AttributeType.Dark:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Dark");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/Da_19");
                break;

            case PlayerState.AttributeType.Void:
                halfCircle.sprite = Resources.Load<Sprite>($"Image/Card/Card_Normal");
                cardImage.sprite = Resources.Load<Sprite>($"Image/Card/No_19");

                break;
        }
    }

    public void Superior()
    {
        StartCoroutine(SuperiorOn());
        StartCoroutine(SuperiorImageOn());
    }

    int Attribute2Num(PlayerState.AttributeType arrtibute)
    {
        switch (arrtibute)
        {
            case PlayerState.AttributeType.Fire:
                return 0;

            case PlayerState.AttributeType.Water:
                return 1;

            case PlayerState.AttributeType.Wood:
                return 2;

            case PlayerState.AttributeType.Earth:
                return 3;

            case PlayerState.AttributeType.Lightning:
                return 4;

            case PlayerState.AttributeType.Wind:
                return 5;

            case PlayerState.AttributeType.Light:
                return 6;

            case PlayerState.AttributeType.Dark:
                return 7;

            case PlayerState.AttributeType.Void:
                return 8;

            default:
                return 0;
        }
    }

    IEnumerator ShowSuperiorMessage()
    {
        yield return new WaitForSeconds(0.5f);
        message.ShowSystemMessage("초월 완료");
    }

    IEnumerator SuperiorOn()
    {
        yield return new WaitForSeconds(0.3f);
        if (superiorAvailable && fillAmount[0] == 1.0f && fillAmount[1] == 1.0f)
        {
            superiorAvailable = false;
            plyerState.AttributeLevelUp(topAttributes[0]);
            yield return new WaitForSeconds(0.3f);
            plyerState.AttributeLevelUp(topAttributes[1]);

            yield return new WaitForSeconds(0.3f);
            rewardManager.GetReward("Card", Attribute2Num(topAttributes[0]) * 100 + 19, 0);
            yield return new WaitForSeconds(0.3f);
            rewardManager.GetReward("Card", Attribute2Num(topAttributes[1]) * 100 + 19, 0);
            Debug.Log(topAttributes[0]);
            Debug.Log(topAttributes[1]);

            StartCoroutine(ShowSuperiorMessage());

            if(plyerState.level <= 9)
            {
                plyerState.level = 10;
                plyerState.resource = 10;
                plyerState.currentResource = 10;
                yield return new WaitForSeconds(0.3f);
                message.ShowSystemMessage("최고 레벨 달성");
            }

        }
        else
        {
            message.ShowSystemMessage("초월 불가.");
        }
    }

    IEnumerator SuperiorImageOn()
    {
        if (superiorAvailable && fillAmount[0] == 1.0f && fillAmount[1] == 1.0f)
        {
            for (int i = 0; i < 50; i++)
            {
                cardImage[0].fillAmount = (float)i / (float)50;
                cardImage[1].fillAmount = (float)i / (float)50;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
}
