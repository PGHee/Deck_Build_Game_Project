using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefab; // ������ ������
    public Transform panel;   // �������� �߰��� �г�

    // �޼ҵ� ����
    public void SpawnObject()
    {
        if (prefab != null && panel != null)
        {
            // ������ �ν��Ͻ� ����
            GameObject newObject = Instantiate(prefab, panel);

            // Transform ������Ʈ�� ����Ͽ� ��ġ�� ũ�� ����
            Transform objectTransform = newObject.GetComponent<Transform>();
            if (objectTransform != null)
            {
                objectTransform.localPosition = Vector3.zero;  // �θ� �г��� �߾����� ��ġ ����
                objectTransform.localScale = Vector3.one;     // ũ�⸦ 1�� �����Ͽ� �г� ũ�⿡ ����
            }
            else
            {
                Debug.LogError("The prefab does not have a Transform component!");
            }
        }
        else
        {
            Debug.LogError("Prefab or Panel is not assigned!");
        }
    }

    public void OnButtonClick()
    {
        ObjectSpawner spawner = FindObjectOfType<ObjectSpawner>();
        if (spawner != null)
        {
            spawner.SpawnObject();
        }
    }
}

