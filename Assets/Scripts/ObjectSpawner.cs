using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefab; // 생성할 프리팹
    public Transform panel;   // 프리팹을 추가할 패널

    // 메소드 선언
    public void SpawnObject()
    {
        if (prefab != null && panel != null)
        {
            // 프리팹 인스턴스 생성
            GameObject newObject = Instantiate(prefab, panel);

            // Transform 컴포넌트를 사용하여 위치와 크기 조정
            Transform objectTransform = newObject.GetComponent<Transform>();
            if (objectTransform != null)
            {
                objectTransform.localPosition = Vector3.zero;  // 부모 패널의 중앙으로 위치 설정
                objectTransform.localScale = Vector3.one;     // 크기를 1로 설정하여 패널 크기에 맞춤
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

