using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PendulumModifier: ImpulseModifier
{
    [Export]
    public bool Absolute = false;
    [Export]
    public Vector2 AbsoluteDir = new(0,1);
    [Export]
    public float AngleOffset = 0;

    public override void ApplyImpulse(float value)
    {
        if(Absolute)
        {
            var rootTrans = Receiver.GetRootTransform();
            rootTrans = new(AbsoluteDir.Angle()+value+AngleOffset, rootTrans.Scale, rootTrans.Skew, rootTrans.Origin);
            Receiver.SetRootTransform(rootTrans);
        }
        else
        {
            var trans = Receiver.GetLocalTransform();
            trans = new Transform2D(value+AngleOffset, trans.Scale, trans.Skew, trans.Origin);
            Receiver.SetLocalTransform(trans);
        }

    }

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        var resize = VectorHelpers.PixelResize;
        var duplicate = Duplicate() as PendulumModifier;
        duplicate.Sensitivity /= Mathf.Min(resize.X, resize.Y);
        return duplicate;
    }
}