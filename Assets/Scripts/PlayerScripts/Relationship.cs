using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Relationship
{
    public PlayerScript teammate;
    public PlayerScript defender;
    public PlayerScript opponent;

    public PlayerScript player;

    public Relationship(PlayerScript player)
    {
        this.player = player;
        FindTeammate();
        FindOpponents();
    }

    //Assumes only two players per team
    private void FindTeammate()
    {
        //For every player no the team, if not the current player, return as teammate
        foreach (PlayerScript checkPlayer in player.transform.parent.GetComponentsInChildren<PlayerScript>()) {
            if (checkPlayer.Equals(player) == false) {
                teammate = checkPlayer;
                break;
            }
        }
    }

    //Assigns the opponent based on the matching player index in each team (first to first, second to second)
    private void FindOpponents()
    {
        Transform opponentTeam = null;

        //Get the other teams main transform, similiar logic to FindTeammate()
        for(int i = 0; i < player.transform.parent.parent.childCount; i++) {
            Transform testTransform = player.transform.parent.parent.GetChild(i);

            if(testTransform.Equals(player.transform.parent) == false) {
                opponentTeam = testTransform;
                break;
            }
        }

        //Get the index of the player in the team 
        int playerIndex = Array.IndexOf(player.transform.parent.GetComponentsInChildren<PlayerScript>(), player);

        //Defender is the player on the other team with the same index
        defender = opponentTeam.GetComponentsInChildren<PlayerScript>()[playerIndex];

        //Other opposing player is the opponent
        int opponentIndex = playerIndex == 1 ? 0 : 1;
        opponent = opponentTeam.GetComponentsInChildren<PlayerScript>()[opponentIndex];
    }
}
