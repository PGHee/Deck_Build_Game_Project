using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    // 각 레이어별로 5개의 배경 이미지를 담는 배열
    public Sprite[] backgroundLayerSprites0; // 레이어 0: 기본 배경 이미지
    public Sprite[] backgroundLayerSprites1; // 레이어 1: 바닥(파운데이션) 이미지
    public Sprite[] backgroundLayerSprites2; // 레이어 2: 바닥 앞 데코 이미지
    public Sprite[] backgroundLayerSprites3; // 레이어 3: 바닥 뒤 데코 이미지
    public Sprite[] backgroundLayerSprites4; // 레이어 4: 바닥 위 데코 이미지

    // 필드 마법용 배경 스프라이트 (8종)
    public Sprite[] fieldMagicSprites;

    // 스테이지마다 사용할 배경음 배열 (5종)
    public AudioClip[] backgroundMusic;

    // 배경을 담당하는 오브젝트들의 SpriteRenderer 컴포넌트 배열
    public SpriteRenderer[] layerSpriteRenderers;
    private AudioSource audioSource;

    // 현재 스테이지 기본 배경 정보 저장
    private Sprite[] currentBackgroundSprites = new Sprite[5]; // 기본 배경을 임시 저장할 배열

    private void Start()
    {
        // Background 오브젝트의 AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();
    }

    // 맵 이동 시 배경화면과 배경음을 변경하는 함수 (필드 마법 발동 시 호출되지 않음)
    public void ChangeBackground(int stageIndex)
    {
        // 스테이지 인덱스에 맞는 배경 이미지로 각 레이어를 교체
        if (stageIndex >= 0 && stageIndex < backgroundLayerSprites0.Length)
        {
            layerSpriteRenderers[0].sprite = backgroundLayerSprites0[stageIndex];
            currentBackgroundSprites[0] = backgroundLayerSprites0[stageIndex];  // 기본 배경 저장
        }
        
        if (stageIndex >= 0 && stageIndex < backgroundLayerSprites1.Length)
        {
            layerSpriteRenderers[1].sprite = backgroundLayerSprites1[stageIndex];
            currentBackgroundSprites[1] = backgroundLayerSprites1[stageIndex];  // 기본 배경 저장
        }
        
        if (stageIndex >= 0 && stageIndex < backgroundLayerSprites2.Length)
        {
            layerSpriteRenderers[2].sprite = backgroundLayerSprites2[stageIndex];
            currentBackgroundSprites[2] = backgroundLayerSprites2[stageIndex];  // 기본 배경 저장
        }
        
        if (stageIndex >= 0 && stageIndex < backgroundLayerSprites3.Length)
        {
            layerSpriteRenderers[3].sprite = backgroundLayerSprites3[stageIndex];
            currentBackgroundSprites[3] = backgroundLayerSprites3[stageIndex];  // 기본 배경 저장
        }
        
        if (stageIndex >= 0 && stageIndex < backgroundLayerSprites4.Length)
        {
            layerSpriteRenderers[4].sprite = backgroundLayerSprites4[stageIndex];
            currentBackgroundSprites[4] = backgroundLayerSprites4[stageIndex];  // 기본 배경 저장
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
            // 각 레이어에 필드 마법용 스프라이트 적용
            for (int i = 0; i < 5 && i < fieldMagicSprites.Length; i++)
            {
                layerSpriteRenderers[i].sprite = fieldMagicSprites[fieldMagicIndex];
            }
        }
    }

    // 필드 마법 종료 시 원래 기본 배경으로 되돌리는 함수
    public void DeactivateFieldMagic()
    {
        // 필드 마법 종료 시 기본 배경 이미지로 복구
        for (int i = 0; i < 5; i++)
        {
            layerSpriteRenderers[i].sprite = currentBackgroundSprites[i];
        }
    }
}
