using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelResizer : MonoBehaviour
{
    // 기준 해상도 비율 설정
    private Vector2 referenceResolution = new Vector2(1920, 1080);

    // 패널의 Transform을 가져올 변수
    public Transform panelTransform;

    // 시작 시 크기 조정을 수행
    void Start()
    {
        AdjustPanelSize();
    }

    // 해상도에 따라 패널 크기만 조정하는 함수
    public void AdjustPanelSize()
    {
        // 현재 화면 해상도와 기준 해상도의 비율 계산
        float widthRatio = Screen.width / referenceResolution.x;
        float heightRatio = Screen.height / referenceResolution.y;
        float scaleRatio = Mathf.Min(widthRatio, heightRatio); // 종횡비 유지를 위해 최소값 사용

        // 패널의 scale만 조정하여 현재 위치를 유지하면서 크기만 변경
        panelTransform.localScale = new Vector3(scaleRatio, scaleRatio, 1);
    }
}
