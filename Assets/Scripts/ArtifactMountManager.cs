using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactMountManager : MonoBehaviour
{
    public ArtifactManager artifactManager;
    public GameDirector gameDirector;

    public int ArtifactListPage;
    public int[] artifactInvenList;
    public int pageArtifactNum;
    public GameObject[] clearButtonsArtifact;
    public GameObject[] artifactPrefabs;
    public TextMeshProUGUI artifactText;
    public bool buffOn;




    // Start is called before the first frame update
    void Start()
    {
        gameDirector = FindObjectOfType<GameDirector>();
        ArtifactListPage = 0; 
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
    }

    public void ArtifactListUp() // ��Ƽ��Ʈ �κ��丮�� uiâ�� ����
    {
        if (!TurnManager.instance.enabled)
        {
            SetAllButtonClear();

            GetClearButtonArtifact();

            artifactInvenList = artifactManager.artifactInvenList;

            if (artifactInvenList.Length > 15 * (ArtifactListPage + 1))
            {
                pageArtifactNum = 15;
            }
            else
            {
                pageArtifactNum = artifactInvenList.Length % 15;
                ArtifactListPage = (int)((artifactInvenList.Length - artifactInvenList.Length % 15) / 15);
            }

            for (int i = 0; i < pageArtifactNum; i++)
            {
                clearButtonsArtifact[i].GetComponent<Image>().sprite = artifactPrefabs[artifactInvenList[15 * ArtifactListPage + i] - 1].GetComponent<SpriteRenderer>().sprite;
                // ���� ������ ������ ���� ���� ����
                Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
                color.a = 255.0f;

                // ����� ������ �ٽ� ����
                clearButtonsArtifact[i].GetComponent<Image>().color = color;
            }
        }       
    }

    public void GetClearButtonArtifact() // uiâ�� ���� ��ư���� ����
    {
        clearButtonsArtifact = GameObject.FindGameObjectsWithTag("ClearButtonBook");
    }

    public void SetPageNext() // ���� ������
    {
        ArtifactListPage++;
        ArtifactListUp();
    }

    public void SetPageBefore() // ���� ������
    {
        ArtifactListPage--;
        if (ArtifactListPage < 0)
        {
            ArtifactListPage = 0;
        }
        ArtifactListUp();
    }

    public void SetAllButtonClear() // ��Ƽ��Ʈ ���� �� ���� ��ư, �ʱ� ��ư ����ȭ
    {
        GetClearButtonArtifact();

        for(int i = 0; i < clearButtonsArtifact.Length; i++)
        {
            // ���� ������ ������ ���� ���� ����
            Color color = clearButtonsArtifact[i].GetComponent<Image>().color;
            color.a = 0.0f;

            // ����� ������ �ٽ� ����
            clearButtonsArtifact[i].GetComponent<Image>().color = color;
        }
    }

    public void MountArtifact(int ButtonNum)
    {
        if (ButtonNum < pageArtifactNum && !TurnManager.instance.enabled)
        {
            // ������ ������ ��Ƽ��Ʈ ������Ʈ ����
            GameObject artifactMounted = GameObject.FindWithTag("Artifact");
            if (artifactMounted != null)
            {
                UnmountArtifact();
                //Destroy(artifactMounted);
            }
            
            artifactManager.GenerateArtifact(artifactInvenList[15 * ArtifactListPage + ButtonNum]);

            artifactManager.GetArtifactInfo(artifactInvenList[15 * ArtifactListPage + ButtonNum]);

            GameObject artifactMountImage = GameObject.Find("Artifact_Mount_Sprite");

            artifactMountImage.GetComponent<Image>().sprite = artifactPrefabs[artifactInvenList[15 * ArtifactListPage + ButtonNum] - 1].GetComponent<SpriteRenderer>().sprite;

            Color color = artifactMountImage.GetComponent<Image>().color;
            color.a = 255.0f;

            // ����� ������ �ٽ� ����
            artifactMountImage.GetComponent<Image>().color = color;

            artifactText.text = GetArtifactText(artifactInvenList[15 * ArtifactListPage + ButtonNum] - 1);

            GameObject artifactGenerated = GameObject.FindWithTag("Artifact");
            artifactGenerated.GetComponent<UIToolTip>().descriptionTextArtifact = GetArtifactText(artifactInvenList[15 * ArtifactListPage + ButtonNum] - 1); // 아티팩트 툴팁을 위한 

            MountedArtifactBuffApply();
        }
    }

    public void UnmountArtifact()
    {
        // 아티팩트 장착 해제
        // 버그 방지를 위해 작동
        // 아티팩트 합성 창 열람 시 작동!!!! 
        GameObject mountedArtifact = GameObject.FindWithTag("Artifact");
        if (mountedArtifact != null)
        {
            //Destroy(GameObject.FindWithTag("Artifact")); // 씬에 배치된 아티팩트 제거
            if (buffOn)
            {
                artifactManager.ArtifactBuffRemove();
                buffOn = false;
            }

            //Color color = mountedArtifact.GetComponent<SpriteRenderer>().color;
            //mountedArtifact.GetComponent<SpriteRenderer>().color = color;
            mountedArtifact.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Images/UI/ClearBox");
            mountedArtifact.GetComponent<UIToolTip>().openTooltip = false;
        }
    }

    public void MountedArtifactBuffApply()
    {
        GameObject mountedArtifact = GameObject.FindWithTag("Artifact");
        if (mountedArtifact != null && !buffOn)
        {
            artifactManager.ArtifactBuffApply();
            buffOn = true;
        }
    }

    public string GetArtifactText(int artifactnum)
    {
        string outputString = new string ("");

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

        ArtifactInfo artifactInfo = artifactPrefabs[artifactnum].GetComponent<ArtifactInfo>();

        switch ((int)(artifactnum / 10))
        {
            case 0:
                outputString += "[하급 아티팩트]<br>";
                break;
            case 1:
                outputString += "[중급 아티팩트]<br>";
                break ;
            case 2:
                outputString += "[상급 아티팩트]<br>";
                break;
            default:
                outputString += "[오류 아티팩트]<br>";
                break;
        }

        if(artifactInfo.activeCoefInfo > 0)
        {
            outputString += "[액티브] : [서클 : " + artifactInfo.artifactCost + "]";

            switch (artifactInfo.activeEffectInfo)
            {
                case ArtifactManager.ActiveEffect.Heal:
                    outputString += "<br>체력을 " + artifactInfo.activeCoefInfo + "회복합니다.";
                    break;

                case ArtifactManager.ActiveEffect.Damage:
                    outputString += "<br>피해를 " + artifactInfo.activeCoefInfo + "입힙니다.";
                    break;

                case ArtifactManager.ActiveEffect.Draw:
                    outputString += "<br>카드를 " + artifactInfo.activeCoefInfo + "장 뽑습니다.";
                    break;

                case ArtifactManager.ActiveEffect.Shiled:
                    outputString += "<br><br>방어도를 " + artifactInfo.activeCoefInfo + "획득합니다.";
                    break;

                case ArtifactManager.ActiveEffect.AreaDamage:
                    outputString += "<br>모든 적에게 피해를" + artifactInfo.activeCoefInfo + "입힙니다.";
                    break;

                case ArtifactManager.ActiveEffect.MultiHit:
                    outputString += "<br>5 피해를 " + artifactInfo.activeCoefInfo + "번 입힙니다.";
                    break;

                case ArtifactManager.ActiveEffect.Poison:
                    outputString += "<br>독을" + artifactInfo.activeCoefInfo + "부여합니다.";
                    break;
            }
        }
        else
        {
            outputString += "[액티브] : 없음";
        }
        outputString += "<br>[패시브] ";

        for (int i = 0; i < artifactInfo.passiveListInfo.Count; i++)
        {
            switch (artifactInfo.passiveListInfo[i])
            {
                case EffectType.IncreaseDamage:
                    outputString += "<br>피해가 " + artifactInfo.passiveCoefListInfo[i] * 100+ "% 증가합니다.";
                    break;

                case EffectType.LifeSteal:
                    outputString += "<br>가한 피해의 " + artifactInfo.passiveCoefListInfo[i] * 100+ "% 만큼 회복합니다.";
                    break;

                case EffectType.ReduceDamage:
                    outputString += "<br>받는 피해가 " + artifactInfo.passiveCoefListInfo[i]  * 100+ "% 감소합니다.";
                    break;

                default:
                    outputString += "<br>알 수 없는 효과입니다.";
                    break;
            }
        }

        if (artifactInfo.bonusAttack != 0) outputString += "<br>타격 시" + (int)(artifactInfo.bonusAttack * 100)+ "% 확률로 재 타격 합니다.";
        if (artifactInfo.bonusCrystal != 0) outputString += "<br>크리스탈을" + artifactInfo.bonusCrystal * 100 + "% 추가로 얻습니다.";
        if (artifactInfo.bonusPoison != 0) outputString += "<br>카드의 대상에게 독을" + artifactInfo.bonusPoison + "부여합니다.";
        if (artifactInfo.bonusAttributeExperience != 0) outputString += "<br>속성 경험치를" + artifactInfo.bonusAttributeExperience * 100 + "% 추가로 얻습니다.";
        if (artifactInfo.execution != false) outputString += "<br>데미지를 준 대상의 남은 체력이 10% 이하면 처형합니다.";
        if (artifactInfo.doubleLifeSteal != false) outputString += "<br>남은 체력이 50% 이하면 2배로 회복합니다.";
        if (artifactInfo.bonusDamage != 0) outputString += "<br>타격 시 " + artifactInfo.bonusDamage+ "의 고정 데미지를 추가 합니다.";
        if (artifactInfo.bonusHeal != 0) outputString += "<br>매 턴 " + artifactInfo.bonusHeal+ "만큼 회복합니다.";
        if (artifactInfo.bonusShield != 0) outputString += "<br>매 턴 " + artifactInfo.bonusShield + "만큼 방어도를 얻습니다.";
        if (artifactInfo.bonusDraw != 0) outputString += "<br>매 턴 카드를" + artifactInfo.bonusDraw + "장 드로우 합니다.";

        return outputString;
    }
}
