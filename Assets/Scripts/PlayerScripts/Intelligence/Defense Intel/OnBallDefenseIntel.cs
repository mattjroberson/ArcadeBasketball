public class OnBallDefenseIntel : DefenseIntelType
{
    public OnBallDefenseIntel(PlayerScript player, ActionsScript actions) : base(player, actions) { }

    public override void Wake() { base.Wake(); }

    public override void Sleep() { base.Sleep(); }

    public override void UpdateIntelligence() { base.UpdateIntelligence(); }
}
