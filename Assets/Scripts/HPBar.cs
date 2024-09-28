using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image hpFillImage;
    public TextMeshProUGUI healthText;          // 체력을 표시할 Text
    public Transform iconTransform;             // 아이콘의 Transform
    public TextMeshProUGUI iconText;            // 아이콘에 표시할 Text
    public SpriteRenderer iconRenderer;         // shield의 SpriteRenderer

    public Sprite HPSprite;                     // 빨간색 HP바 스프라이트
    public Sprite shieldHPSprite;               // 파란색 HP바 스프라이트
    public Sprite poisonHPSprite;               // 보라색 HP바 스프라이트

    public Sprite shieldSprite;                 // shield 스프라이트
    public Sprite poisonSprite;                 // poison 스프라이트
    public Sprite poisonSheildSprite;           // poison + shield 스프라이트

    public Transform buffIconPanel;             // 버프 아이콘 패널
    public Transform debuffIconPanel;           // 디버프 아이콘 패널

    // Initialize 메서드
    public void Initialize(Transform target, int maxHealth, int shield, int poison, Vector3 offset)
    {
        transform.SetParent(target);
        transform.localPosition = offset + new Vector3(0, -2.5f, 0);
        UpdateHealth(maxHealth, maxHealth, shield, poison);
    }

    // UpdateHealth 메서드
    public void UpdateHealth(int currentHealth, int maxHealth, int shield, int poison)
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        hpFillImage.fillAmount = healthPercentage;

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }

        if (shield > 0)
        {
            iconTransform.gameObject.SetActive(true);
            iconText.gameObject.SetActive(true);
            if (poison > 0)
            {
                iconText.text = $"{shield}";
                healthText.text = $"{currentHealth} / {maxHealth} <color=#66FF66>(-{poison})</color>";
                iconRenderer.sprite = poisonSheildSprite;   // poison + shield 스프라이트
            }
            else
            {
                iconText.text = $"{shield}";
                iconRenderer.sprite = shieldSprite;         // shield 스프라이트
            }
            hpFillImage.sprite = shieldHPSprite;         // 쉴드가 있을 때 파란색 HP바 스프라이트 사용
        }
        else if (poison > 0)
        {
            iconTransform.gameObject.SetActive(true);
            iconText.gameObject.SetActive(true);
            healthText.text = $"{currentHealth} / {maxHealth} <color=#66FF66>(-{poison})</color>";
            iconRenderer.sprite = poisonSprite;             // poison 스프라이트
            hpFillImage.sprite = poisonHPSprite;         // 중독 상태일 때 보라색 HP바 스프라이트 사용
        }
        else
        {
            iconTransform.gameObject.SetActive(false);
            iconText.gameObject.SetActive(false);
            hpFillImage.sprite = HPSprite;               // 쉴드가 없을 때 빨간색 HP바 스프라이트 사용
        }
    }
}
