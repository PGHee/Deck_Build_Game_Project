using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour // ��Ƽ��Ʈ �Ŵ��� ������Ʈ�� ����, ���� ��Ƽ��Ʈ�� ������ ó��
{
    public DeckManager deckManager;
    public GameObject player;
    public PlayerState playerState;
    public GameObject artifact;

    public bool artifactReady;
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

        artifactInvenList = null;

        artifactSynthesizeDict = new Dictionary<int[], int>();

        artifactSynthesizeDict.Add(new int[] {1, 2}, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (artifact)  // ��Ƽ��Ʈ ������Ʈ�� ���� ���� ��ó�� 
        {
            ArtifactBlurONOFF(); 
        }
        
        if (Input.GetKeyDown("a")) //��Ƽ��Ʈ ���� �� ���� ����
        {
            GenerateArtifect(1);
            GetArtifectInfo();
        }


        if (Input.GetKeyDown("q")) //��Ƽ�� �нú� �Ѵ� ����
        {
            ActiveEffectApply();
            ArtifactBuffApply();
        }

        if (Input.GetKeyDown("s")) //��Ƽ��Ʈ �ռ� ����
        {
            SynthesizeArtifact(1, 2, 1);
        }
    }

    public void GenerateArtifect(int artifactNum)
    {
        GameObject go = Instantiate(artifactPrefabs[artifactNum - 1]);
        go.transform.position = new Vector3(-5, 4, 1);
        go.name = "Artifact";
    }

    public void GetArtifectInfo() // ����� ��Ƽ��Ʈ�� ������ �ҷ���
    {
        artifact = GameObject.FindWithTag("Artifact");
        activeEffect = artifact.GetComponent<ArtifactInfo>().activeEffectInfo;
        activeCoef = artifact.GetComponent<ArtifactInfo>().activeCoefInfo;
        passiveEffectList = artifact.GetComponent<ArtifactInfo>().passiveListInfo;
        passiveCoefList = artifact.GetComponent<ArtifactInfo>().passiveCoefListInfo;

        ResetArtifactReady();
    }

    public void ArtifactBuffApply() // ���� ���� �� �÷��̾�� ��Ƽ��Ʈ�� ���� ����
    {
        for (int i = 0; i < passiveEffectList.Count; i++)
        {
            playerState.ApplyBuffDebuff(passiveEffectList[i], 1, passiveCoefList[i], 0);
        }
    }

    public void ArtfactActiveApply() // ��Ƽ��Ʈ ��Ƽ�� ȿ�� ���� 
    {
        // ȿ�� ó��

        artifactReady = false; //��� �Ұ��� ����
    }

    public void ResetArtifactReady()  // ��Ƽ��Ʈ�� ��� Ƚ�� �ʱ�ȭ
    {
        artifactReady = true;   // ��� �������� ����
    }

    public void ActiveEffectApply() // �÷��̾ ������ �ִ� ��Ƽ��(ȸ��,��,��ο�)
    {
        if (artifactReady)
        {
            switch (activeEffect)
            {
                case ActiveEffect.Draw:
                    for (int i = 0; i < activeCoef; i++)
                    {
                        deckManager.CardDraw();
                    }
                    break;

                case ActiveEffect.Damage:
                    Debug.Log("Damage_Artifact");
                    break;

                case ActiveEffect.Heal:
                    playerState.Heal(activeCoef);
                    break;

                case ActiveEffect.Shiled:
                    playerState.ApplyShield(activeCoef);
                    break;

                default:
                    Debug.Log("none identifiyed active");
                    break;
            }

            artifactReady = false; //��� �Ұ��� ����
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
        int outArtifact;

        if ((int)(a/10) == (int)(b/10) && (int)(b / 10) == (int)(c / 10)) // 3��Ƽ��Ʈ�� ����� ������
        {
            if (artifactSynthesizeDict.TryGetValue(new int[] {a,b}, out outArtifact))
            {
                AddArtifact2Inven(outArtifact);
            }
            else
            {
                Debug.Log("no match");
                Debug.Log(artifactSynthesizeDict);
            }
        }
    }


}
