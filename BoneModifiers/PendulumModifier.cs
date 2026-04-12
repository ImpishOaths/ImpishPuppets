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

    public override void ApplyImpulse(float value)
    {
        if(Absolute)
        {
            var rootTrans = Receiver.GetRootTransform();
            rootTrans = new(AbsoluteDir.Angle()+value, rootTrans.Scale, rootTrans.Skew, rootTrans.Origin);
            Receiver.SetRootTransform(rootTrans);
        }
        else
        {
            var trans = Receiver.GetLocalTransform();
            trans = new Transform2D(value, trans.Scale, trans.Skew, trans.Origin);
            Receiver.SetLocalTransform(trans);
        }

    }

    public override PuppetBoneModifier MakeDuplicate3D(Vector2 resize)
    {
        var duplicate = Duplicate() as PendulumModifier;
        duplicate.Sensitivity /= Mathf.Min(resize.X, resize.Y);
        return duplicate;
    }
}