using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager instance = null;

    // 오디오 클립 리스트 (효과음)
    public AudioClip PanelOpen;
    public AudioClip PanelClose;
    public AudioClip DragCard;

    private AudioSource audioSource;

    // 싱글톤 패턴 설정
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // 씬이 변경되어도 사라지지 않도록 설정
        audioSource = GetComponent<AudioSource>();
    }

    // 상황에 따라 다른 효과음을 재생하는 메소드
    public void PlaySound(string situation)
    {
        switch (situation)
        {
            case "PanelOpen":
                audioSource.PlayOneShot(PanelOpen);
                break;
            case "PanelClose":
                audioSource.PlayOneShot(PanelClose);
                break;
            case "DragCard":
                audioSource.PlayOneShot(DragCard);
                break;
            default:
                Debug.LogWarning("알 수 없는 상황: " + situation);
                break;
        }
    }
}
