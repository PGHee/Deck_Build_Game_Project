using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour
{
    public int handCardNum = 0; //���� �ڵ��� ��, ���׷����Ϳ��� +, ��Ʈ�ѷ����� - 
    int handCardNumCh = 0; //�ڵ��� ���� ���ߴ��� �˾ƺ��� ���� ���

    public GameObject[] hands;

    public GameObject cardController;
    public GameObject cardGenerater;

    public float radius = 20.0f;
    public Vector2 center = new Vector2(0f, -19f);

    // Start is called before the first frame update
    void Start()
    {
        cardController = GameObject.Find("CardController");
        cardGenerater = GameObject.Find("CardGenerater");
        hands = new GameObject[10];
    }

    // Update is called once per frame
    void Update()
    {
        HandSort();
    }

    void HandSort()
    {
        if (handCardNum != handCardNumCh) // �ڵ��� ���� ���ϸ�
        {
            if (handCardNum < handCardNumCh)// �ڵ��� ���� �پ�� ���
            {
                for (int i = 0; i < 9; i++) // ��� �ڵ� ������Ʈ �������
                {
                    if (hands[i] == null && hands[i + 1] != null)
                    {
                        hands[i] = hands[i + 1];
                        hands[i + 1] = null;
                    }
                }
            }
            for (int j = 0; j < handCardNum; j++)
            {
                float xposition = (0 - (float)handCardNum)/(2.0f) + j + 0.5f;
                //float zposition = 0.0f;
                //hands[j].transform.position = new Vector3(xposition, hands[j].transform.position.y, zposition - j);
                
                // ���� �����Ŀ� x ���� �����Ͽ� y ���� ���
                float y = Mathf.Sqrt(radius * radius - xposition * xposition) + center.y;

                // ���� ��ġ�� ���ο� ��ǥ�� �̵�
                hands[j].transform.position = new Vector3((2.0f) * xposition, y - 1, transform.position.z);

                Debug.Log(hands[j].transform.position);

                hands[j].transform.rotation = Quaternion.Euler(0, 0, xposition * -3);
            }
            handCardNumCh = handCardNum;
        }
    }
}
