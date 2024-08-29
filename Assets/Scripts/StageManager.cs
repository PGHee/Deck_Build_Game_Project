using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : MonoBehaviour
{
    public string[] portalRatio1;
    public string[] portalRatio2;
    public GameDirector gameDirector;

    // Start is called before the first frame update
    void Start()
    {
        portalRatio1 = new string[] { "NormalBattle" , "NormalBattle", "NormalBattle", "EliteBattle", "Village", "Event" };
        portalRatio2 = new string[] { "NormalBattle", "NormalBattle", "EliteBattle", "EliteBattle", "Village", "Event" };
    }

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            GenerateRandomPortal();
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

    public void GenerateArtifactSynthesisOpener()
    {
        GameObject artifactSynthesisOpener = Resources.Load<GameObject>($"Prefabs/ArtifactSynthesisOpener");
        GameObject go = Instantiate(artifactSynthesisOpener);
        go.transform.position = new Vector3(7, 0, -2);
        go.name = "artifactSynthesisOpener";
    }

    public void DestroyEventOpener()
    {
        Destroy(GameObject.Find("eventOpener"));
    }

    public void DestroyShopOpener()
    {
        Destroy(GameObject.Find("shopOpener"));
    }

    public void DestroyArtifactSynthesisOpener()
    {
        Destroy(GameObject.Find("artifactSynthesisOpener"));
    }

    //��Ż ����

    public void GenerateRandomPortal()
    {
        if (gameDirector.currentMap == 9)
        {
            GameObject portal_0 = Resources.Load<GameObject>($"Prefabs/BossBattlePortal");
            GameObject go0 = Instantiate(portal_0);
            go0.transform.position = new Vector3(6, 0, -2);
            go0.name = "BossBattle";
            go0.GetComponent<PortalInfo>().portalName = "BossBattle";
        }
        else
        {
            string portal1 = portalRatio1[Random.Range(0, portalRatio1.Length)];
            string portal2 = portalRatio2[Random.Range(0, portalRatio2.Length)];

            GameObject portal_1 = Resources.Load<GameObject>($"Prefabs/{portal1}Portal");
            GameObject go1 = Instantiate(portal_1);
            go1.transform.position = new Vector3(5, 0, -2);
            go1.name = portal1;
            go1.GetComponent<PortalInfo>().portalName = portal1;

            GameObject portal_2 = Resources.Load<GameObject>($"Prefabs/{portal2}Portal");
            GameObject go2 = Instantiate(portal_2);
            go2.transform.position = new Vector3(7, 0, -2);
            go2.name = portal2;
            go2.GetComponent<PortalInfo>().portalName = portal2;
        }
    }
}
