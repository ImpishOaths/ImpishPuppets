using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PendulumModifier: ImpulseModifier
{
    public override void ApplyImpulse(float value)
    {
        Receiver.SetLocalRotation(value);
    }
}