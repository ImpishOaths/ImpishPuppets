using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKTipModifier: IKModifier
{
    [Export(PropertyHint.NodePathValidTypes, "Puppet2DBone,Puppet2DControl,Puppet3DBone,Puppet3DControl")]
    private NodePath TargetPath;
    public PuppetTransform Target;

    public override void Initialize()
    {
        base.Initialize();
        Target = GetPuppetTransform(TargetPath);
    }
    
    private int GetNudgeSign()
    {
        Vector2 targetPos = Target.GetRootScale();
        return Mathf.Sign(targetPos.X*targetPos.Y);
    }

    public override void Apply(float delta)
    {
        if(Target == null || !Target.HasRoot())
            return;

        Vector2 rootPos = GetChainRootPos();

        ForwardPass(Target, GetNudgeSign());
        BackwardPass(rootPos);
    }
}
