using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    public GameObject cardPrefab;
    int xposition = 0;
    int zposition = 0;
    public int cardNum = 0;
    public int cardNameNum = 0;
    int maxCardNum = 9;
    public GameObject handController;
    public GameObject[] cardPrefabs;
    public HandControl handControl;

    // Start is called before the first frame update
    void Start()
    {
        handController = GameObject.Find("HandController");
        cardPrefabs = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            cardPrefabs[i] = Resources.Load<GameObject>($"Prefabs/card{i}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("d"))
        {
            DrawFromDeck(0);
        }

        cardNum = handControl.handCardNum;

    }

    public void DrawFromDeck(int cardInd)
    {      
        GameObject go = Instantiate(cardPrefabs[cardInd]);
        go.transform.position = new Vector3(xposition, -4, zposition);
        go.name = "card" + cardNameNum;
        handController.GetComponent<HandControl>().hands[cardNum] = go;
        handController.GetComponent<HandControl>().handCardNum++;
        cardNameNum++;
    }
}
