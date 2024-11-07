using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private int originalLayer;
    public Transform dropZone; // 드롭존 지정
    public GameObject[] cutScenes; // 컷신 오브젝트들 (4개)
    private bool isCutscenePlaying = false; // 컷신이 재생 중인지 여부
    public float fadeDuration = 0.5f; // 페이드 인 지속 시간

    void Start()
    {
        startPosition = transform.position;
        originalLayer = gameObject.layer;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
        newPosition.z = 0;
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        gameObject.layer = originalLayer;

        // 드롭존에 카드가 닿았는지 확인
        if (hit.collider != null)
        {
            // 카드 종류에 따라 동작을 결정
            switch (gameObject.name)
            {
                case "0StartCard":
                    StartCoroutine(PlayCutscenesAndLoadGame());
                    break;

                case "0QuitCard":
                    QuitGame();
                    break;

                case "0SettingsCard":
                    OpenSettings();
                    break;

                default:
                    Debug.Log("Unknown card action.");
                    break;
            }
        }

        // 카드 원위치 복구
        transform.position = startPosition;
    }

    // 컷신을 순차적으로 재생하고 게임 씬 로드
    IEnumerator PlayCutscenesAndLoadGame()
    {
        isCutscenePlaying = true;

        // 씬 비동기 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");
        asyncLoad.allowSceneActivation = false; // 로딩 완료 후 씬 전환을 잠시 보류

        // 특정 컷신만 서서히 등장시키고 나머지는 바로 활성화
        for (int i = 0; i < cutScenes.Length; i++)
        {
            if (i == 2 || i == 6) // 예시로 두 번째 컷신만 서서히 등장
            {
                cutScenes[i].SetActive(true);
                yield return StartCoroutine(FadeInImage(cutScenes[i].GetComponent<Image>()));
            }
            else
            {
                cutScenes[i].SetActive(true);
            }
            yield return new WaitForSeconds(2.5f); // 각 컷신 재생 시간
        }

        // 컷신 재생이 끝났을 때 로딩이 끝났는지 확인, 로딩이 끝날 때까지 대기
        yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);

        // 컷신이 완료되면 씬 전환을 허용
        asyncLoad.allowSceneActivation = true;

        isCutscenePlaying = false;
    }

    // 이미지의 투명도를 서서히 증가시키는 코루틴
    IEnumerator FadeInImage(Image image)
    {
        if (image == null) yield break;
        
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0); // 투명도 초기화

        float timer = 0;
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration); // 투명도를 점진적으로 증가
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }
        
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1); // 완전히 보이도록 설정
    }

    // 게임 종료
    void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // 세팅 패널 열기
    void OpenSettings()
    {
        SettingsManager.Instance.OpenSettings();
    }
}
