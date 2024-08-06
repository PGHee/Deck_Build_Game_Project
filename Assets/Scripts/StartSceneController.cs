using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    // 게임 시작 버튼을 클릭하면 호출되는 메서드
    public void StartGame()
    {
        // 메인 게임 씬의 이름을 적어주세요.
        SceneManager.LoadScene("SampleScene");
    }

    // 종료 버튼을 클릭하면 호출되는 메서드
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
