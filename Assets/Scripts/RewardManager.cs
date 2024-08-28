using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public PlayerState playerState;
    public DeckManager deckManager;
    public ArtifactManager artifactManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetReward(string message, int num, int price)
    {
        if (playerState.crystal >= price)
        {
            switch (message)
            {
                case "Card":
                    deckManager.AddRewardCard(num);
                    playerState.SpendCrystal(price);
                    break;

                case "Artifact":
                    artifactManager.AddArtifact2Inven(num);
                    playerState.SpendCrystal(price);
                    break;

                case "HP":
                    playerState.Heal(num);
                    playerState.SpendCrystal(price);
                    break;

                case "Crystal":
                    playerState.GainCrystal(num);
                    playerState.SpendCrystal(price);
                    break;

                default:
                    Debug.Log("Wrong Reward Message");
                    break;
            }
        }       
    }
}
