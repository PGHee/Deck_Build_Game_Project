using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public PlayerState playerState;
    public DeckManager deckManager;
    public ArtifactManager artifactManager;
    private SystemMessage message;

    // Start is called before the first frame update
    void Start()
    {
        message = FindObjectOfType<SystemMessage>();
    }

    public void GetReward(string messageS, int num, int price)
    {
        if (playerState.crystal >= price)
        {
            switch (messageS)
            {
                case "Card":
                    deckManager.AddRewardCard(num);
                    playerState.SpendCrystal(price);
                    message.ShowSystemMessage("Ä«µå È¹µæ");
                    break;

                case "Artifact":
                    artifactManager.AddArtifact2Inven(num);
                    playerState.SpendCrystal(price);
                    message.ShowSystemMessage("¾ÆÆ¼ÆÑÆ® È¹µæ");
                    break;

                case "HP":
                    playerState.Heal(num);
                    playerState.SpendCrystal(price);
                    message.ShowSystemMessage("Ã¼·Â È¸º¹");
                    break;

                case "Crystal":
                    playerState.GainCrystal(num);
                    playerState.SpendCrystal(price);
                    message.ShowSystemMessage("Å©¸®½ºÅ» È¹µæ");
                    break;

                default:
                    Debug.Log("Wrong Reward Message");
                    break;
            }
        }
        else
        {
            message.ShowSystemMessage("Å©¸®½ºÅ»ÀÌ ºÎÁ·ÇØ");
        }
    }
}
