using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Energized;
using Content.Shared.StatusEffect;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Energized;

public sealed class EnergizedOverlay : Overlay
{
    [Dependency] protected readonly ISharedAdminLogManager AdminLogger = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntitySystemManager _sysMan = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowFOV;
    public override bool RequestScreenTexture => true;
    private readonly ShaderInstance _EnergizedShader;

    public float CurrentEnergizePower = 0.0f;

    private const float VisualThreshold = 4.0f;
    private const float PowerDivisor = 120.0f;

    private float _visualScale = 0;

    public EnergizedOverlay()
    {
        IoCManager.InjectDependencies(this);
        _EnergizedShader = _prototypeManager.Index<ShaderPrototype>("Energized").InstanceUnique();
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {

        var playerEntity = _playerManager.LocalEntity;

        if (playerEntity == null)
            return;

        if (!_entityManager.HasComponent<EnergizedComponent>(playerEntity)
            || !_entityManager.TryGetComponent<StatusEffectsComponent>(playerEntity, out var status))
            return;

        var statusSys = _sysMan.GetEntitySystem<StatusEffectsSystem>();
        if (!statusSys.TryGetTime(playerEntity.Value, SharedEnergizedSystem.EnergyKey, out var time, status))
            return;

        var curTime = _timing.CurTime;
        var timeLeft = (float) (time.Value.Item2 - curTime).TotalSeconds;


        CurrentEnergizePower += 5.0f * args.DeltaSeconds;
        AdminLogger.Add(LogType.Action, LogImpact.Low, $" {CurrentEnergizePower.ToString()} ");
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_entityManager.TryGetComponent(_playerManager.LocalEntity, out EyeComponent? eyeComp))
            return false;

        if (args.Viewport.Eye != eyeComp.Eye)
            return false;

        _visualScale = EnergizePowerToVisual(CurrentEnergizePower);
        return _visualScale > 0;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        var handle = args.WorldHandle;
        _EnergizedShader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _EnergizedShader.SetParameter("tranq_power", _visualScale);
        handle.UseShader(_EnergizedShader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }

    /// <summary>
    ///     Converts the # of seconds the drunk effect lasts for (tranq power) to a percentage
    ///     used by the actual shader.
    /// </summary>
    /// <param name="energizePower"></param>
    private float EnergizePowerToVisual(float energizePower)
    {
        // Clamp tranq power when it's low, to prevent really jittery effects
        if (energizePower < 2f)
        {
            return 0;
        }
        else
        {
            return Math.Clamp((energizePower - VisualThreshold) / PowerDivisor, 0.0f, 1.0f);
        }
    }
}
