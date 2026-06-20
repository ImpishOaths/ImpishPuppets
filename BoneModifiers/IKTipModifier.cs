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

    const float DistEpislon = 0f;

    private Vector2? PrevTargetPos = null;

    public override void Apply(float delta)
    {
        Target ??= GetNodeOrNull<PuppetTransform>(TargetPath);
        if(Target == null || ! Target.Active())
            return;

        Vector2 targetPos = Target.GetRootTransform().Origin;
        if(PrevTargetPos == null || PrevTargetPos.Value.DistanceTo(targetPos) > DistEpislon)
        {
            ForwardPass(Target, Target.GetFlip(), null);
            PrevTargetPos = targetPos;
        }
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
