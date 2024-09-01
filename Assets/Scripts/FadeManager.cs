using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage; // 페이드 효과를 위한 UI 이미지
    public float fadeDuration = 1f; // 페이드 지속 시간

    private void OnEnable()
    {
        // 페이드 효과를 초기화
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void StartFadeInOut()
    {
        // 페이드 인 아웃 코루틴 시작
        StartCoroutine(FadeInOut());
    }

    private IEnumerator FadeInOut()
    {
        yield return StartCoroutine(FadeIn());

        // 여기에 포탈 이동 로직 또는 다른 이벤트를 추가할 수 있음

        yield return StartCoroutine(FadeOut());

        // 페이드 효과가 끝나면 패널을 비활성화
        gameObject.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}
