using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager instance = null;

    // ����� Ŭ�� ����Ʈ (ȿ����)
    public AudioClip PanelOpen;
    public AudioClip PanelClose;
    public AudioClip DragCard;

    private AudioSource audioSource;

    // �̱��� ���� ����
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

        DontDestroyOnLoad(gameObject); // ���� ����Ǿ ������� �ʵ��� ����
        audioSource = GetComponent<AudioSource>();
    }

    // ��Ȳ�� ���� �ٸ� ȿ������ ����ϴ� �޼ҵ�
    public void PlaySound(string situation)
    {

        if (audioSource.isPlaying)
        {
            audioSource.Stop();  // 현재 재생 중인 사운드를 중지
        }

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
            case "UI":
                break;
            case "Shop":
                break;
            case "ArtipactSYS":
                break;
            default:
                Debug.LogWarning("�� �� ���� ��Ȳ: " + situation);
                break;
        }
    }
}
