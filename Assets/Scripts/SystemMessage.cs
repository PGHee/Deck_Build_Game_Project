using System.Collections;
using UnityEngine;
using TMPro; // TextMeshProUGUI를 사용하기 위해 추가

public class SystemMessage : MonoBehaviour
{
    public GameObject systemMessagePrefab; // 시스템 메시지 프리팹

    public void ShowSystemMessage(string message)
    {
        // 메시지 중앙에 배치
        StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        // 시스템 메시지 인스턴스 생성
        GameObject textInstance = Instantiate(systemMessagePrefab, transform);

        // RectTransform을 사용하여 캔버스의 중심에 배치
        RectTransform rectTransform = textInstance.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector3.zero;  // 캔버스 중심에 위치

        // 텍스트 메시지 설정
        TextMeshProUGUI tmp = textInstance.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = message;
            tmp.raycastTarget = false;  // TextMeshProUGUI의 Raycast Target을 비활성화
        }

        // CanvasGroup을 가져오거나 추가하고 Blocks Raycasts 비활성화
        CanvasGroup canvasGroup = textInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = textInstance.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false;  // 전체 CanvasGroup이 클릭 차단을 하지 않도록 설정

        // 애니메이션 시작
        StartCoroutine(AnimateSystemMessage(rectTransform));

        yield return null;
    }

    private IEnumerator AnimateSystemMessage(RectTransform rectTransform)
    {
        float duration = 1.5f; // 애니메이션 지속 시간
        float elapsedTime = 0f;
        Vector3 initialPosition = rectTransform.anchoredPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0, 150, 0); // 위로 이동

        CanvasGroup canvasGroup = rectTransform.GetComponent<CanvasGroup>();

        while (elapsedTime < duration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration); // 서서히 투명해짐
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(rectTransform.gameObject); // 애니메이션이 끝나면 텍스트 삭제
    }
}
