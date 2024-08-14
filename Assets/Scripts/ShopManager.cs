using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public PlayerState playerState;
    public DynamicButtonManager dynamicButtonManager;
    public GameObject[] clearButtonsShop;

    public int usableCrystal;
    public int playerLevel;
    public Dictionary<PlayerState.AttributeType, int> attributeMastery;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPlayerState() // 플레이어 정보 획득
    {
        usableCrystal = playerState.crystal;
        playerLevel = playerState.level;
        attributeMastery = playerState.attributeMastery;
    } 

    public void FeedbackPlayerState()   // 플레이어 정보 반환(최신화)
    {
        playerState.crystal = usableCrystal;
    }

    public int RandomSelectCard()  // 상점에 뜰 카드 랜덤 선정
    {
        GetPlayerState();
        // 랜덤으로 생성될 카드들의 프리팹 번호 선정 
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

        for (int i = 0; i < 10; i++)    // 판매하는 카드 수 만큼 반복 범위 지정, 현재 max는 15까지
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
