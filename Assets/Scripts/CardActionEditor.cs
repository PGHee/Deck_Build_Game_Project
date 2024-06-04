using UnityEngine;
using UnityEditor;
// Card.cs가 들어있는 프리팹에서 지정된 ActionType에 따라 표시되는 변수를 달리하기 위해 존재하는 에디터 파일
[CustomPropertyDrawer(typeof(CardAction))]
public class CardActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)    // 행동 방식 별 추가로 표기될 변수
    {
        EditorGUI.BeginProperty(position, label, property);

        var actionType = property.FindPropertyRelative("actionType");
        var value = property.FindPropertyRelative("value");
        var secondaryValue = property.FindPropertyRelative("secondaryValue");
        var killEffectType = property.FindPropertyRelative("killEffectType");
        var killEffectValue = property.FindPropertyRelative("thirdValue");      // 처치 시 행동의 세 번째 값으로 사용
        var bouseAttackValue = property.FindPropertyRelative("thirdValue");     // 다중 타격에서 세 번째 값으로 사용

        Rect actionTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect secondaryValueRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect killEffectTypeRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect killEffectValueRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect bonusAttackValue = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 3, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(actionTypeRect, actionType);

        if (actionType.enumValueIndex != (int)CardActionType.killEffect)
        {
            EditorGUI.PropertyField(valueRect, value);
        }

        if (actionType.enumValueIndex == (int)CardActionType.RandomTargetDamage || 
            actionType.enumValueIndex == (int)CardActionType.StunCheckDamage || 
            actionType.enumValueIndex == (int)CardActionType.PoisonCheckDamage ||
            actionType.enumValueIndex == (int)CardActionType.MultiHit ||
            actionType.enumValueIndex == (int)CardActionType.IncrementalDamage ||
            actionType.enumValueIndex == (int)CardActionType.RandomTargetDamageWithBonus)
        {
            EditorGUI.PropertyField(secondaryValueRect, secondaryValue);
        }

        if (actionType.enumValueIndex == (int)CardActionType.killEffect)
        {
            EditorGUI.PropertyField(killEffectTypeRect, killEffectType);
            EditorGUI.PropertyField(killEffectValueRect, killEffectValue);
        }

        if (actionType.enumValueIndex == (int)CardActionType.RandomTargetDamageWithBonus)
        {
            EditorGUI.PropertyField(bonusAttackValue, bouseAttackValue);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) // 행동 방식 별 inspector창이 표현될 크기
    {
        var actionType = property.FindPropertyRelative("actionType");

        if (actionType.enumValueIndex == (int)CardActionType.RandomTargetDamage || 
            actionType.enumValueIndex == (int)CardActionType.StunCheckDamage || 
            actionType.enumValueIndex == (int)CardActionType.PoisonCheckDamage  ||
            actionType.enumValueIndex == (int)CardActionType.killEffect ||
            actionType.enumValueIndex == (int)CardActionType.MultiHit ||
            actionType.enumValueIndex == (int)CardActionType.IncrementalDamage)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 6;
        }

        if(actionType.enumValueIndex == (int)CardActionType.RandomTargetDamageWithBonus)
        {
            return EditorGUIUtility.singleLineHeight * 4 + 8;
        }

        return EditorGUIUtility.singleLineHeight * 2 + 4;
    }
}
