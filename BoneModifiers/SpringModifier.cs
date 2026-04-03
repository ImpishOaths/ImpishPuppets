using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class SpringModifier: ImpulseModifier
{
    [Export]
    public Vector2 Offset;

    public override void ApplyImpulse(float value)
    {
        var trans = Receiver.GetLocalTransform();
        trans.Origin = ImpulseDir.Normalized() * value + Offset;
        Receiver.SetLocalTransform(trans);
    }

    public override PuppetBoneModifier Make3DDuplicate()
    {
        var duplicate = Duplicate() as SpringModifier;
        duplicate.Offset *= To3DScale;
        duplicate.MinMax *= To3DScale;
        return duplicate;
    }
}
