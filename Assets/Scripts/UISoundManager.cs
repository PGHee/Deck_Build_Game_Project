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
    public AudioClip CoinSound;
    public AudioClip UIClick;
    public AudioClip ArtifactSys;
    public AudioClip StartDeckStoneOn;

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
        if(instance == null) return;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("AudioSource component is missing from UISoundManager.");
                return;
            }
        }
        
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
                audioSource.PlayOneShot(UIClick);
                break;
            case "Shop":
                audioSource.PlayOneShot(CoinSound);
                break;
            case "ArtifactSYS":
                audioSource.PlayOneShot(ArtifactSys);
                break;
            case "StoneOn":
                audioSource.PlayOneShot(StartDeckStoneOn);
                break;
            default:
                Debug.LogWarning(situation);
                break;
        }
    }
}
