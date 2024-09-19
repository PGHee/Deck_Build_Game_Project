using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CardGenerator : MonoBehaviour
{
    public GameObject cardPrefab;
    int xposition = 0;
    int zposition = 0;
    public int cardNum = 0;
    public int cardNameNum = 0;
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
            cardPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Card/Fi_{i + 1}");
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
        GameObject go = Instantiate(Resources.Load<GameObject>($"Prefabs/Card/{CardNameConverter.CardNumToCode(cardInd - 1)}"));
        GameObject parentCardObject = GameObject.Find("Cards");
        go.transform.SetParent(parentCardObject.transform, false);
        go.transform.localPosition = new Vector3(xposition, -4, zposition);
        go.name = "" + cardInd;
        //handController.GetComponent<HandControl>().hands[cardNum] = go;
        handController.GetComponent<HandControl>().handCardNum++;
        cardNameNum++;
    }
}
