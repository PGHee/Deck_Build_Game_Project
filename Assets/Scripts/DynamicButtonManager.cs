using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonManager : MonoBehaviour
{
    public GameObject[] cardPrefabs;  // 카드 프리팹 배열
    public GameObject[] artifactPrefabs; // 아티팩트 프리팹 배열

    void Start()
    {

    }

    public void CardSpriteToButton(int cardNum, GameObject clearButton)
    {
        GameObject cardPrefab = Resources.Load<GameObject>($"Prefabs/Card/{CardNameConverter.CardNumToCode(cardNum)}");

        Sprite spriteFromPrefab = cardPrefab.GetComponent<Card>().cardImage;

        if (spriteFromPrefab != null)
        {
            // 버튼의 이미지 컴포넌트를 가져와 스프라이트를 변경합니다.
            clearButton.GetComponent<Image>().sprite = spriteFromPrefab;
        }
        else
        {
            Debug.LogError("프리팹에서 스프라이트를 가져올 수 없습니다. 프리팹에 SpriteRenderer가 포함되어 있는지 확인하세요.");
        }   
        Debug.Log(cardNum);
    }

    public void ArtifactSpriteToButton(int artifactNum, GameObject clearButton)
    {
        GameObject artifactPrefab = Resources.Load<GameObject>($"Prefabs/Artifact{artifactNum}");

        Sprite spriteFromPrefab = artifactPrefab.GetComponent<SpriteRenderer>().sprite;

        // 스프라이트가 정상적으로 가져와졌는지 확인합니다.
        if (spriteFromPrefab != null)
        {
            // 버튼의 이미지 컴포넌트를 가져와 스프라이트를 변경합니다.
            clearButton.GetComponent<Image>().sprite = spriteFromPrefab;
        }
        else
        {
            Debug.LogError("프리팹에서 스프라이트를 가져올 수 없습니다. 프리팹에 SpriteRenderer가 포함되어 있는지 확인하세요.");
        }
        Debug.Log(artifactNum);
    }

}
