using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardNameConverter : MonoBehaviour
{

    public static string CardNumToCode(int cardNum)
    {
        switch((int)(cardNum / 100))
        {
            case 0:
                return ("Fi_" + (cardNum % 100  + 1)+ "");
                
            case 1:
                return ("Wa_" + (cardNum % 100 + 1)+ "");
                
            case 2:
                return ("Tr_" + (cardNum % 100 + 1)+ "");
                
            case 3:
                return ("Gr_" + (cardNum % 100 + 1) + "");
                
            case 4:
                return ("Th_" + (cardNum % 100 + 1) + "");
                
            case 5:
                return ("Wi_" + (cardNum % 100 + 1) + "");
                
            case 6:
                return ("Li_" + (cardNum % 100 + 1) + "");
                
            case 7:
                return ("Da_" + (cardNum % 100 + 1) + "");
                
            //case 8: 노말 카드 제작 이후 해금
                //return ("No_" + (cardNum % 100 + 1) + "");
                
            default:
                Debug.Log("un identified card number");
                return ("Fi_" + (cardNum % 100 + 1) + "");      
        }
    }
}
