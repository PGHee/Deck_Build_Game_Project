using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CircleBar : MonoBehaviour
{
    public Image circleFill;                   // 채워진 서클 바의 Image 컴포넌트
    public TextMeshProUGUI circleText;         // 서클을 표시할 Text

    private void Awake()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 41;
        }
    }

    // Initialize 메서드
    public void Initialize(Transform target, int maxCircle, Vector3 offset)
    {
        transform.SetParent(target);
        transform.localPosition = offset + new Vector3(0, -2.0f, 0);
        UpdateCircle(maxCircle, maxCircle);
    }

    // UpdateCircle 메서드
    public void UpdateCircle(int currentCircle, int maxCircle)
    {
        if (circleText != null) circleText.text = $"{currentCircle} / {maxCircle}";
        float circlePercentage = (float)currentCircle / maxCircle;
        circleFill.fillAmount = circlePercentage;
    }
}
