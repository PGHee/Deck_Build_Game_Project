using UnityEngine;
using UnityEditor;
// Card.cs가 들어있는 프리팹에서 지정된 ActionType에 따라 표시되는 변수를 달리하기 위해 존재하는 에디터 파일
[CustomPropertyDrawer(typeof(CardAction))]
public class CardActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var actionType = property.FindPropertyRelative("actionType");
        var value = property.FindPropertyRelative("value");
        var secondaryValue = property.FindPropertyRelative("secondaryValue");
        var killEffectType = property.FindPropertyRelative("killEffectType");
        var killEffectValue = property.FindPropertyRelative("killEffectValue");

        Rect actionTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect secondaryValueRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect killEffectTypeRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 3, position.width, EditorGUIUtility.singleLineHeight);
        Rect killEffectValueRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 4, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(actionTypeRect, actionType);
        EditorGUI.PropertyField(valueRect, value);

        if (actionType.enumValueIndex == (int)CardActionType.RandomTargetDamage || 
            actionType.enumValueIndex == (int)CardActionType.StunCheckDamage || 
            actionType.enumValueIndex == (int)CardActionType.PoisonCheckDamage)
        {
            EditorGUI.PropertyField(secondaryValueRect, secondaryValue);
        }

        if (actionType.enumValueIndex == (int)CardActionType.killEffect)
        {
            EditorGUI.PropertyField(killEffectTypeRect, killEffectType);
            EditorGUI.PropertyField(killEffectValueRect, killEffectValue);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var actionType = property.FindPropertyRelative("actionType");

        if (actionType.enumValueIndex == (int)CardActionType.RandomTargetDamage || 
            actionType.enumValueIndex == (int)CardActionType.StunCheckDamage || 
            actionType.enumValueIndex == (int)CardActionType.PoisonCheckDamage)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 6;
        }
        else if (actionType.enumValueIndex == (int)CardActionType.killEffect)
        {
            return EditorGUIUtility.singleLineHeight * 5 + 8;
        }

        return EditorGUIUtility.singleLineHeight * 2 + 4;
    }
}
