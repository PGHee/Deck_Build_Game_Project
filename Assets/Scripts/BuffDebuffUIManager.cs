using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffUIManager : MonoBehaviour
{
    private Dictionary<EffectType, GameObject> iconPrefabs = new Dictionary<EffectType, GameObject>();
    private Dictionary<Transform, List<GameObject>> activeBuffIcons = new Dictionary<Transform, List<GameObject>>();
    private Dictionary<Transform, List<GameObject>> activeDebuffIcons = new Dictionary<Transform, List<GameObject>>();

    public float iconSpacing = 30f;

    private void Awake()
    {
        LoadPrefabs(iconPrefabs, "Icons");
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
            if (prefab != null)
            {
                prefabDictionary[effectType] = prefab;
            }
            else
            {
                Debug.LogError($"Failed to load prefab at {prefabPath}");
            }
        }
    }

    public void AddBuffIcon(Transform panel, EffectType effectType)
    {
        if (!iconPrefabs.ContainsKey(effectType))
        {
            Debug.LogError($"No buff icon prefab for effect type: {effectType}");
            return;
        }

        if (!activeBuffIcons.ContainsKey(panel))
        {
            activeBuffIcons[panel] = new List<GameObject>();
        }

        if (!IconExists(activeBuffIcons[panel], effectType))
        {
            GameObject buffIcon = Instantiate(iconPrefabs[effectType], panel);
            activeBuffIcons[panel].Add(buffIcon);
            ReorganizeIcons(panel, activeBuffIcons[panel]);
        }
    }

    public void AddDebuffIcon(Transform panel, EffectType effectType)
    {
        if (!iconPrefabs.ContainsKey(effectType))
        {
            Debug.LogError($"No debuff icon prefab for effect type: {effectType}");
            return;
        }

        if (!activeDebuffIcons.ContainsKey(panel))
        {
            activeDebuffIcons[panel] = new List<GameObject>();
        }

        if (!IconExists(activeDebuffIcons[panel], effectType))
        {
            GameObject debuffIcon = Instantiate(iconPrefabs[effectType], panel);
            activeDebuffIcons[panel].Add(debuffIcon);
            ReorganizeIcons(panel, activeDebuffIcons[panel]);
        }
    }

    public void RemoveBuffIcon(Transform panel, EffectType effectType)
    {
        RemoveIcon(activeBuffIcons, panel, effectType);
    }

    public void RemoveDebuffIcon(Transform panel, EffectType effectType)
    {
        RemoveIcon(activeDebuffIcons, panel, effectType);
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

    private void ReorganizeIcons(Transform panel, List<GameObject> icons)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            RectTransform iconTransform = icons[i].GetComponent<RectTransform>();
            if (iconTransform != null)
            {
                iconTransform.anchoredPosition = new Vector2(i * iconSpacing, 0);
            }
            else
            {
                Debug.LogError($"Icon {icons[i].name} does not have a RectTransform component.");
            }
        }
    }
}
