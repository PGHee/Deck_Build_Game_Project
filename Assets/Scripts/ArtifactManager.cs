using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour // 아티팩트 매니저 오브젝트에 적용, 실제 아티팩트의 동작을 처리
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
        if (artifact)  // 아티팩트 오브젝트가 있을 때만 블러처리 체크
        {
            ArtifactBlurONOFF(); 
        }

        if (artifactDamageReady)
        {
            if (Input.GetMouseButtonUp(0) && artifactReady)
            {
                // 마우스 클릭 위치를 월드 좌표로 변환
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // 해당 위치로 레이캐스트 발사
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                // 오브젝트가 감지되었는지 확인
                if (hit.collider != null)
                {
                    // 감지된 오브젝트의 이름 출력
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

    public void GetArtifactInfo() // 착용된 아티팩트의 정보를 불러옴
    {
        artifact = GameObject.FindWithTag("Artifact");
        activeEffect = artifact.GetComponent<ArtifactInfo>().activeEffectInfo;
        activeCoef = artifact.GetComponent<ArtifactInfo>().activeCoefInfo;
        passiveEffectList = artifact.GetComponent<ArtifactInfo>().passiveListInfo;
        passiveCoefList = artifact.GetComponent<ArtifactInfo>().passiveCoefListInfo;
        artifactCost = artifact.GetComponent<ArtifactInfo>().artifactCost;

    }

    public void ArtifactBuffApply() // 전투 시작 시 플레이어에게 아티팩트의 버프 적용
    {
        GameObject mountedArtifact = GameObject.FindWithTag("Artifact");
        if (mountedArtifact != null)
        {
            for (int i = 0; i < passiveEffectList.Count; i++)
            {
                playerState.ApplyBuffDebuff(passiveEffectList[i], 1, passiveCoefList[i], 0); // 버프 수치 적용
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

    public void ResetArtifactReady()  // 아티팩트의 사용 횟수 초기화
    {
        artifactReady = true;   // 사용 가능으로 변경
    }

    public void DeactivateArtfactReady()
    {
        artifactReady = false;
    }

    public void ActiveEffectApply() // 플레이어에 영향을 주는 액티브(회복,방어도,드로우)
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

        if ((int)(a/10) == (int)(b/10) && (int)(b / 10) == (int)(c / 10)) // 3아티팩트의 등급이 같으면
        {
            int[] inputArtifacts = new int[] { a, b }; // 합성의 기준 아티팩트 2개

            System.Array.Sort(inputArtifacts); //정렬


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
