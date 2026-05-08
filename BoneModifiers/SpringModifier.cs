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

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        var resize = VectorHelpers.PixelResize;
        var duplicate = Duplicate() as SpringModifier;
        duplicate.Offset *= resize;
        duplicate.MinMax *= resize;
        return duplicate;
    }
}
