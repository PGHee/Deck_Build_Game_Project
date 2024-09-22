using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class ShopButtonManager : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public int cardPrefabNum;
    public int artifactPrefabNum;

    private Vector3 fixedScreenPosition = new Vector3(0.9f, 0.55f, 10f);
    private GameObject zoomedCard;
    public GameObject originalPrefab;

    public Vector2 canvasSize = new Vector2(0, 0);
    public Vector3 canvasPosition = new Vector3(0, 0, 0);

    private int baseLayer = 710;

    // Start is called before the first frame update
    void Start()
    {
        //cardPrefabNum = 9999;
        //artifactPrefabNum = 9999;
    }

    // Update is called once per frame
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (artifactPrefabNum)
        {
            case 9999:  // If shop item is card
                originalPrefab = Resources.Load<GameObject>($"Prefabs/Card/{CardNameConverter.CardNumToCode(cardPrefabNum)}");

                if (originalPrefab != null)
                {
                    zoomedCard = Instantiate(originalPrefab);   // 원본 프리팹을 복제하여 확대된 카드로 사용

                    // 화면의 특정 위치에 고정 (스크린 좌표를 월드 좌표로 변환)
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * fixedScreenPosition.x, Screen.height * fixedScreenPosition.y, fixedScreenPosition.z));
                    zoomedCard.transform.position = worldPosition;
                    zoomedCard.transform.localScale = zoomedCard.transform.localScale * 1.5f; // 크기 조정

                    Canvas canvas = zoomedCard.GetComponentInChildren<Canvas>();
                    if (canvas != null)
                    {
                        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            // 크기와 위치를 설정합니다.
                            rectTransform.sizeDelta = rectTransform.sizeDelta * 0.5f; //canvasSize;
                            rectTransform.localPosition = canvasPosition;
                        }
                    }

                    SetLayerOrders(zoomedCard);
                }
                break;

            default:    // If shop item is Artifact

                break;
        }       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (zoomedCard != null) // 마우스가 카드에서 벗어나면 확대된 카드 제거
        {
            Destroy(zoomedCard);
        }
    }

    void SetCardPositions()
    {
        Transform cardAttribute = transform.Find("CardFront/CardAttribute");
        cardAttribute.localPosition = new Vector3(2.06f, 3f, 0f);

        Transform cardSprite = transform.Find("CardFront/CardImage");
        cardSprite.localPosition = new Vector3(0f, 0.5f, 0f);

        Transform cardCost = transform.Find("CardFront/Canvas/CardCost");
        cardCost.localPosition = new Vector3(-2f, 2.25f, 0f);
        TextMeshProUGUI costText = cardCost.GetComponent<TextMeshProUGUI>();
        costText.fontSize = 0.75f;
        RectTransform costRectTransform = cardCost.GetComponent<RectTransform>();
        costRectTransform.sizeDelta = new Vector2(1f, 1f);

        Transform cardType = transform.Find("CardFront/Canvas/CardType");
        cardType.localPosition = new Vector3(-2f, 1.25f, 0f);
        TextMeshProUGUI typeText = cardType.GetComponent<TextMeshProUGUI>();
        typeText.fontSize = 0.25f;
        RectTransform typeRectTransform = cardType.GetComponent<RectTransform>();
        typeRectTransform.sizeDelta = new Vector2(1.5f, 1.5f);

        Transform cardText = transform.Find("CardFront/Canvas/CardName");
        cardText.localPosition = new Vector3(0f, 3f, 0f);
        TextMeshProUGUI nameText = cardText.GetComponent<TextMeshProUGUI>();
        nameText.fontSize = 0.5f;
        RectTransform nameRectTransform = cardText.GetComponent<RectTransform>();
        nameRectTransform.sizeDelta = new Vector2(3.5f, 0.75f);

        Transform cardDescription = transform.Find("CardFront/Canvas/CardDescription");
        cardDescription.localPosition = new Vector3(0f, -2.75f, 0f);
        TextMeshProUGUI descriptionText = cardDescription.GetComponent<TextMeshProUGUI>();
        descriptionText.fontSize = 0.25f;
        RectTransform descriptionRectTransform = cardDescription.GetComponent<RectTransform>();
        descriptionRectTransform.sizeDelta = new Vector2(4.5f, 1.75f);
    }

    void SetLayerOrders(GameObject card)
    {
        Transform backSide = card.transform.Find("CardBack");
        if (backSide != null) SetSortingOrder(backSide.gameObject, baseLayer + 1);

        Transform frontImage = card.transform.Find("CardFront/CardImage");
        if (frontImage != null) SetSortingOrder(frontImage.gameObject, baseLayer + 2);

        Transform frontSide = card.transform.Find("CardFront");
        if (frontSide != null) SetSortingOrder(frontSide.gameObject, baseLayer + 3);

        Transform canvas = card.transform.Find("CardFront/Canvas");
        if (canvas != null) SetSortingOrder(canvas.gameObject, baseLayer + 4);

        Transform attributeImage = card.transform.Find("CardFront/CardAttribute");
        if (canvas != null) SetSortingOrder(attributeImage.gameObject, baseLayer + 4);
    }

    void SetSortingOrder(GameObject obj, int order)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.sortingOrder = order;

        Canvas canvas = obj.GetComponent<Canvas>();
        if (canvas != null) canvas.sortingOrder = order;
    }
}
