using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Sprite[] backgroundSprites;      // 스테이지마다 사용할 배경 이미지 배열
    public AudioClip[] backgroundMusic;     // 스테이지마다 사용할 배경음 배열
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Start()
    {
        // Background 오브젝트의 SpriteRenderer와 AudioSource 컴포넌트를 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // 배경화면과 배경음을 변경하는 함수
    public void ChangeBackground(int stageIndex)
    {
        // 스테이지 인덱스에 맞는 배경 이미지로 교체
        if (stageIndex >= 0 && stageIndex < backgroundSprites.Length) spriteRenderer.sprite = backgroundSprites[stageIndex];

        // 스테이지 인덱스에 맞는 배경음으로 교체, 이미 같은 음악이 재생 중인 경우는 무시
        if (stageIndex >= 0 && stageIndex < backgroundMusic.Length)
        {
            if (audioSource.clip != backgroundMusic[stageIndex])
            {
                audioSource.clip = backgroundMusic[stageIndex];
                audioSource.Play();  // 새로운 배경음 재생
            }
            else if (!audioSource.isPlaying)
            {
                // 같은 배경음이지만 정지 상태면 다시 재생
                audioSource.Play();
            }
        }
    }
}
