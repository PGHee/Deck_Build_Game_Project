using UnityEngine;
using TMPro;

public class CircleBar : MonoBehaviour
{
    public Transform circleFill;                // 채워진 서클 바
    public TextMeshProUGUI circleText;          // 서클을 표시할 Text

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
        circleFill.localScale = new Vector3(circlePercentage, 1, 1);
        circleFill.localPosition = new Vector3(-(1 - circlePercentage) * 1.0f, circleFill.localPosition.y, circleFill.localPosition.z);
    }
}
