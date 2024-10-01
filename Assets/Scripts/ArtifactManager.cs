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
    private HandControl handControl;

    public bool artifactReady;
    public bool artifactDamageReady;
    public enum ActiveEffect { Heal, Shiled, Draw, Damage, AreaDamage, MultiHit, Poison, Dump}
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
    } //���x

    public ActiveEffect activeEffect;
    public int activeCoef;
    public List<EffectType> passiveEffectList;
    public List<float> passiveCoefList;
    public int artifactCost;

    public GameObject[] artifactPrefabs;
    public int[] artifactInvenList;
    public Dictionary<int[], int> artifactSynthesizeDict;

    public int bonusPoison;
    public int bonusDamage;
    public int bonusShield;
    public int bonusHeal;
    public int bonusDraw;
    public float bonusAttack;
    public float bonusCrystal;
    public float bonusAttributeExperience;
    public bool execution;
    public bool doubleLifeSteal;


    // Start is called before the first frame update
    void Start()
    {
        artifactPrefabs = new GameObject[30];
        for (int i = 0; i < 7; i++)
        {
            artifactPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Artifact/artifact_{i + 1}");
        }

        for (int j = 10; j < 17; j++)
        {
            artifactPrefabs[j] = Resources.Load<GameObject>($"Prefabs/Artifact/artifact_{j + 1}");
        }

        for (int k = 20; k < 27; k++)
        {
            artifactPrefabs[k] = Resources.Load<GameObject>($"Prefabs/Artifact/artifact_{k + 1}");
        }

        artifactSynthesizeDict = new Dictionary<int[], int>();

        // ��Ƽ��Ʈ ���� ��
        artifactSynthesizeDict.Add(new int[] {1, 1}, 11);
        artifactSynthesizeDict.Add(new int[] {1, 2}, 12);
        artifactSynthesizeDict.Add(new int[] {1, 4}, 13);
        artifactSynthesizeDict.Add(new int[] {2, 3}, 14);
        artifactSynthesizeDict.Add(new int[] {4, 5}, 15);
        artifactSynthesizeDict.Add(new int[] {5, 7}, 16);
        artifactSynthesizeDict.Add(new int[] {6, 7}, 17);

        artifactSynthesizeDict.Add(new int[] {11, 11}, 21);
        artifactSynthesizeDict.Add(new int[] {12, 12}, 22);
        artifactSynthesizeDict.Add(new int[] {13, 13}, 23);
        artifactSynthesizeDict.Add(new int[] {14, 14}, 24);
        artifactSynthesizeDict.Add(new int[] {15, 15}, 25);
        artifactSynthesizeDict.Add(new int[] {16, 16}, 26);
        artifactSynthesizeDict.Add(new int[] {17, 17}, 27);



        artifactDamageReady = false;

        handControl = FindObjectOfType<HandControl>();
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
                        switch (activeEffect)
                        {
                            case ActiveEffect.Damage:
                                actions.DealMultipleHits(hit.collider.gameObject, activeCoef, 1, null, PlayerState.AttributeType.Fire);
                                artifactReady = false;
                                break;

                            case ActiveEffect.MultiHit:
                                actions.DealMultipleHits(hit.collider.gameObject, 5, activeCoef, null, PlayerState.AttributeType.Wind);
                                artifactReady = false;
                                break;

                            case ActiveEffect.AreaDamage:
                                GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
                                foreach (GameObject monster in monsters)
                                {
                                    actions.DealMultipleHits(monster, activeCoef, 1, null, PlayerState.AttributeType.Lightning);
                                }
                                artifactReady = false;
                                break;
                        }                      
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
        go.transform.position = new Vector3(-2.0f, 0f, 1);
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
        bonusPoison = artifact.GetComponent<ArtifactInfo>().bonusPoison;
        bonusAttack = artifact.GetComponent<ArtifactInfo>().bonusAttack;
        bonusCrystal = artifact.GetComponent<ArtifactInfo>().bonusCrystal;
        bonusAttributeExperience = artifact.GetComponent<ArtifactInfo>().bonusAttributeExperience;
        execution = artifact.GetComponent<ArtifactInfo>().execution;
        bonusDamage = artifact.GetComponent<ArtifactInfo>().bonusDamage;
        bonusShield = artifact.GetComponent<ArtifactInfo>().bonusShield;
        bonusHeal = artifact.GetComponent<ArtifactInfo>().bonusHeal;

        
    }

    public void ArtifactBuffApply() // ���� ���� �� �÷��̾�� ��Ƽ��Ʈ�� ���� ����
    {
        GameObject mountedArtifact = GameObject.FindWithTag("Artifact");

        passiveEffectList = mountedArtifact.GetComponent<ArtifactInfo>().passiveListInfo;
        passiveCoefList = mountedArtifact.GetComponent<ArtifactInfo>().passiveCoefListInfo;
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

        bonusAttack = 0;
        bonusCrystal = 0;
        bonusPoison = 0;
        bonusAttributeExperience = 0;
        execution = false;
        doubleLifeSteal = false;
        bonusDamage = 0;
        bonusHeal = 0;
        bonusShield = 0;
        bonusDraw = 0;
        playerState.doubleLifeSteal = false;
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

                    case ActiveEffect.Dump:                       
                        handControl.Dump(activeCoef);
                        CostSpend();
                        artifactReady = false;
                        break;

                    case ActiveEffect.Damage:
                        Debug.Log("Damage_Artifact");
                        artifactDamageReady = true;
                        break;

                    case ActiveEffect.AreaDamage:
                        Debug.Log("Damage_Artifact");
                        artifactDamageReady = true;
                        break;

                    case ActiveEffect.MultiHit:
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

                    case ActiveEffect.Poison:
                        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
                        foreach(GameObject monster in monsters)
                        {
                            monster.GetComponent<MonsterState>().ApplyPoison(activeCoef);
                        }
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
