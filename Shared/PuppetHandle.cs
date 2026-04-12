using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetHandle: Node2D
{
    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetTransform2D,PuppetBone3D,PuppetTransform3D")]
    public NodePath ReceiverPath;
    private PuppetTransform Receiver = null;

    [Export]
    public Vector2 AnimScale = Vector2.One;
    [Export]
    public Vector2 CharacterScale = Vector2.One;

    public override void _Process(double delta)
    {
        if(ReceiverPath != null)
            Receiver ??= GetNodeOrNull<PuppetTransform>(ReceiverPath);

        if(Receiver == null || ! Receiver.Active())
            return;

        var trans = new Transform2D(GlobalRotation, new Vector2(1, Receiver.GetFlip() ? -1 : 1) * AnimScale, 0, GlobalPosition*CharacterScale);
        Receiver.SetRootTransform(trans);
    }
}
