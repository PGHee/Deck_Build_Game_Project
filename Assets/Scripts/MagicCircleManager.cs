using UnityEngine;

public class MagicCircleManager : MonoBehaviour
{
    public SpriteRenderer[] magicCircles = new SpriteRenderer[3]; // 세 개의 마법진 이미지 슬롯
    public float[] rotationSpeeds = { 30f, -20f, 10f }; // 회전 속도, 정방향과 반대방향 혼합

    private PlayerState playerState;

    private void Start()
    {
        playerState = FindObjectOfType<PlayerState>();

        // 마법진 초기화
        InitializeMagicCircles();
    }

    private void Update()
    {
        RotateMagicCircles();
    }

    // 마법진을 초기화하고 색상 및 크기를 설정하는 함수
    private void InitializeMagicCircles()
    {
        // 모든 마법진 비활성화
        foreach (var magicCircle in magicCircles)
        {
            magicCircle.gameObject.SetActive(false);
        }
    }

    // 마법진의 색상과 크기를 갱신하는 함수
    public void UpdateMagicCircle()
    {
        PlayerState.AttributeType[] topAttributes = GetTopTwoAttributes();
        
        Color primaryColor = GetColorForAttribute(topAttributes[0]);
        Color secondaryColor = GetColorForAttribute(topAttributes[1]);

        if (playerState.attributeMastery[topAttributes[0]] >= 4)
        {
            magicCircles[0].gameObject.SetActive(true);
            if (playerState.attributeMastery[topAttributes[0]] < 7)
            {
                magicCircles[0].color = primaryColor;
                magicCircles[0].transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                magicCircles[0].color = secondaryColor;
                magicCircles[0].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                magicCircles[1].gameObject.SetActive(true);
                magicCircles[1].color = primaryColor;
                magicCircles[1].transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        if (playerState.attributeMastery[topAttributes[0]] >= 9)
        {
            magicCircles[2].gameObject.SetActive(true);
            Material gradientMaterial = magicCircles[2].material;
            gradientMaterial.SetColor("_TopColor", primaryColor);
            gradientMaterial.SetColor("_BottomColor", secondaryColor);
            magicCircles[2].transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        }
    }


    // 마법진의 회전을 처리하는 함수
    private void RotateMagicCircles()
    {
        for (int i = 0; i < magicCircles.Length; i++)
        {
            if (magicCircles[i].gameObject.activeSelf)
            {
                magicCircles[i].transform.Rotate(Vector3.forward, rotationSpeeds[i] * Time.deltaTime);
            }
        }
    }

    // 가장 높은 두 개의 숙련도를 가진 속성을 반환
    private PlayerState.AttributeType[] GetTopTwoAttributes()
    {
        PlayerState.AttributeType[] topAttributes = new PlayerState.AttributeType[2];
        int highestLevel = 0, secondHighestLevel = 0;

        foreach (var attribute in playerState.attributeMastery)
        {
            if (attribute.Value > highestLevel)
            {
                secondHighestLevel = highestLevel;
                topAttributes[1] = topAttributes[0];

                highestLevel = attribute.Value;
                topAttributes[0] = attribute.Key;
            }
            else if (attribute.Value > secondHighestLevel)
            {
                secondHighestLevel = attribute.Value;
                topAttributes[1] = attribute.Key;
            }
        }

        return topAttributes;
    }

    // 속성에 따른 색상 반환 메서드
    private Color GetColorForAttribute(PlayerState.AttributeType attribute)
    {
        switch (attribute)
        {
            case PlayerState.AttributeType.Fire: return Color.red;
            case PlayerState.AttributeType.Water: return Color.blue;
            case PlayerState.AttributeType.Wood: return Color.green;
            case PlayerState.AttributeType.Metal: return Color.gray;
            case PlayerState.AttributeType.Earth: return new Color(0.6f, 0.3f, 0.1f); // 갈색
            case PlayerState.AttributeType.Lightning: return Color.yellow;
            case PlayerState.AttributeType.Wind: return Color.cyan;
            case PlayerState.AttributeType.Light: return Color.white;
            case PlayerState.AttributeType.Dark: return Color.black;
            case PlayerState.AttributeType.Void: return new Color(0.5f, 0f, 0.5f); // 보라색
            default: return Color.white;
        }
    }

    // 두 색상을 그라데이션한 색상 반환
    private Color GetGradientColor(Color color1, Color color2)
    {
        return Color.Lerp(color1, color2, 0.5f); // 중간 색상으로 그라데이션
    }
}
