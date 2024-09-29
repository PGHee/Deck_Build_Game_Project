using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonManager : MonoBehaviour
{
    public GameObject[] cardPrefabs;  // ī�� ������ �迭
    public GameObject[] artifactPrefabs; // ��Ƽ��Ʈ ������ �迭

    public void CardSpriteToButton(int cardNum, GameObject clearButton)
    {
        GameObject cardPrefab = Resources.Load<GameObject>($"Prefabs/Card/{CardNameConverter.CardNumToCode(cardNum)}");

        Sprite spriteFromPrefab = cardPrefab.GetComponent<Card>().cardImage;

        if (spriteFromPrefab != null)
        {
            // ��ư�� �̹��� ������Ʈ�� ������ ��������Ʈ�� �����մϴ�.
            clearButton.GetComponent<Image>().sprite = spriteFromPrefab;
        }
        else
        {
            Debug.LogError("�����տ��� ��������Ʈ�� ������ �� �����ϴ�. �����տ� SpriteRenderer�� ���ԵǾ� �ִ��� Ȯ���ϼ���.");
        }
    }

    public void ArtifactSpriteToButton(int artifactNum, GameObject clearButton)
    {
        GameObject artifactPrefab = Resources.Load<GameObject>($"Prefabs/Artifact/artifact_{artifactNum}");

        Sprite spriteFromPrefab = artifactPrefab.GetComponent<SpriteRenderer>().sprite;

        // ��������Ʈ�� ���������� ������������ Ȯ���մϴ�.
        if (spriteFromPrefab != null)
        {
            // ��ư�� �̹��� ������Ʈ�� ������ ��������Ʈ�� �����մϴ�.
            clearButton.GetComponent<Image>().sprite = spriteFromPrefab;
        }
        else
        {
            Debug.LogError("�����տ��� ��������Ʈ�� ������ �� �����ϴ�. �����տ� SpriteRenderer�� ���ԵǾ� �ִ��� Ȯ���ϼ���.");
        }
    }

}
