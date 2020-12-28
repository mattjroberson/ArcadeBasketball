using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLayerSorter : MonoBehaviour
{

    private List<PlayerScript> players;
    private Dictionary<PlayerScript, SpriteRenderer> playerSprites;

    private GameLogicScript gameLogic;

    private BallScript basketball;
    private SpriteRenderer basketballSprite;

    void Awake()
    {
        players = new List<PlayerScript>();
        playerSprites = new Dictionary<PlayerScript, SpriteRenderer>();
    }

    void Start()
    {
        gameLogic = GetComponent<GameLogicScript>();

        //Get a reference to the basketball
        basketball = gameLogic.GetBasketball();
        basketballSprite = basketball.GetComponent<SpriteRenderer>();

        GameObject playerContainer = GameObject.Find("Players");

        //Get a list of all Players
        foreach (Transform team in playerContainer.GetComponentInChildren<Transform>()) {
            foreach (Transform player in team.GetComponentInChildren<Transform>()) {
                players.Add(player.GetComponent<PlayerScript>());
            }
        }

        //Create a dictionary to hold the players Sprite Renderers
        for(int i = 0; i < players.Count; i++) {
            playerSprites.Add(players[i], players[i].GetComponent<SpriteRenderer>());
        }
    }

    void FixedUpdate()
    {
        //Sort the players
        players.Sort(SortByY);

        int i = 0;

        bool ballSorted = false;

        //Set the Layer for each Player
        foreach (PlayerScript player in players) {
            //If the ball isnt possessed and hasnt been sorted account for its sorting order
            if (BallScript.GetBallState() != BallScript.BallState.POSESSED && ballSorted == false) {

                //If ball is being passed
                if (BallScript.GetBallState() == BallScript.BallState.PASSING) {
                    //If path is behind a player, order it behind
                    if (IsPassPathBehindPlayer(player) == true) {
                        basketballSprite.sortingOrder = i;

                        ballSorted = true;
                        i++;
                    }
                }
                //If ball is shooting or falling, use the balls ground position as it order
                else {
                    float ground = basketball.GetFloor().y;
                    ground -= (basketballSprite.size.y / 2);

                    //If the balls last ground position is behind (>) player, put it here
                    if (ground >= player.GetFrontPoint().position.y) {
                        basketballSprite.sortingOrder = i;
                        ballSorted = true;
                        i++;
                    }
                }
            }

            //Set the players render order
            playerSprites[player].sortingOrder = i;
            i++;

            //If the player has the ball set its order
            //if (player.GetHasBall() == true) {
            //    basketballSprite.sortingOrder = i;
            //    i++;
            //}
        }
    }

    //Sort by the Y Position
    static int SortByY(PlayerScript p1, PlayerScript p2)
    {
        float p1_y = p1.GetFrontPoint().position.y;
        float p2_y = p2.GetFrontPoint().position.y;

        return p2_y.CompareTo(p1_y);
    }

    //Checks the line between players involved in a pass, if tested player is in front of that line, return true
    private bool IsPassPathBehindPlayer(PlayerScript player)
    {
        //Get references to the players involved in the pass
        PlayerScript currentPlayer = basketball.GetCurrentPlayer();
        PlayerScript targetPlayer = basketball.GetTargetPlayer();

        //Do nothing if the evaluated player is part of the pass
        if (player == currentPlayer) return false;
        if (player == targetPlayer) return false;

        //Get references to player positions
        Vector2 p1 = currentPlayer.transform.position;
        Vector2 p2 = targetPlayer.transform.position;
        Vector2 p3 = player.transform.position;

        //Do nothing if the player would not be obstructed by the pass
        if (PassWouldObstruct(p1, p2, p3) == true) return false;

        Debug.Log(p1 + " : " + p2 + " : " + p3);

        //Line Algorithm
        float slope = (p1.y - p2.y) / (p1.x - p2.x);
        float y_intercept = p1.y - (slope * p1.x);

        float line_y = (slope * p3.x) + y_intercept;
        float diff_y = line_y - p3.y;

        if (diff_y >= 0) {
            Debug.Log("aye aye");
            return true;
        }
        else return false;
    }

    private bool PassWouldObstruct(Vector2 passer, Vector2 target, Vector2 player)
    {
        if(passer.x < target.x) {
            if (player.x > passer.x || player.x < target.x) return true;
        }
        else{
            if (player.x < passer.x || player.x > target.x) return true; 
        }

        return false;
    }
}
