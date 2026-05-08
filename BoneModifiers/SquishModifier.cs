using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class SquishModifier: ImpulseModifier
{
    [Export]
    public Vector2 SquishAmount = Vector2.Zero;

    public override void ApplyImpulse(float value)
    {
        var trans = Receiver.GetLocalTransform();
        trans = new(trans.Rotation, Vector2.One + SquishAmount*value, trans.Skew, trans.Origin);
        Receiver.SetLocalTransform(trans);
    }

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        var resize = VectorHelpers.PixelResize;
        var duplicate = Duplicate() as SquishModifier;
        duplicate.Sensitivity /= Mathf.Min(resize.X, resize.Y);
        return duplicate;
    }
}
