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

    public override void Initialize()
    {
        Target = GetPuppetTransform(TargetPath);
    }

    public override void Apply(float delta)
    {
        if(Target == null)
            return;
        
        var receiverTrans = Receiver.GetRootPosition();
        var targetTrans = Target.GetRootPosition();

        var diff = targetTrans - receiverTrans;

        Receiver.SetRootRotation(diff.Angle() + Offset);
    }

}
