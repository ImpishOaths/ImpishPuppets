using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PointDirModifier: PuppetBoneModifier
{
    [Export]
    public Vector2 dir = new(1, 0);
    
    public override void Apply(float delta)
    {
        Receiver.SetRootRotation(dir.Angle());
    }

    public override void Initialize() {}

}
