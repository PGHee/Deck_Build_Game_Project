using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnActionUI : MonoBehaviour
{
    public SpriteRenderer monsterActionUI;
    public TextMeshProUGUI monsterActionText;
    public Sprite[] monsterActionSprite;

    public void Initialize(Transform target, ActionType monsterAction)
    {
        transform.SetParent(target);
        transform.localPosition = new Vector3(0, 3.0f, 0);
    }
    
    public void UpdateAction(ActionType monsterAction, int intEffect, float floatEffect)
    {
        switch (monsterAction)
        {
            case ActionType.Damage:
            case ActionType.StrongAttack:
            case ActionType.LifeStealAttack:
            case ActionType.Shield:
            case ActionType.Heal:
            case ActionType.AreaHeal:
            case ActionType.Poison:
            case ActionType.SelfDestruct:
            case ActionType.Wait:
                if(intEffect != 0) monsterActionText.text = intEffect + "";
                else monsterActionText.text = "";
                monsterActionUI.sprite = monsterActionSprite[(int)monsterAction];
                break;
            case ActionType.IncreaseDamage:
            case ActionType.IncreaseDamageStack:
            case ActionType.LifeSteal:
            case ActionType.ReduceDamage:
            case ActionType.ReflectDamage:
            case ActionType.SkipTurn:
            case ActionType.Confuse:
                if(floatEffect != 0) monsterActionText.text = floatEffect + "%";
                else monsterActionText.text = "";
                monsterActionUI.sprite = monsterActionSprite[(int)monsterAction];
                break;
        }
    }
}
