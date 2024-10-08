using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffUIManager : MonoBehaviour
{
    private Dictionary<EffectType, GameObject> iconPrefabs = new Dictionary<EffectType, GameObject>();
    private Dictionary<PassiveType, GameObject> passiveIconPrefabs = new Dictionary<PassiveType, GameObject>();
    private Dictionary<Transform, List<GameObject>> activeBuffIcons = new Dictionary<Transform, List<GameObject>>();
    private Dictionary<Transform, List<GameObject>> activeDebuffIcons = new Dictionary<Transform, List<GameObject>>();
    private Dictionary<Transform, List<GameObject>> activePassiveIcons = new Dictionary<Transform, List<GameObject>>();

    public float iconSpacing = 30f;

    private void Awake()
    {
        LoadPrefabs(iconPrefabs, "Icons");
        LoadPassivePrefabs(passiveIconPrefabs, "Icons");
    }

    private void LoadPrefabs(Dictionary<EffectType, GameObject> prefabDictionary, string folderPath)
    {
        foreach (EffectType effectType in System.Enum.GetValues(typeof(EffectType)))
        {
            if (effectType == EffectType.Purification || effectType == EffectType.RandomAction || effectType == EffectType.Field)
            {
                continue; // 즉발성 효과나 아이콘이 필요 없는 타입은 제외
            }

            string prefabPath = $"Prefabs/{folderPath}/{effectType}Prefab";
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab != null) prefabDictionary[effectType] = prefab;
        }
    }

    private void LoadPassivePrefabs(Dictionary<PassiveType, GameObject> prefabDictionary, string folderPath)
    {
        foreach (PassiveType passiveType in System.Enum.GetValues(typeof(PassiveType)))
        {
            string prefabPath = $"Prefabs/{folderPath}/{passiveType}Prefab";
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab != null) prefabDictionary[passiveType] = prefab;
        }
    }

    public GameObject AddBuffIcon(Transform panel, EffectType effectType)
    {
        if (!activeBuffIcons.ContainsKey(panel)) activeBuffIcons[panel] = new List<GameObject>();
        if (!IconExists(activeBuffIcons[panel], effectType))
        {
            GameObject buffIcon = Instantiate(iconPrefabs[effectType], panel);
            activeBuffIcons[panel].Add(buffIcon);
            ReorganizeIcons(panel, activeBuffIcons[panel]);

            return buffIcon;  // 생성된 아이콘을 반환
        }

        return null;  // 아이콘이 이미 존재하면 null 반환
    }

    public GameObject AddDebuffIcon(Transform panel, EffectType effectType)
    {
        if (!activeDebuffIcons.ContainsKey(panel)) activeDebuffIcons[panel] = new List<GameObject>();
        if (!IconExists(activeDebuffIcons[panel], effectType))
        {
            GameObject debuffIcon = Instantiate(iconPrefabs[effectType], panel);
            activeDebuffIcons[panel].Add(debuffIcon);
            ReorganizeIcons(panel, activeDebuffIcons[panel]);

            return debuffIcon;  // 생성된 아이콘을 반환
        }

        return null;  // 아이콘이 이미 존재하면 null 반환
    }

    public GameObject AddPassiveIcon(Transform panel, PassiveType passiveType)
    {
        if (!activePassiveIcons.ContainsKey(panel)) activePassiveIcons[panel] = new List<GameObject>();
        if (!PassiveIconExists(activePassiveIcons[panel], passiveType))
        {
            GameObject passiveIcon = Instantiate(passiveIconPrefabs[passiveType], panel);
            activePassiveIcons[panel].Add(passiveIcon);
            ReorganizeIcons(panel, activePassiveIcons[panel], true);

            return passiveIcon;  // 생성된 아이콘을 반환
        }

        return null;  // 아이콘이 이미 존재하면 null 반환
    }

    private bool PassiveIconExists(List<GameObject> icons, PassiveType passiveType)
    {
        return icons.Exists(icon => icon.name.Contains(passiveType.ToString()));
    }

    public void RemoveBuffIcon(Transform panel, EffectType effectType)
    {
        RemoveIcon(activeBuffIcons, panel, effectType);
    }

    public void RemoveDebuffIcon(Transform panel, EffectType effectType)
    {
        RemoveIcon(activeDebuffIcons, panel, effectType);
    }

    public void RemovePassiveIcon(Transform panel, PassiveType passiveType)
    {
        if (activePassiveIcons.ContainsKey(panel))
        {
            GameObject iconToRemove = activePassiveIcons[panel].Find(icon => icon.name.Contains(passiveType.ToString()));
            if (iconToRemove != null)
            {
                activePassiveIcons[panel].Remove(iconToRemove);
                Destroy(iconToRemove);
                ReorganizeIcons(panel, activePassiveIcons[panel]);
            }
        }
    }

    private bool IconExists(List<GameObject> icons, EffectType effectType)
    {
        return icons.Exists(icon => icon.name.Contains(effectType.ToString()));
    }

    private void RemoveIcon(Dictionary<Transform, List<GameObject>> iconDictionary, Transform panel, EffectType effectType)
    {
        if (iconDictionary.ContainsKey(panel))
        {
            GameObject iconToRemove = iconDictionary[panel].Find(icon => icon.name.Contains(effectType.ToString()));
            if (iconToRemove != null)
            {
                iconDictionary[panel].Remove(iconToRemove);
                Destroy(iconToRemove);
                ReorganizeIcons(panel, iconDictionary[panel]);
            }
        }
    }

    private void ReorganizeIcons(Transform panel, List<GameObject> icons, bool isPassive = false)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            RectTransform iconTransform = icons[i].GetComponent<RectTransform>();
            if (iconTransform != null)
            {
                float offset = isPassive ? -0.5f : 0f;
                iconTransform.anchoredPosition = new Vector2(i * iconSpacing + offset, - offset);
            }
        }
    }
}
