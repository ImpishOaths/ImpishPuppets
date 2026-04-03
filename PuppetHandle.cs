using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetHandle: Node2D
{
    [Export(PropertyHint.NodePathValidTypes, "Puppet2DBone,Puppet2DControl,Puppet3DBone,Puppet3DControl")]
    public NodePath ReceiverPath
    {
        get => _ReceiverPath;
        set
        {
            _ReceiverPath = value;
            Receiver = null;
        }
    }
    private NodePath _ReceiverPath = null;
    private PuppetTransform Receiver = null;

    [Export]
    private Vector2 AnimScale = Vector2.One;

    public override void _Process(double delta)
    {
        Receiver ??= GetNodeOrNull<PuppetTransform>(ReceiverPath);

        if(Receiver == null || ! Receiver.HasRoot())
            return;
            
        var trans = new Transform2D(GlobalRotation, AnimScale, 0, GlobalPosition);
        Receiver.SetRootTransform(trans);
    }
}
