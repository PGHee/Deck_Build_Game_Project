using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public PlayerState playerState;
    public DynamicButtonManager dynamicButtonManager;
    public GameObject[] clearButtonsShop;

    public int playerLevel;
    public Dictionary<PlayerState.AttributeType, int> attributeMastery;

    // Start is called before the first frame update
    void Start()
    {
        
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

        for(int i = 0; i < 10; i++)
        {
            sumLevel =- attributeLevelList[i];

            if (sumLevel <= 0)
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

        for (int i = 0; i < 10; i++)    // �Ǹ��ϴ� ī�� �� ��ŭ �ݺ� ���� ����, ���� max�� 15����
        {
            int sellCardNum = RandomSelectCard();
            dynamicButtonManager.CardSpriteToButton(sellCardNum, clearButtonsShop[i]);
        }
        
    }

    public void GetClearButtonShop()
    {
        clearButtonsShop = GameObject.FindGameObjectsWithTag("ClearButton");
    }
}
