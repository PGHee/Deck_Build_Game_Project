using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControl : MonoBehaviour // CardController�� ����
{

    // �巡�װ� ������ �������� Ȯ���ϴ� bool ����
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


    // �� ������ �� ȣ��Ǵ� �޼ҵ�
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //���콺 Ŭ�� ��
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // ȭ�� ���� ���콺 ��ġ

            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);    //������Ʈ ��Ʈ �˻�

            if (hit.collider != null)   // ��Ʈ ���� ��
            {
                dragOn = true;                          // �巡�� �����
                dragCard = hit.transform.gameObject;    // ��Ʈ �� ������Ʈ 
                Debug.Log(dragCard.name);               // ��Ʈ �� ������Ʈ �̸� �α�               
                s_collider = dragCard.GetComponent<Collider2D>();   // ��Ʈ �� ������Ʈ�� �ݶ��̴�(ī�尡 ���� ������Ʈ�� �����̶� ��������� ��ǥ�� �޾ƿ��°� ����) 
                sposition = new Vector3(s_collider.bounds.center.x, s_collider.bounds.center.y, dragCard.transform.position.z); // �巡�װ� ������ ī�尡 ���ƿ� ���� ��ġ
                    // z��ǥ�� ī���� z��ǥ�� ������ ���ƿ� �� �ٸ� ī��� ���� ���̴°� �����ϱ� ���ؼ� 
                Debug.Log(sposition);   // ������ġ �α�
            }
        }

        if (dragOn) // �巡�� ��
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);  // �ǽð� ���콺 ��ġ

            // ī���� �߰� �̵�, ī���� z��ǥ�� ������ ������ �� �ٸ� ī��� ���� ���̴°� ����
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

        if (Input.GetMouseButtonUp(0))  // ���콺�� ����
        {
            dragOn = false;             // �巡�� ����� X
            dragCard.transform.position = sposition;    // ī�带 ���� ��ġ��

            Vector2 pos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Debug.Log(pos2);
            RaycastHit2D hit2 = Physics2D.Raycast(pos2, Vector2.zero, 0f);

            Debug.DrawRay(pos2, transform.forward * 10, Color.red, 0f);
            Debug.Log(hit2.transform.position);

            if (hit2.collider != null) // ������Ʈ ����
            {
                targetMob = hit2.collider.gameObject;
                Debug.Log(targetMob.name);
            }

            if (targetMob.GetComponent<monsterControl>().mobHP > 0) // ������Ʈ�� �����̸�
            {
                targetMob.GetComponent<monsterControl>().mobHP -= dragCard.GetComponent<CardInfo>().cardDamage; // ������ ����

                cardmove2empty(dragCard); // �ڵ� ī�� ����

                Destroy(dragCard); //���� ī�� ����
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