using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class RemotePuppetTransform: Node2D
{
    private static readonly float GlobalDelay = 0.4f;

    [Export]
    public string ReceiverPath;

    public void SetReceiver(Node puppet)
    {
        if(puppet != null)
            Receiver = puppet.GetNode<PuppetTransform>(ReceiverPath);
    }
    private PuppetTransform Receiver = null;

    [Export]
    public Vector2 AnimScale = Vector2.One;

    public override void _Process(double delta)
    {
        if(Receiver == null || ! Receiver.Active())
            return;

        var oldTrans = Receiver.GetRootTransform();

        var trans = new Transform2D(
            Mathf.LerpAngle(oldTrans.Rotation, GlobalRotation, GlobalDelay),
            new Vector2(1, Receiver.GetFlip() ? -1 : 1) * AnimScale, 0,
            oldTrans.Origin.Lerp(GlobalPosition, GlobalDelay)
        );
        
        Receiver.SetRootTransform(trans);
    }
}
