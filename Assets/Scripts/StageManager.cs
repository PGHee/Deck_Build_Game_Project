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
    }

    public void GenerateShopOpener()
    {
        GameObject shopOpener = Resources.Load<GameObject>($"Prefabs/ShopOpener");
        GameObject go = Instantiate(shopOpener);
        go.transform.position = new Vector3(5 , 0, -2);
        go.name = "shopOpener";
    }

    public void DestroyShopOpener()
    {
        Destroy(GameObject.Find("shopOpener"));
    }

}
