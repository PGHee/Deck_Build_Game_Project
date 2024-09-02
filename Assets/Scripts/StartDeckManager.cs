using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class StartDeckManager : MonoBehaviour
{
    public DeckManager deckManager;
    public RewardManager rewardManager;

    public int[] chosenAttributeNums;
    public GameObject[] clearButtonsStartDeck;
    public RectTransform parentObject;

    // Start is called before the first frame update
    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        rewardManager = FindObjectOfType<RewardManager>();
    }

    public void GetClearButtonStartDeck()
    {
        //clearButtonsStartDeck = GameObject.FindGameObjectsWithTag("ClearButton");
    }

    public void ChooseAttribute(int buttonNum)
    {
        DeleteLines();

        if (chosenAttributeNums.Contains(buttonNum)) // 이미 선택된 속성이면 해제
        {
            List<int> tempList = new List<int>();

            foreach (int number in chosenAttributeNums)
            {
                if (number != buttonNum)
                {
                    tempList.Add(number);
                }
            }

            chosenAttributeNums = tempList.ToArray();
        }
        else if(chosenAttributeNums.Length < 3)
        {
            List<int> tempListp = new List<int>();

            foreach (int number in chosenAttributeNums)
            {
                tempListp.Add(number);
            }
            tempListp.Add(buttonNum);

            chosenAttributeNums = tempListp.ToArray();
        }
        else
        {
            Debug.Log("already 3 attribute has choosen");
        }
        GetClearButtonStartDeck();

        for (int i = 0; i < chosenAttributeNums.Length * 2 - 3; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Line_UI"), GetMiddle(clearButtonsStartDeck[chosenAttributeNums[i]], clearButtonsStartDeck[chosenAttributeNums[(i + 1) % 3]]), Quaternion.identity, parentObject);

            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = parentObject.InverseTransformPoint(GetMiddle(clearButtonsStartDeck[chosenAttributeNums[i]], clearButtonsStartDeck[chosenAttributeNums[(i + 1) % 3]]));
            rectTransform.sizeDelta = new Vector2(GetDistance(clearButtonsStartDeck[chosenAttributeNums[i]], clearButtonsStartDeck[chosenAttributeNums[(i + 1) % 3]]), rectTransform.sizeDelta.y); // 가로 길이를 거리로 설정
            rectTransform.rotation = Quaternion.Euler(0, 0, GetAngle(clearButtonsStartDeck[chosenAttributeNums[i]], clearButtonsStartDeck[chosenAttributeNums[(i + 1) % 3]])); // 각도 설정
        }

    }

    public float GetDistance(GameObject bt1, GameObject bt2)
    {
        Vector3 pos1 = bt1.GetComponent<RectTransform>().position;
        Vector3 pos2 = bt2.GetComponent<RectTransform>().position;

        float distance = Vector3.Distance(pos1, pos2);
        
        return distance;
    }

    public float GetAngle(GameObject bt1, GameObject bt2)
    {
        Vector3 pos1 = bt1.GetComponent<RectTransform>().position;
        Vector3 pos2 = bt2.GetComponent<RectTransform>().position;

        Vector3 direction = pos2 - pos1;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        return angle;
    }

    public Vector3 GetMiddle(GameObject bt1, GameObject bt2)
    {
        Vector3 pos1 = bt1.GetComponent<RectTransform>().position;
        Vector3 pos2 = bt2.GetComponent<RectTransform>().position;

        Vector3 midpoint = (pos1 + pos2) / 2;

        return midpoint;
    }

    public void DeleteLines()
    {
        GameObject[] Line_UIs = GameObject.FindGameObjectsWithTag("Line_UI");

        foreach( GameObject line in Line_UIs)
        {
            Destroy(line);
        }
    }

    public void MakeStartDeck()
    {
        if (chosenAttributeNums.Length == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                rewardManager.GetReward("Card", chosenAttributeNums[i] * 100 + 1, 0);
                rewardManager.GetReward("Card", chosenAttributeNums[i] * 100 + 2, 0);
            }
        }
        else
        {
            Debug.Log("Choose 3 Attribute");
        }
        
    }


}
