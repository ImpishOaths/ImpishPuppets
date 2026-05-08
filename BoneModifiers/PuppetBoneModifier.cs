using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public abstract partial class PuppetBoneModifier: Node, Puppet2Dto3DConverter
{
    protected PuppetTransform Receiver = null;

    public override string[] _GetConfigurationWarnings()
    {
        var parent = GetParent();
        if(parent is PuppetTransform)
            return [];
        return ["PuppetBoneModifier node must be child of PuppetTransform node"];
    }

    public override void _EnterTree()
    {
        Receiver ??= GetParent<PuppetTransform>();
    }

    public override void _PhysicsProcess(double delta)
    {
        Receiver ??= GetParent<PuppetTransform>();
        if(Receiver == null || ! Receiver.Active())
            return;
        
        Apply((float)delta);
    }

    public abstract void Apply(float delta);

    public virtual Node ConvertTo3D(Puppet3D puppet)
    {
        return Duplicate();
    }
}
