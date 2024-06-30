using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel; // �˾� �г�
    public Text popupText; // �˾��� �ؽ�Ʈ ������Ʈ
    public Image popupImage; // �˾��� �̹��� ������Ʈ (���� ����)
    public Transform contentArea; // �˾� �� ������Ʈ�� ������ ��ġ
    public GameObject prefabToInstantiate; // ������ ������

    public GameObject prefab; // ������ ������(���ο� ���)
    public Transform panel;   // �������� �߰��� �г�
    public string testKey;

    void Start()
    {
        popupPanel.SetActive(false); // ������ �� �˾� ��Ȱ��ȭ
    }

    void Update()
    {
        // ����: ������ �޼��Ǿ��� �� �˾��� Ȱ��ȭ
        if (ConditionMet())
        {
            ShowPopup("�����մϴ�! ���ο� ī�带 ȹ���߽��ϴ�.", null); // �̹����� �ʿ� ���� ��� null�� ����
        }
    }

    bool ConditionMet()
    {
        // ���⿡ Ư�� ������ üũ�ϴ� ������ ����
        // ���� ���, ������ ���� ���� �̻��� �Ǹ� true�� ��ȯ
        return Input.GetKeyDown(testKey);
    }

    public void ShowPopup(string message, Sprite image)
    {
        popupPanel.SetActive(true);

        GameObject go = Instantiate(Resources.Load<GameObject>($"Prefabs/Popup"));
        go.name = "Popup";

        if (image != null)
        {
            //popupImage.sprite = image;
            //popupImage.gameObject.SetActive(true);
        }
        else
        {
            //popupImage.gameObject.SetActive(false);
        }
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        Destroy(GameObject.Find("Popup"));
    }

    void ClearPopupContents()
    {
        // �˾��� ���빰�� ����
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
    }
}

