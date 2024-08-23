using System.Collections;
using UnityEngine;
using TMPro; // TextMeshProUGUI를 사용하기 위해 추가

public class SystemMessage : MonoBehaviour
{
    public GameObject systemMessagePrefab; // 시스템 메시지 프리팹

    public void ShowSystemMessage(string message)
    {
        // 화면 중앙 위치로 설정
        Vector3 screenCenterPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        StartCoroutine(ShowMessageRoutine(screenCenterPosition, message));
    }

    private IEnumerator ShowMessageRoutine(Vector3 position, string message)
    {
        // UI 캔버스 좌표로 변환
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        worldPosition.z = 0; // UI 상에서 Z 축은 0으로 고정

        // 시스템 메시지 인스턴스 생성
        GameObject textInstance = Instantiate(systemMessagePrefab, worldPosition, Quaternion.identity, transform);
        TextMeshProUGUI tmp = textInstance.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = message;

        // 애니메이션 시작
        StartCoroutine(AnimateSystemMessage(textInstance));

        yield return null;
    }

    private IEnumerator AnimateSystemMessage(GameObject textInstance)
    {
        float duration = 1.5f; // 애니메이션 지속 시간
        float elapsedTime = 0f;
        Vector3 initialPosition = textInstance.transform.position;
        Vector3 targetPosition = initialPosition + new Vector3(0, 1.5f, 0); // 위로 이동

        CanvasGroup canvasGroup = textInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = textInstance.AddComponent<CanvasGroup>();
        }

        while (elapsedTime < duration)
        {
            textInstance.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration); // 서서히 투명해짐
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(textInstance); // 애니메이션이 끝나면 텍스트 삭제
    }
}
