using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControl : MonoBehaviour // CardController에 적용
{

    // 드래그가 가능한 상태인지 확인하는 bool 변수
    public bool draggable;
    public Vector3 sposition;
    public bool dragOn;
    public GameObject dragCard;
    public GameObject targetMob;
    Collider2D s_collider;
    float speedx = 0;
    float speedy = 0;
    GameObject movingCard;

    void Start()
    {

    }


    // 매 프레임 별 호출되는 메소드
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //마우스 클릭 시
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // 화면 상의 마우스 위치

            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);    //오브젝트 히트 검사

            if (hit.collider != null)   // 히트 성공 시
            {
                dragOn = true;                          // 드래그 사용중
                dragCard = hit.transform.gameObject;    // 히트 된 오브젝트 
                Debug.Log(dragCard.name);               // 히트 된 오브젝트 이름 로그               
                s_collider = dragCard.GetComponent<Collider2D>();   // 히트 된 오브젝트의 콜라이더(카드가 여러 오브젝트의 묶음이라 구성요소의 좌표를 받아오는걸 방지) 
                sposition = new Vector3(s_collider.bounds.center.x, s_collider.bounds.center.y, dragCard.transform.position.z); // 드래그가 끝나면 카드가 돌아올 시작 위치
                    // z좌표가 카드의 z좌표인 이유는 돌아온 후 다른 카드와 겹쳐 보이는걸 방지하기 위해서 
                Debug.Log(sposition);   // 시작위치 로그
            }
        }

        if (dragOn) // 드래그 중
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // 실시간 마우스 위치

            // 카드의 중간 이동, 카드의 z좌표인 이유는 움직일 때 다른 카드와 겹쳐 보이는걸 방지
            float pox = 0 - dragCard.transform.position.x;
            float poy = 0 - dragCard.transform.position.y;
            float poxy = Mathf.Abs(pox) + Mathf.Abs(poy);
            float speed = 20.0f;
            if (pox < 0.2 && poy < 0.2)
            {
                speed = 0.0f;
            }
            speedx = pox / (poxy) * speed;

            dragCard.transform.Translate(speedx * Time.deltaTime, 0, 0);

            speedy = poy / (poxy) * speed;

            dragCard.transform.Translate(0, speedy * Time.deltaTime, 0);
        }

        if (Input.GetMouseButtonUp(0))  // 마우스를 때면
        {
            dragOn = false;             // 드래그 사용중 X
            dragCard.transform.position = sposition;    // 카드를 시작 위치로

            Vector2 pos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Debug.Log(pos2);
            RaycastHit2D hit2 = Physics2D.Raycast(pos2, Vector2.zero, 0f);

            Debug.DrawRay(pos2, transform.forward * 10, Color.red, 0f);
            Debug.Log(hit2.transform.position);

            if (hit2.collider != null) // 오브젝트 검출
            {
                targetMob = hit2.collider.gameObject;
                Debug.Log(targetMob.name);
            }

            if (targetMob.GetComponent<monsterControl>().mobHP > 0) // 오브젝트가 몬스터이면
            {
                targetMob.GetComponent<monsterControl>().mobHP -= dragCard.GetComponent<CardInfo>().cardDamage; // 데미지 적용

                cardmove2empty(dragCard); // 핸드 카드 정렬

                Destroy(dragCard); //사용된 카드 제거
            }

            

        }


        
    }
    void cardmove2empty(GameObject movedCard)
    {
        Vector3 pos = movedCard.transform.position;
        int fornum = movedCard.GetComponent<CardInfo>().cardNum;

        for (int i = 0; i < (10 - fornum); i++)
        {
            Vector2 posRay = new Vector2(pos.x + i + 0.1f, pos.y);
            RaycastHit2D hit = Physics2D.Raycast(posRay , Vector2.zero, 0f);

            if (hit.collider != null)
            {
                movingCard = hit.collider.gameObject;
                if (movingCard.transform.position.x > pos.x + i)
                {
                    movingCard.transform.position = new Vector3(pos.x + i, pos.y, pos.z - i);
                }       
            }
            else 
            {
                break;
            }
        }

        

    }

   


}