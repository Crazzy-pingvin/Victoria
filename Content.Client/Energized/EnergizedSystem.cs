using Content.Shared.Energized;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Energized;

public sealed class EnergizedSystem : SharedEnergizedSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;

    private EnergizedOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EnergizedComponent, ComponentInit>(OnEnergizedInit);
        SubscribeLocalEvent<EnergizedComponent, ComponentShutdown>(OnEnergizedShutdown);

        SubscribeLocalEvent<EnergizedComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<EnergizedComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new();
    }

    private void OnPlayerAttached(EntityUid uid, EnergizedComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnPlayerDetached(EntityUid uid, EnergizedComponent component, LocalPlayerDetachedEvent args)
    {
        _overlay.CurrentEnergizePower = 0;
        _overlayMan.RemoveOverlay(_overlay);
    }

    private void OnEnergizedInit(EntityUid uid, EnergizedComponent component, ComponentInit args)
    {
        if (_player.LocalEntity == uid)
            _overlayMan.AddOverlay(_overlay);
    }

    private void OnEnergizedShutdown(EntityUid uid, EnergizedComponent component, ComponentShutdown args)
    {
        if (_player.LocalEntity == uid)
        {
            _overlay.CurrentEnergizePower = 0;
            _overlayMan.RemoveOverlay(_overlay);
        }
    }
}
