using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Sprite[] backgroundSprites;  // 5개의 배경 이미지를 담는 배열
    public Sprite[] fieldMagicSprites;  // 필드 마법용 배경 스프라이트 (8종)
    public AudioClip[] backgroundMusic; // 스테이지마다 사용할 배경음 배열 (5종)
    private SpriteRenderer background;
    private AudioSource audioSource;

    private void Start()
    {
        background = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        AdjustBackgroundScale();
    }

    private void AdjustBackgroundScale()
    {
        Camera mainCamera = Camera.main;    // 카메라의 뷰포트를 기준으로 크기 조정
        Vector2 spriteSize = background.sprite.bounds.size; // 배경 이미지의 크기를 얻기 위해 SpriteRenderer의 bounds 사용

        // 화면의 높이와 너비 계산
        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        
        // 화면 크기와 스프라이트 크기 비교 후, 스케일 조정
        Vector3 scale = transform.localScale;
        scale.x = screenWidth / spriteSize.x;
        scale.y = screenHeight / spriteSize.y;
        transform.localScale = scale;
    }

    // 맵 이동 시 배경화면과 배경음을 변경하는 함수
    public void ChangeBackground(int stageIndex)
    {
        // 스테이지 인덱스에 맞는 배경 이미지로 교체
        if (stageIndex >= 0 && stageIndex < backgroundSprites.Length)
        {
            background.sprite = backgroundSprites[stageIndex];
        }

        // 배경음악 변경
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

    // 필드 마법 발동 시 배경을 필드 마법용 이미지로 변경하는 함수
    public void ActivateFieldMagic(int fieldMagicIndex)
    {
        if (fieldMagicIndex >= 0 && fieldMagicIndex < fieldMagicSprites.Length)
        {
            background.sprite = fieldMagicSprites[fieldMagicIndex];
        }
    }
}
