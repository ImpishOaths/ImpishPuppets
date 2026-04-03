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
        return ["PuppetBoneModifier node must be child of PuppetTransform node"];
    }

    public override void _PhysicsProcess(double delta)
    {
        Receiver ??= GetParent<PuppetTransform>();

        if(Receiver == null || ! Receiver.HasRoot())
            return;
        
        Apply((float)delta);
    }

    public abstract void Apply(float delta);

    public virtual PuppetBoneModifier Make3DDuplicate(Vector2 resize)
    {
        return Duplicate() as PuppetBoneModifier;
    }
}
