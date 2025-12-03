using Content.Shared.Animals;
using Content.Shared.Mobs;
using Robust.Client.GameObjects;

namespace Content.Client.Animals;

public sealed class WoolyStateVisualizerSystem : VisualizerSystem<WoolyVisualsComponent>
{
    [Dependency] private readonly SpriteSystem _sprite = default!;
    protected override void OnAppearanceChange(EntityUid uid, WoolyVisualsComponent component, ref AppearanceChangeEvent args)
    {
        var sprite = args.Sprite;

        if (sprite == null)
            return;

        //        if (!sprite.LayerMapTryGet(WoolyVisualLayers.Base, out var layer))
        //            return;
        if (!_sprite.LayerMapTryGet((uid, sprite), WoolyVisualLayers.Base, out var layer, false))
            return;

        var state = component.SpritePrefix;

        if (AppearanceSystem.TryGetData<WoolyState>(uid, WoolyVisualState.State, out var woolyState, args.Component))
        {
            state += woolyState.ToString().ToLower();
        }

        if (AppearanceSystem.TryGetData<MobState>(uid, MobStateVisuals.State, out var mobState, args.Component))
        {
            state += "-" + mobState.ToString().ToLower();
        }

        //sprite.LayerSetState(layer, state);
        _sprite.LayerSetRsiState((uid, args.Sprite), layer, state);
        //sprite.LayerSetVisible(layer, true);
        _sprite.LayerSetVisible((uid, args.Sprite), layer, true);
    }
}
