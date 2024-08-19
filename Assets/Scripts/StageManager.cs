using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            GenerateShopOpener();
        }

        if (Input.GetKeyDown("v"))
        {
            GenerateEventOpener();
        }
    }

    public void GenerateShopOpener()
    {
        GameObject shopOpener = Resources.Load<GameObject>($"Prefabs/ShopOpener");
        GameObject go = Instantiate(shopOpener);
        go.transform.position = new Vector3(5 , 0, -2);
        go.name = "shopOpener";
    }

    public void GenerateEventOpener()
    {
        GameObject eventOpener = Resources.Load<GameObject>($"Prefabs/EventOpener");
        GameObject go = Instantiate(eventOpener);
        go.transform.position = new Vector3(5, 0, -2);
        go.name = "eventOpener";
    }

    public void DestroyEventOpener()
    {
        Destroy(GameObject.Find("eventOpener"));
    }

    public void DestroyShopOpener()
    {
        Destroy(GameObject.Find("shopOpener"));
    }

}
