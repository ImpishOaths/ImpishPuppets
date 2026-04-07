using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PointAtModifier: PuppetBoneModifier
{
    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetTransform2D,PuppetBone3D,PuppetTransform3D")]
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
