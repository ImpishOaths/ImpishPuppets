using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class MatchModifier: PuppetBoneModifier
{
    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetTransform2D,PuppetBone3D,PuppetTransform3D")]
    public NodePath TargetPath;
    private PuppetTransform Target = null;
    
    [Export]
    public Vector2 Offset;
    [Export]
    public float RotationOffset;

    public override void Apply(float delta)
    {
        Target ??= GetNodeOrNull<PuppetTransform>(TargetPath);
        if(Target == null || ! Target.Active())
            return;
        
        var trans = Receiver.GetRootTransform();
        var targetTrans = Target.GetRootTransform();

        trans = new(targetTrans.Rotation + RotationOffset, trans.Scale, trans.Skew, targetTrans.Origin + Offset);
        Receiver.SetRootTransform(trans);
    }

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        var resize = VectorHelpers.PixelResize;
        var duplicate = Duplicate() as MatchModifier;
        duplicate.Offset *= resize;
        return duplicate;
    }
}
