using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Sprite[] backgroundSprites;  // 5개의 배경 이미지를 담는 배열
    public Sprite[] fieldMagicSprites;  // 필드 마법용 배경 스프라이트 (8종)
    public AudioClip[] backgroundMusic; // 스테이지마다 사용할 배경음 배열 (5종)
    public AudioClip[] bossMusic;       // 보스 전투용 배경음악 배열 (5종)
    public AudioClip rewardSound;       // 보상 효과음 (1회 재생)

    private SpriteRenderer background;
    private AudioSource audioSource;
    public AudioSource sfxAudioSource;  // 효과음용 AudioSource
    public float fadeDuration = 1f;      // 페이드 효과 지속 시간

    private void Start()
    {
        background = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // 맵 이동 시 배경화면과 배경음을 변경하는 함수
    public void ChangeBackground(int stageIndex)
    {
        // 스테이지 인덱스에 맞는 배경 이미지로 교체
        if (stageIndex >= 0 && stageIndex < backgroundSprites.Length)
        {
            background.sprite = backgroundSprites[stageIndex];
        }

        // 배경음악 변경 (페이드 인/아웃 효과 적용)
        if (stageIndex >= 0 && stageIndex < backgroundMusic.Length)
        {
            StartCoroutine(FadeInNewMusic(backgroundMusic[stageIndex]));
        }
    }

    // 필드 마법 발동 시 배경을 필드 마법용 이미지로 변경하는 함수
    public void ActivateFieldMagic(int fieldMagicIndex)
    {
        if (fieldMagicIndex >= 0 && fieldMagicIndex < fieldMagicSprites.Length)
        {
            background.sprite = fieldMagicSprites[fieldMagicIndex];
        }
    }

    // 배경음악 페이드 인/아웃 처리하는 코루틴
    private IEnumerator FadeInNewMusic(AudioClip newMusic)
    {
        // 현재 재생 중인 배경음과 새로운 배경음이 동일한 경우 페이드 처리 생략
        if (audioSource.clip == newMusic && audioSource.isPlaying)
        {
            yield break; // 코루틴을 종료하여 기존 음악이 계속 재생되도록 함
        }

        // 현재 재생 중인 배경음 페이드 아웃
        if (audioSource.isPlaying)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }
            audioSource.Stop();
        }

        // 새로운 배경음 재생 및 페이드 인
        audioSource.clip = newMusic;
        audioSource.Play();
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
    }

    // 보스 전투용 배경음악으로 교체하는 함수
    public void ChangeToBossMusic(int stageIndex)
    {
        // 스테이지 인덱스에 맞는 보스 전투용 배경음악으로 교체 (페이드 인/아웃 효과 적용)
        if (stageIndex >= 0 && stageIndex < bossMusic.Length)
        {
            StartCoroutine(FadeInNewMusic(bossMusic[stageIndex]));
        }
    }
    
    public void PlayRewardSound()
    {
        if (rewardSound != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(rewardSound);
        }
    }
}
