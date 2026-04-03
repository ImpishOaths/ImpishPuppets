using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PendulumModifier: ImpulseModifier
{
    public override void ApplyImpulse(float value)
    {
        var trans = Receiver.GetLocalTransform();
        trans = new Transform2D(value, trans.Scale, trans.Skew, trans.Origin);
        Receiver.SetLocalTransform(trans);
    }

    public override PuppetBoneModifier Make3DDuplicate()
    {
        var duplicate = Duplicate() as PendulumModifier;
        duplicate.Sensitivity /= To3DScale;
        return duplicate;
    }
}