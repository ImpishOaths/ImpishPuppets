using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PointAtModifier: PuppetBoneModifier
{
    [Export(PropertyHint.NodePathValidTypes, "Puppet2DBone,Puppet2DControl,Puppet3DBone,Puppet3DControl")]
    public NodePath TargetPath;
    private PuppetTransform Target = null;

    [Export]
    public float Offset;

    public override void Apply(float delta)
    {
        Target ??= GetNodeOrNull<PuppetTransform>(TargetPath);
        if(Target == null || ! Target.Active())
            return;
        
        var trans = Receiver.GetRootTransform();
        var targetTrans = Target.GetRootTransform();

        var diff = targetTrans.Origin - trans.Origin;
        trans = new(diff.Angle() + Offset, trans.Scale, trans.Skew, trans.Origin);
        Receiver.SetRootTransform(trans);
    }
}
