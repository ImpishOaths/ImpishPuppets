using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PointDirModifier: PuppetBoneModifier
{
    [Export]
    public Vector2 dir = new(1, 0);
    
    public override void Apply(float delta)
    {
        var trans = Receiver.GetRootTransform();
        trans = new(dir.Angle(), trans.Scale, trans.Skew, trans.Origin);
        Receiver.SetRootTransform(trans);
    }

    public override PuppetBoneModifier MakeDuplicate3D(Vector2 resize)
    {
        return Duplicate() as PointDirModifier;
    }
}
