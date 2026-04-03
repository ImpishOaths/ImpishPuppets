using Godot;
using System;

namespace ImpishPuppets;

[Tool]
public partial class PuppetHandle: Node2D
{
    [Export(PropertyHint.NodePathValidTypes, "Puppet2DBone,Puppet2DControl,Puppet3DBone,Puppet3DControl")]
    public NodePath ReceiverPath
    {
        get => _ReceiverPath;
        set
        {
            _ReceiverPath = value;
            Initialize();
        }
    }
    private NodePath _ReceiverPath;
    private PuppetTransform Receiver;

    [Export]
    private Vector2 AnimScale = Vector2.One;

    [Export]
    public bool FlipH
    {
        get => Receiver != null && Receiver.GetFlipH();
        set => Receiver?.SetFlipH(value);
    }
    [Export]
    public bool FlipV
    {
        get => Receiver != null && Receiver.GetFlipV();
        set => Receiver?.SetFlipV(value);
    }

    public void Initialize()
    {
        if(_ReceiverPath == null)
        {
            Receiver = null;
            return;
        }
        var receiver = GetNodeOrNull(_ReceiverPath);
        if(receiver != null && receiver is PuppetTransform bone)
            Receiver = bone;
        else
            Receiver = null;
    }

    public override void _Ready()
    {
        Initialize();
    }

    public override void _Process(double delta)
    {
        if(Receiver == null || !Receiver.HasRoot())
            return;
        Receiver.SetRootRotation(GlobalRotation);
        Receiver.SetRootPosition(GlobalPosition);
        Receiver.SetLocalScale(AnimScale);
    }
}
