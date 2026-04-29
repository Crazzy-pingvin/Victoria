using Content.Client.Light.Components;
using Content.Client.Fuel;
using Content.Shared.Fuel;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Shared.Audio.Systems;

namespace Content.Client.Light.EntitySystems;

public sealed class FuelSystem : VisualizerSystem<FuelConsumerComponent>
{
    [Dependency] private readonly SharedPointLightSystem _pointLightSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly LightBehaviorSystem _lightBehavior = default!;
    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<FuelConsumerComponent, ComponentShutdown>(OnLightShutdown);
    }

/*     private void OnLightShutdown(EntityUid uid, FuelConsumerComponent component, ComponentShutdown args)
    {
        component.PlayingStream = _audioSystem.Stop(component.PlayingStream);
    }*/

    protected override void OnAppearanceChange(EntityUid uid, FuelConsumerComponent comp, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

/*         if (AppearanceSystem.TryGetData<string>(uid, ExpendableLightVisuals.Behavior, out var lightBehaviourID, args.Component)
            && TryComp<LightBehaviourComponent>(uid, out var lightBehaviour))
        {
            _lightBehavior.StopLightBehaviour((uid, lightBehaviour));

            if (!string.IsNullOrEmpty(lightBehaviourID))
            {
                _lightBehavior.StartLightBehaviour((uid, lightBehaviour), lightBehaviourID);
            }
            else if (TryComp<PointLightComponent>(uid, out var light))
            {
                _pointLightSystem.SetEnabled(uid, false, light);
            }
        } */

        if (!AppearanceSystem.TryGetData<ConsumerStates>(uid, FuelVisuals.State, out var state, args.Component))
            return;

        switch (state)
        {
            case ConsumerStates.Empty:
                if (SpriteSystem.LayerMapTryGet((uid, args.Sprite), ConsumerVisualLayers.Fire, out var layerIdx, true))
                    SpriteSystem.LayerSetVisible((uid,args.Sprite), layerIdx, false);
                break;
            case ConsumerStates.Burns:
                if (SpriteSystem.LayerMapTryGet((uid, args.Sprite), ConsumerVisualLayers.Fire, out var layerIdx1, true))
                    SpriteSystem.LayerSetVisible((uid,args.Sprite), layerIdx1, true);
                break;
        }
    }
}
