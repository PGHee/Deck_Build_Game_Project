using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class HandControl : MonoBehaviour
{
    public int handCardNum = 0; //현재 핸드의 수, 제네레이터에서 +, 컨트롤러에서 - 
    int handCardNumCh = 0; //핸드의 수가 변했는지 알아보기 위한 상수

    public GameObject[] hands;

    public GameObject cardController;
    public GameObject cardGenerater;
    public DeckManager deckManager;

    public float radius = 20.0f;
    public Vector2 center = new Vector2(0f, -19f);

    // Start is called before the first frame update
    void Start()
    {
        cardController = GameObject.Find("CardController");
        cardGenerater = GameObject.Find("CardGenerater");
        deckManager = FindObjectOfType<DeckManager>();
    }


    public void HandSort(GameObject card, bool cardAdded)
    {
        if (cardAdded)
        {
            hands = GameObject.FindGameObjectsWithTag("CardInHand");
        }
        else
        {
            hands = GameObject.FindGameObjectsWithTag("CardInHand");
            List<GameObject> handsList = new List<GameObject>(hands);
            handsList.Remove(card);
            hands = handsList.ToArray();
        }

        for (int j = 0; j < hands.Length; j++)
        {
            float xposition = (0 - (float)hands.Length) / (2.0f) + j + 0.5f;
            //float zposition = 0.0f;
            //hands[j].transform.position = new Vector3(xposition, hands[j].transform.position.y, zposition - j);

            // 원의 방정식에 x 값을 대입하여 y 값을 계산
            float y = Mathf.Sqrt(radius * radius - xposition * xposition) + center.y;

            // 현재 위치를 새로운 좌표로 이동
            hands[j].transform.position = new Vector3((2.0f) * xposition, y - 1, transform.position.z);

            hands[j].transform.rotation = Quaternion.Euler(0, 0, xposition * -3);


            Transform canvasT = hands[j].transform.Find("CardFront/Canvas");
            Canvas canvas = canvasT.gameObject.GetComponent<Canvas>();
            if (canvas != null) canvas.sortingOrder = 300 + 10 * j + 5;

            hands[j].GetComponent<SortingGroup>().sortingOrder = 300 + j * 10;
        }

        handCardNumCh = handCardNum;        
    }

    public void DiscardAllHand()
    {
        hands = GameObject.FindGameObjectsWithTag("CardInHand");
        if(hands != null)
        {
            for (int i = 0; i < hands.Length; i++)
            {
                deckManager.graveArray = deckManager.Card2Grave(int.Parse(hands[i].name));
            }

            for (int j = 0; j < hands.Length; j++)
            {
                Destroy(hands[j]);
            }
        }      
    }

    public void Dump(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject[] cardInHands = GameObject.FindGameObjectsWithTag("CardInHand");
            int randNum = Random.Range(0, cardInHands.Length);
            if (cardInHands[randNum].layer != LayerMask.NameToLayer("Ignore Raycast") && cardInHands[randNum] != null)
            {
                HandSort(cardInHands[randNum], false);
                cardInHands[randNum].tag = "Untagged";
                deckManager.graveArray = deckManager.Card2Grave(int.Parse(cardInHands[randNum].name));
                Destroy(cardInHands[randNum]);
            }
            else
            {
                i--;
            }
        }
    }
}