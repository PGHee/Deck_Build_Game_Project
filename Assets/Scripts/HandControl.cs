using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HandControl : MonoBehaviour
{
    public int handCardNum = 0; //���� �ڵ��� ��, ���׷����Ϳ��� +, ��Ʈ�ѷ����� - 
    int handCardNumCh = 0; //�ڵ��� ���� ���ߴ��� �˾ƺ��� ���� ���

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

            // ���� �����Ŀ� x ���� �����Ͽ� y ���� ���
            float y = Mathf.Sqrt(radius * radius - xposition * xposition) + center.y;

            // ���� ��ġ�� ���ο� ��ǥ�� �̵�
            hands[j].transform.position = new Vector3((2.0f) * xposition, y - 1, transform.position.z);

            hands[j].transform.rotation = Quaternion.Euler(0, 0, xposition * -3);
        }

        handCardNumCh = handCardNum;        
    }

    public void DiscardAllHand()
    {
        hands = GameObject.FindGameObjectsWithTag("CardInHand");
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