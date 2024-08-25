using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonManager : MonoBehaviour
{
    public GameObject[] cardPrefabs;  // ī�� ������ �迭
    public GameObject[] artifactPrefabs; // ��Ƽ��Ʈ ������ �迭

    void Start()
    {

    }

    public void CardSpriteToButton(int cardNum, GameObject clearButton)
    {
        //GameObject cardPrefab = Resources.Load<GameObject>($"Prefabs/CardPrefab{cardNum}");
        GameObject cardPrefab = Resources.Load<GameObject>($"Prefabs/CardPrefab4");
        // �ڽ� ������Ʈ���� ��������Ʈ�� ��� ��������
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

            // ��������Ʈ�� �ִ� ��� UI ��ư�� ���ο� Image�� �߰�
            if (sprite != null)
            {
                // ���ο� UI Image ����
                GameObject newImageObject = new GameObject("SpriteImage");
                newImageObject.transform.SetParent(clearButton.transform, false);

                Image newImage = newImageObject.AddComponent<Image>();
                newImage.sprite = sprite;

                // ���ϴ� ��ġ ���� (���⼭�� �߾ӿ� ��ġ) �̰� �ؾ���!!!!
                RectTransform rectTransform = newImage.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector2.zero; // �߽ɿ� ��ġ
                //rectTransform.sizeDelta = clearButton.GetComponent<RectTransform>().sizeDelta; // ��ư ũ�⿡ ���߱�
            }
        }
        Debug.Log(cardNum);
    }

}
