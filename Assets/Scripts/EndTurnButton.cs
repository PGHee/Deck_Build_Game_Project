using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    void OnMouseDown()
    {
        TurnManager.instance.EndPlayerTurn();
    }
}
