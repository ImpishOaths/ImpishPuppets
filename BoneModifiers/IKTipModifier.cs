using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKTipModifier: IKModifier
{
    [Export(PropertyHint.NodePathValidTypes, "Puppet2DBone,Puppet2DControl,Puppet3DBone,Puppet3DControl")]
    public NodePath TargetPath
    {
        get => _TargetPath;
        set
        {
            _TargetPath = value;
            Target = null;
        }
    }
    private NodePath _TargetPath = null;
    private PuppetTransform Target = null;
    [Export]
    public bool debug;

    private int GetNudgeSign()
    {
        Vector2 targetScale = Target.GetRootTransform().Scale;
        return Mathf.Sign(targetScale.X*targetScale.Y);
    }

    public override void Apply(float delta)
    {
        Target ??= GetNodeOrNull<PuppetTransform>(TargetPath);
        if(Target == null || ! Target.HasRoot())
            return;

        ForwardPass(Target, GetNudgeSign());
    }
}
