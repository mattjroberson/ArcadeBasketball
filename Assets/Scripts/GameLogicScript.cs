using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private PlayerScript[] userPlayers;
    private PlayerScript[] opponentPlayers;

    private Dictionary<PlayerScript, Relationship> userPlayerRelationships;
    private Dictionary<PlayerScript, Relationship> opponentPlayerRelationships;

    private BallScript basketball;

    private float targetPlaybackSpeed;
    private float slomoLerpValue;
    private float slomoLerpSpeed;
    private bool lerpingPlayback;

    public void Awake()
    {
        //Set up some display variables
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;

        lerpingPlayback = false;
        slomoLerpSpeed = .3f;

        basketball = GameObject.Find("Basketball").GetComponent<BallScript>();
    }

    public void Start()
    {

        userPlayers = GetPlayers("userTeam");
        userPlayerRelationships = GetPlayerRelationships(userPlayers);

        opponentPlayers = GetPlayers("opponentTeam");
        opponentPlayerRelationships = GetPlayerRelationships(opponentPlayers);
    }

    public void Update()
    { 
        if (lerpingPlayback) {
            float newTime = Mathf.Lerp(Time.timeScale, targetPlaybackSpeed, slomoLerpValue);
            slomoLerpValue += slomoLerpSpeed * Time.unscaledDeltaTime;
            SetPlaybackSpeed(newTime);

            //Stop lerping when completely lerped
            if (newTime == targetPlaybackSpeed) {
                Debug.Log("on");
                lerpingPlayback = false;
            }
        }
    }

    public void UpdatePossession(PlayerScript ballHandler)
    {
        if (IsUserPlayer(ballHandler)) {
            Relationship relationship = userPlayerRelationships[ballHandler];

            ballHandler.HandlePossession(true, IntelligenceContainer.IntelligenceType.USER);
            relationship.teammate.HandlePossession(false, IntelligenceContainer.IntelligenceType.OFFBALL_OFF);
            relationship.defender.HandlePossession(false, IntelligenceContainer.IntelligenceType.ONBALL_DEF);
            relationship.opponent.HandlePossession(false, IntelligenceContainer.IntelligenceType.OFFBALL_DEF);
        }
        else {
            Relationship relationship = opponentPlayerRelationships[ballHandler];

            ballHandler.HandlePossession(true, IntelligenceContainer.IntelligenceType.ONBALL_OFF);
            relationship.teammate.HandlePossession(false, IntelligenceContainer.IntelligenceType.OFFBALL_OFF);
            relationship.defender.HandlePossession(false, IntelligenceContainer.IntelligenceType.ONBALL_DEF);
            relationship.opponent.HandlePossession(false, IntelligenceContainer.IntelligenceType.OFFBALL_DEF);
        }
    }

    public void SetPlaybackSpeed(float speed, bool lerp)
    {
        if (lerp == true) {
            slomoLerpValue = 0f;
            targetPlaybackSpeed = speed;
        }
        else {
            SetPlaybackSpeed(speed);
        }

        lerpingPlayback = lerp;
    }

    private void SetPlaybackSpeed(float speed) {
        Time.timeScale = speed;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    private PlayerScript[] GetPlayers(string team)
    {
        return GameObject.Find(team).GetComponentsInChildren<PlayerScript>();
    }

    //Build the relationships of the players
    private Dictionary<PlayerScript, Relationship> GetPlayerRelationships(PlayerScript[] playerList)
    {
        Dictionary<PlayerScript, Relationship> relationships = new Dictionary<PlayerScript, Relationship>();

        foreach(PlayerScript player in playerList){
            relationships.Add(player, new Relationship(player));
        }

        return relationships;
    }

    //Returns true if the passed player is a user controlled player
    private bool IsUserPlayer(PlayerScript testPlayer)
    {
        foreach (PlayerScript player in userPlayers) {
            if (player.Equals(testPlayer)) return true;
        }

        return false;
    }

    public BallScript GetBasketball() { return basketball; }
}
