namespace Content.Client.Animals;

[RegisterComponent]
public sealed partial class WoolyVisualsComponent : Component
{
    /// <summary>
    /// Will be automatically combined with WoolyState and MobState, for example, SpritePrefix is equal to "goat-", then alive and shaved goat will be "goat-short-alive".
    /// If no MobState provided then it will be "goat-short". When no WoolyState provided it will be "goat-alive" (and give a warning) but for this purpose you better use DamageVisualsComponent
    /// </summary>
    [DataField]
    public string SpritePrefix = "goat-";
}
