using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour
{
    public int handCardNum = 0; //현재 핸드의 수, 제네레이터에서 +, 컨트롤러에서 - 
    int handCardNumCh = 0; //핸드의 수가 변했는지 알아보기 위한 상수

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
        if (handCardNum != handCardNumCh) // 핸드의 수가 변하면
        {
            if (handCardNum < handCardNumCh)// 핸드의 수가 줄어든 경우
            {
                for (int i = 0; i < 9; i++) // 모든 핸드 오브젝트 순서대로
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
                
                // 원의 방정식에 x 값을 대입하여 y 값을 계산
                float y = Mathf.Sqrt(radius * radius - xposition * xposition) + center.y;

                // 현재 위치를 새로운 좌표로 이동
                hands[j].transform.position = new Vector3((2.0f) * xposition, y - 1, transform.position.z);

                Debug.Log(hands[j].transform.position);

                hands[j].transform.rotation = Quaternion.Euler(0, 0, xposition * -3);
            }
            handCardNumCh = handCardNum;
        }
    }
}
