using Content.Shared.Tranquil;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Tranquil;

public sealed class TranquilSystem : SharedTranquilSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;

    private TranquilOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TranquilComponent, ComponentInit>(OnTranquilInit);
        SubscribeLocalEvent<TranquilComponent, ComponentShutdown>(OnTranquilShutdown);

        SubscribeLocalEvent<TranquilComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<TranquilComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new();
    }

    private void OnPlayerAttached(EntityUid uid, TranquilComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnPlayerDetached(EntityUid uid, TranquilComponent component, LocalPlayerDetachedEvent args)
    {
        _overlay.CurrentTranqPower = 0;
        _overlayMan.RemoveOverlay(_overlay);
    }

    private void OnTranquilInit(EntityUid uid, TranquilComponent component, ComponentInit args)
    {
        if (_player.LocalEntity == uid)
            _overlayMan.AddOverlay(_overlay);
    }

    private void OnTranquilShutdown(EntityUid uid, TranquilComponent component, ComponentShutdown args)
    {
        if (_player.LocalEntity == uid)
        {
            _overlay.CurrentTranqPower = 0;
            _overlayMan.RemoveOverlay(_overlay);
        }
    }
}
