using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour // ��Ƽ��Ʈ �Ŵ��� ������Ʈ�� ����, ���� ��Ƽ��Ʈ�� ������ ó��
{
    public DeckManager deckManager;
    public GameObject player;
    public PlayerState playerState;
    public GameObject artifact;
    public Actions actions;

    public bool artifactReady;
    public bool artifactDamageReady;
    public enum ActiveEffect { Heal, Shiled, Draw, Damage}
    public enum PassiveEffect 
    { 
        IncreaseDamage,
        AdditionalStrike, 
        Poison, 
        LifeSteal, 
        ReduceDamage, 
        IncreaseMaxHP, 
        IncreaseManaGain, 
        IncreaseMasteryGain
    }

    public ActiveEffect activeEffect;
    public int activeCoef;
    public List<EffectType> passiveEffectList;
    public List<float> passiveCoefList;
    public int artifactCost;

    public GameObject[] artifactPrefabs;
    public int[] artifactInvenList;
    public Dictionary<int[], int> artifactSynthesizeDict;


    // Start is called before the first frame update
    void Start()
    {
        artifactPrefabs = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            artifactPrefabs[i] = Resources.Load<GameObject>($"Prefabs/artifact{i + 1}");
        }

        artifactSynthesizeDict = new Dictionary<int[], int>();

        artifactSynthesizeDict.Add(new int[] {1, 2}, 3);

        artifactDamageReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (artifact)  // ��Ƽ��Ʈ ������Ʈ�� ���� ���� ��ó�� üũ
        {
            ArtifactBlurONOFF(); 
        }

        if (artifactDamageReady)
        {
            if (Input.GetMouseButtonUp(0) && artifactReady)
            {
                // ���콺 Ŭ�� ��ġ�� ���� ��ǥ�� ��ȯ
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // �ش� ��ġ�� ����ĳ��Ʈ �߻�
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                // ������Ʈ�� �����Ǿ����� Ȯ��
                if (hit.collider != null)
                {
                    // ������ ������Ʈ�� �̸� ���
                    Debug.Log("Clicked on object: " + hit.collider.name);

                    if (hit.collider.CompareTag("Monster"))
                    {
                        actions.DealMultipleHits(hit.collider.gameObject, activeCoef, 1, null, PlayerState.AttributeType.Fire);
                        artifactReady = false;
                        CostSpend();
                        Debug.Log("Target damaged");
                    }
                    else
                    {
                        Debug.Log(" Wrong target!!");
                        artifactDamageReady = false;
                        artifactReady = true;
                    }
                }
                else
                {
                    Debug.Log("No target Detected");
                    artifactDamageReady = false;
                    artifactReady = true;
                }
            }
            else
            {
                //artifactDamageReady = false;
                //artifactReady = true;
            }           
        }
    }

    public void GenerateArtifact(int artifactNum)
    {
        GameObject go = Instantiate(artifactPrefabs[artifactNum - 1]);
        go.transform.position = new Vector3(-7, 3, 1);
        go.name = "Artifact";

        GetArtifactInfo();
    }

    public void GetArtifactInfo() // ����� ��Ƽ��Ʈ�� ������ �ҷ���
    {
        artifact = GameObject.FindWithTag("Artifact");
        activeEffect = artifact.GetComponent<ArtifactInfo>().activeEffectInfo;
        activeCoef = artifact.GetComponent<ArtifactInfo>().activeCoefInfo;
        passiveEffectList = artifact.GetComponent<ArtifactInfo>().passiveListInfo;
        passiveCoefList = artifact.GetComponent<ArtifactInfo>().passiveCoefListInfo;
        artifactCost = artifact.GetComponent<ArtifactInfo>().artifactCost;

    }

    public void ArtifactBuffApply() // ���� ���� �� �÷��̾�� ��Ƽ��Ʈ�� ���� ����
    {
        GameObject mountedArtifact = GameObject.FindWithTag("Artifact");
        if (mountedArtifact != null)
        {
            for (int i = 0; i < passiveEffectList.Count; i++)
            {
                playerState.ApplyBuffDebuff(passiveEffectList[i], 1, passiveCoefList[i], 0); // ���� ��ġ ����
            }
        }    
    }

    public void ArtifactBuffRemove()
    {
        for (int i = 0; i < passiveEffectList.Count; i++)
        {
            playerState.RemoveBuffDebuff(passiveEffectList[i], 1, passiveCoefList[i], 0);
        }
    }

    public void ResetArtifactReady()  // ��Ƽ��Ʈ�� ��� Ƚ�� �ʱ�ȭ
    {
        artifactReady = true;   // ��� �������� ����
    }

    public void DeactivateArtfactReady()
    {
        artifactReady = false;
    }

    public void ActiveEffectApply() // �÷��̾ ������ �ִ� ��Ƽ��(ȸ��,��,��ο�)
    {
        if (artifactReady)
        {
            if (playerState.currentResource >= artifactCost)
            {
                switch (activeEffect)
                {
                    case ActiveEffect.Draw:
                        for (int i = 0; i < activeCoef; i++)
                        {
                            deckManager.CardDraw();
                        }
                        CostSpend();
                        artifactReady = false;
                        break;

                    case ActiveEffect.Damage:
                        Debug.Log("Damage_Artifact");
                        artifactDamageReady = true;
                        break;

                    case ActiveEffect.Heal:
                        playerState.Heal(activeCoef);
                        CostSpend();
                        artifactReady = false;
                        break;

                    case ActiveEffect.Shiled:
                        playerState.ApplyShield(activeCoef);
                        CostSpend();
                        artifactReady = false;
                        break;

                    default:
                        Debug.Log("none identifiyed active");
                        break;
                }

            }
            else
            {
                Debug.Log("Not enough Resources");
            }
        }
        else
        {
            Debug.Log("Artifact Active Unavailable");
        }
        
    }

    public void ArtifactBlurONOFF() // ��Ƽ��Ʈ�� ��밡�� ���ο� ���� ���� ����(���� ����ĳ��Ʈ�� �ȵǰ� �� ����)
    {
        Color color = artifact.GetComponent<SpriteRenderer>().color;

        if (artifactReady)
        {
            color.a = 1.0f;  // ��밡���ϸ� ������       
        }
        else
        {
            color.a = 0.5f; // ��� �Ұ� �� ������
        }

        artifact.GetComponent<SpriteRenderer>().color = color; // ���İ� ����
    }

    // ��Ƽ��Ʈ �κ�

    public void AddArtifact2Inven(int artifactNum) // �κ��丮�� ��Ƽ��Ʈ �ֱ�
    {

        if (artifactInvenList == null)
        {
            artifactInvenList = new int[] { artifactNum };
        }
        else
        {
            int[] dest = new int[artifactInvenList.Length + 1];
            for (int i = 0, j = 0; i < artifactInvenList.Length; i++)
            {
                dest[j++] = artifactInvenList[i];
            }
            dest[dest.Length - 1] = artifactNum;

            artifactInvenList = dest;
        }
        
    }

    public void DeleteArtifact2Inven(int artifactNum)  //�κ��丮���� ��Ƽ��Ʈ ����
    {

        if (artifactInvenList == null)
        {
            Debug.Log("no artifact in inventory");
        }
        else
        {
            List<int> intList = new List<int>(artifactInvenList);

            intList.Remove(artifactNum);

            artifactInvenList = intList.ToArray();
        }
    }

    public void SynthesizeArtifact(int a, int b, int c)
    {

        if ((int)(a/10) == (int)(b/10) && (int)(b / 10) == (int)(c / 10)) // 3��Ƽ��Ʈ�� ����� ������
        {
            int[] inputArtifacts = new int[] { a, b }; // �ռ��� ���� ��Ƽ��Ʈ 2��

            System.Array.Sort(inputArtifacts); //����


            foreach (int[] key in artifactSynthesizeDict.Keys)
            {
                if (key[0] == inputArtifacts[0] && key[1] == inputArtifacts[1])
                {
                    AddArtifact2Inven(artifactSynthesizeDict[key]);
                    DeleteArtifact2Inven(a);
                    DeleteArtifact2Inven(b);
                    DeleteArtifact2Inven(c);
                }
                else
                {
                    Debug.Log("no match");
                }
            }
        }
        else
        {
            Debug.Log("grade missmatch");
        }
    }

    public void CostSpend()
    {
        playerState.SpendResource(artifactCost);
    }

}
