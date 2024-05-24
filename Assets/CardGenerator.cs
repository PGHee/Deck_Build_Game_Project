using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    public GameObject cardPrefab;
    int xposition = -6;
    int zposition = 0;
    int cardNum = 0;
    int maxCardNum = 10;

    // Start is called before the first frame update
    void Start()
    {
       


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c") && cardNum < maxCardNum)
        {
            GameObject go = Instantiate(cardPrefab);
            go.transform.position = new Vector3(xposition, -4, zposition);
            xposition += 1;
            zposition -= 1;
            go.name = "card" +cardNum;
            go.GetComponent<CardInfo>().cardName = "card" + cardNum;
            go.GetComponent<CardInfo>().cardNum = cardNum;
            go.GetComponent<CardInfo>().cardDamage = 10;
            cardNum++;
        }


    }
}
