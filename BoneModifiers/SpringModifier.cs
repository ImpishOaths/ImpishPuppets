using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class SpringModifier: ImpulseModifier
{
    public override void ApplyImpulse(float value)
    {
        Receiver.SetLocalPosition(ImpulseDir.Normalized() * value);
    }
}
