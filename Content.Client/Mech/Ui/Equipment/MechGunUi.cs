using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Robust.Client.UserInterface;

namespace Content.Client.Mech.Ui.Equipment;

public sealed partial class MechGunUi : UIFragment
{
    private MechGunUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        if (fragmentOwner == null)
            return;

        _fragment = new MechGunUiFragment();
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not MechGunUiState gunState)
            return;

        _fragment?.UpdateContents(gunState);
    }
}
