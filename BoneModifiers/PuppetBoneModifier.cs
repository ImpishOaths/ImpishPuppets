using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public abstract partial class PuppetBoneModifier: Node
{
    protected PuppetTransform Receiver = null;

    public override string[] _GetConfigurationWarnings()
    {
        var parent = GetParent();
        if(parent is PuppetTransform)
            return [];
        return ["PuppetBonePhysics node must be child of PuppetTransform node"];
    }

    public PuppetTransform GetPuppetTransform(NodePath path)
    {
        var trans = GetNodeOrNull(path);
        if(trans == null)
            return null;
        if(trans is PuppetTransform transform)
            return transform;
        return null;
    }

    public abstract void Initialize();
    public override void _Ready()
    {
        var parent = GetParent();
        if(parent is PuppetTransform trans)
        {
            Receiver = trans;
            Initialize();
        }
        else
            GD.PrintErr("PuppetBonePhysics node must be child of PuppetTransform node");
    }

    public abstract void Apply(float delta);
    public override void _PhysicsProcess(double delta)
    {
        if(Receiver == null || !Receiver.HasRoot())
            return;
        
        Apply((float)delta);
    }
}
