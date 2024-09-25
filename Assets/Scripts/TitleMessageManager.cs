using System.Collections;
using UnityEngine;
using TMPro; // TextMeshProUGUI를 사용하기 위해 추가

public class TitleMessageManager : MonoBehaviour
{
    public GameObject titleMessagePrefab; // 타이틀 메시지 프리팹
    public float fadeDuration = 1.0f; // 페이드 인/아웃 애니메이션 지속 시간
    public float displayDuration = 2.0f; // 메시지가 화면에 표시되는 시간

    public void ShowTitleMessage(string message)
    {
        // 화면 중앙보다 약간 위쪽 위치로 설정
        Vector3 screenCenterPosition = new Vector3(Screen.width / 2, Screen.height * 0.8f, 0);
        StartCoroutine(ShowMessageRoutine(screenCenterPosition, message));
    }

    private IEnumerator ShowMessageRoutine(Vector3 screenPosition, string message)
    {
        // 타이틀 메시지 인스턴스 생성
        GameObject textInstance = Instantiate(titleMessagePrefab, transform);

        // RectTransform을 사용하여 캔버스의 중심 위쪽에 배치
        RectTransform rectTransform = textInstance.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(screenPosition.x - Screen.width / 2, screenPosition.y - Screen.height / 2); 

        // 텍스트 메시지 설정
        TextMeshProUGUI tmp = textInstance.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = message;
            tmp.raycastTarget = false;  // TextMeshProUGUI의 Raycast Target 비활성화
        }

        // CanvasGroup을 추가하여 알파값 조절 및 클릭 방지 설정
        CanvasGroup canvasGroup = textInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = textInstance.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0; // 초기 알파값 0으로 설정 (투명)
        canvasGroup.blocksRaycasts = false;  // 전체 CanvasGroup이 클릭을 차단하지 않도록 설정

        // 페이드 인 애니메이션
        yield return StartCoroutine(FadeText(canvasGroup, 0, 1, fadeDuration));

        // 메시지 표시 대기
        yield return new WaitForSeconds(displayDuration);

        // 페이드 아웃 애니메이션
        yield return StartCoroutine(FadeText(canvasGroup, 1, 0, fadeDuration));

        // 애니메이션이 끝나면 텍스트 삭제
        Destroy(textInstance);
    }

    private IEnumerator FadeText(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // 종료 알파값으로 설정
    }
}
