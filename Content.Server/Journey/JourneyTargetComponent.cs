namespace Content.Server.Journey
{
    [RegisterComponent]
    public sealed partial class JourneyTargetComponent : Component
    {
        [DataField("priority")]
        public int Priority = 1;

        [DataField("ignoreGroups")]
        public bool IgnoreGroups = false;

        [DataField("journeyGroup")]
        public string JourneyGroup = "Alpha";

        [DataField("maxRange")]
        public float MaxRange = 10;
    }
}
