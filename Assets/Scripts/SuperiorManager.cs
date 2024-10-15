using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SuperiorManager : MonoBehaviour
{
    public float[] fillAmount;
    public Image[] shapeImage;
    public PlayerState.AttributeType[] topAttributes;

    private PlayerState plyerState;
    private RewardManager rewardManager;
    private SystemMessage message;

    // Start is called before the first frame update
    void Start()
    {
        plyerState = FindObjectOfType<PlayerState>();
        rewardManager = FindObjectOfType<RewardManager>();
        message = FindObjectOfType<SystemMessage>();
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

        shapeImage[0].fillAmount = (float)topTwoValues[0] / (float)9;
        fillAmount[0] = (float)topTwoValues[0] / (float)9;
        topAttributes[0] = topTwoKeys[0];

        shapeImage[1].fillAmount = (float)topTwoValues[1] / (float)9;
        fillAmount[1] = (float)topTwoValues[1] / (float)9;
        topAttributes[1] = topTwoKeys[1];

        ChangeColor(shapeImage[0], topTwoKeys[0]);
        ChangeColor(shapeImage[1], topTwoKeys[1]);
    }

    public void ChangeColor(Image halfCircle, PlayerState.AttributeType arrtibute)
    {
        switch (arrtibute)
        {
            case PlayerState.AttributeType.Fire:
                halfCircle.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                break;

            case PlayerState.AttributeType.Water:
                halfCircle.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                break;

            case PlayerState.AttributeType.Wood:
                halfCircle.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                break;

            case PlayerState.AttributeType.Earth:
                halfCircle.color = new Color(153f / 255f, 102f / 255f, 0.0f, 1.0f);
                break;

            case PlayerState.AttributeType.Lightning:
                halfCircle.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                break;

            case PlayerState.AttributeType.Wind:
                halfCircle.color = new Color(153f / 255f, 255f / 255f, 153f / 255f, 1.0f);
                break;

            case PlayerState.AttributeType.Light:
                halfCircle.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;

            case PlayerState.AttributeType.Dark:
                halfCircle.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                break;

            case PlayerState.AttributeType.Void:
                halfCircle.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                break;
        }
    }

    public void Superior()
    {
        StartCoroutine(SuperiorOn());
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
        if (fillAmount[0] == 1.0f && fillAmount[1] == 1.0f)
        {
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
}
