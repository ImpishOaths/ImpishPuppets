using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public abstract partial class PuppetBoneModifier: Node
{
    public const float To3DScale = 0.05f;

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

    public abstract PuppetBoneModifier Make3DDuplicate();
}
