using UnityEngine;

public class DefenseIntelType : ArtificialIntelType
{
    private readonly TargetScript opponent;

    public DefenseIntelType(PlayerScript player, ActionsScript actions) : base(player, actions)
    {
        opponent = GameObject.Find(player.Defender.name+"Target").GetComponent<TrackingTargetScript>().Target;
    }

    public override void UpdateIntelligence()
    {
        base.UpdateIntelligence();
        if (trackingLooseBall) return;

        if(Vector2.Distance(opponent.PlayerPos, player.transform.position) > .1f) {
            MoveToSpot(opponent.FeetPos);
        }
    }

    public override void Wake()
    {
        base.Wake();
    }

    public override void Sleep()
    {
        base.Sleep();
    }
}
