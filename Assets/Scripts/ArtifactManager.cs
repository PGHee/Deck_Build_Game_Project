using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour // 아티팩트 매니저 오브젝트에 적용, 실제 아티팩트의 동작을 처리
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
        if (artifact)  // 아티팩트 오브젝트가 있을 때만 블러처리 
        {
            ArtifactBlurONOFF(); 
        }
        
        if (Input.GetKeyDown("a")) //아티팩트 생성 후 정보 갱신
        {
            GenerateArtifect(1);
            GetArtifectInfo();
        }


        if (Input.GetKeyDown("q")) //액티브 패시브 둘다 적용
        {
            ActiveEffectApply();
            ArtifactBuffApply();
        }

        if (Input.GetKeyDown("s")) //아티팩트 합성 실험
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

    public void GetArtifectInfo() // 착용된 아티팩트의 정보를 불러옴
    {
        artifact = GameObject.FindWithTag("Artifact");
        activeEffect = artifact.GetComponent<ArtifactInfo>().activeEffectInfo;
        activeCoef = artifact.GetComponent<ArtifactInfo>().activeCoefInfo;
        passiveEffectList = artifact.GetComponent<ArtifactInfo>().passiveListInfo;
        passiveCoefList = artifact.GetComponent<ArtifactInfo>().passiveCoefListInfo;

        ResetArtifactReady();
    }

    public void ArtifactBuffApply() // 전투 시작 시 플레이어에게 아티팩트의 버프 적용
    {
        for (int i = 0; i < passiveEffectList.Count; i++)
        {
            playerState.ApplyBuffDebuff(passiveEffectList[i], 1, passiveCoefList[i], 0);
        }
    }

    public void ArtfactActiveApply() // 아티팩트 액티브 효과 적용 
    {
        // 효과 처리

        artifactReady = false; //사용 불가로 변경
    }

    public void ResetArtifactReady()  // 아티팩트의 사용 횟수 초기화
    {
        artifactReady = true;   // 사용 가능으로 변경
    }

    public void ActiveEffectApply() // 플레이어에 영향을 주는 액티브(회복,방어도,드로우)
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

            artifactReady = false; //사용 불가로 변경
        }
        else
        {
            Debug.Log("Artifact Active Unavailable");
        }
        
    }

    public void ArtifactBlurONOFF() // 아티팩트의 사용가능 여부에 따라 투명도 조절(이후 레이캐스트도 안되게 할 예정)
    {
        Color color = artifact.GetComponent<SpriteRenderer>().color;

        if (artifactReady)
        {
            color.a = 1.0f;  // 사용가능하면 불투명       
        }
        else
        {
            color.a = 0.5f; // 사용 불가 시 반투명
        }

        artifact.GetComponent<SpriteRenderer>().color = color; // 알파값 적용
    }

    // 아티팩트 인벤

    public void AddArtifact2Inven(int artifactNum) // 인벤토리에 아티팩트 넣기
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

    public void DeleteArtifact2Inven(int artifactNum)  //인벤토리에서 아티팩트 빼기
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

        if ((int)(a/10) == (int)(b/10) && (int)(b / 10) == (int)(c / 10)) // 3아티팩트의 등급이 같으면
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
