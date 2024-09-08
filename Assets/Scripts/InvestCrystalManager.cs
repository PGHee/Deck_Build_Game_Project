using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestCrystalManager : MonoBehaviour
{
    public List<int> investTurn;
    public List<int> investGain;

    private PlayerState playerState;

    // Start is called before the first frame update
    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
    }

    public void AddInvest(int turn, int gain)
    {
        investTurn.Add( turn );
        investGain.Add( gain );
    }

    public void CheckInvest() // ���� ���� �� ����
    {
        for (int i = 0; i < investTurn.Count; i++)
        {
            if( investTurn[i]  <= 1)
            {
                playerState.GainCrystal(investGain[i]); // ���� ���� ȹ��

                investGain.RemoveAt(i);
                investTurn.RemoveAt(i);
            }
            else
            {
                investTurn[i]--;
            }
        }
    }

    public void ClearInvest() // �� ���� ����� ����
    {
        investTurn.Clear();
        investGain.Clear();
    }

    public void GetInvestGainAll()
    {
        for (int i = 0; i < investGain.Count; i++)
        {
            playerState.GainCrystal(investGain[i]);
        }
        CheckInvest();
    }
}
