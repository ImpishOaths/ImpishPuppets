using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKTipModifier: IKModifier
{
    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetTransform2D,PuppetBone3D,PuppetTransform3D")]
    public NodePath TargetPath;
    private PuppetTransform Target = null;

    public override void Apply(float delta)
    {
        Target ??= GetNodeOrNull<PuppetTransform>(TargetPath);
        if(Target == null || ! Target.Active())
            return;

        ForwardPass(Target, Target.GetFlip(), null);
    }

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        var resize = VectorHelpers.PixelResize;
        var duplicate = Duplicate() as IKTipModifier;
        duplicate.Length *= Mathf.Min(resize.X,resize.Y);
        duplicate.TargetOffset *= resize;
        return duplicate;
    }
}
