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
        //GameObject cardPrefab = Resources.Load<GameObject>($"Prefabs/CardPrefab{cardNum}");
        GameObject cardPrefab = Resources.Load<GameObject>($"Prefabs/CardPrefab4");
        // 자식 오브젝트들의 스프라이트를 모두 가져오기
        Transform[] childTransforms = cardPrefab.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childTransforms)
        {
            Sprite sprite = null;
            SpriteRenderer spriteRenderer = childTransform.GetComponent<SpriteRenderer>();
            Image image = childTransform.GetComponent<Image>();

            if (spriteRenderer != null)
            {
                sprite = spriteRenderer.sprite;
            }
            else if (image != null)
            {
                sprite = image.sprite;
            }

            // 스프라이트가 있는 경우 UI 버튼에 새로운 Image로 추가
            if (sprite != null)
            {
                // 새로운 UI Image 생성
                GameObject newImageObject = new GameObject("SpriteImage");
                newImageObject.transform.SetParent(clearButton.transform, false);

                Image newImage = newImageObject.AddComponent<Image>();
                newImage.sprite = sprite;

                // 원하는 위치 조정 (여기서는 중앙에 배치) 이거 해야함!!!!
                RectTransform rectTransform = newImage.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector2.zero; // 중심에 배치
                //rectTransform.sizeDelta = clearButton.GetComponent<RectTransform>().sizeDelta; // 버튼 크기에 맞추기
            }
        }
        Debug.Log(cardNum);
    }

}
