using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel; // 팝업 패널
    public Text popupText; // 팝업의 텍스트 컴포넌트
    public Image popupImage; // 팝업의 이미지 컴포넌트 (선택 사항)
    public Transform contentArea; // 팝업 내 오브젝트가 생성될 위치
    public GameObject prefabToInstantiate; // 생성할 프리팹

    public GameObject prefab; // 생성할 프리팹(새로운 방법)
    public Transform panel;   // 프리팹을 추가할 패널
    public string testKey;

    void Start()
    {
        popupPanel.SetActive(false); // 시작할 때 팝업 비활성화
    }

    void Update()
    {
        // 예시: 조건이 달성되었을 때 팝업을 활성화
        if (ConditionMet())
        {
            ShowPopup("축하합니다! 새로운 카드를 획득했습니다.", null); // 이미지가 필요 없을 경우 null로 설정
        }
    }

    bool ConditionMet()
    {
        // 여기에 특정 조건을 체크하는 로직을 구현
        // 예를 들어, 점수가 일정 점수 이상이 되면 true를 반환
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
        // 팝업의 내용물을 정리
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
    }
}

