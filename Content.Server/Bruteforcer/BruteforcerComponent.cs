// Content.Server/Sound/Bruteforcer.cs
using Robust.Shared.GameStates;

namespace Content.Server.Bruteforcer;

[RegisterComponent]
public sealed partial class BruteforcerComponent : Component
{
    [DataField]
    public string Sound = string.Empty;
}
