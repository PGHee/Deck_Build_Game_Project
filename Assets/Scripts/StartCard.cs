using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartCard : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private int originalLayer;
    public Transform dropZone; // 드롭존 지정
    public GameObject[] cutScenes; // 컷신 오브젝트들 (4개)
    private bool isCutscenePlaying = false; // 컷신이 재생 중인지 여부

    void Start()
    {
        startPosition = transform.position;
        originalLayer = gameObject.layer;
    }

    public void OnDrag(PointerEventData eventData)          // 카드를 드래그했을 때의 동작
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
        newPosition.z = 0; // z축 값을 0으로 고정
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)       // 카드의 드래그를 마쳤을 때의 동작
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, -Camera.main.transform.position.z));
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        gameObject.layer = originalLayer;

        // 드롭존에 카드가 닿았는지 확인
        if (Vector3.Distance(transform.position, dropZone.position) < 1.0f)
        {
            if (!isCutscenePlaying)  // 컷신이 아직 시작되지 않았다면
            {
                StartCoroutine(PlayCutscenesAndLoadGame());
            }
        }

        // 카드 원위치 복구
        transform.position = startPosition;
    }

    // 컷신을 순차적으로 재생하고 게임 씬 로드
    IEnumerator PlayCutscenesAndLoadGame()
    {
        isCutscenePlaying = true;  // 컷신 재생 시작
        cutScenes[0].SetActive(true);
        for (int i = 1; i < cutScenes.Length; i++)
        {
            cutScenes[i].SetActive(true); // 각 컷신을 활성화
            yield return new WaitForSeconds(3); // 3초 동안 컷신 재생 (원하는 시간으로 조정 가능)
            cutScenes[i].SetActive(false); // 컷신 비활성화
        }

        // 컷신이 끝나면 게임 씬 비동기로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");
        while (!asyncLoad.isDone)
        {
            yield return null; // 로딩이 끝날 때까지 대기
        }

        isCutscenePlaying = false;
    }
}
