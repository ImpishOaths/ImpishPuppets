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

    [Export]
    public StringName WorldTargetID
    {
        get => _WorldTargetID;
        set
        {
            _WorldTargetID = value;
            _WorldTarget = WorldTarget.GetTarget(_WorldTargetID);
        }
    }
    private StringName _WorldTargetID;
    [Export]
    public bool WorldTargetFound
    {
        get => _WorldTarget != null;
        set {}
    }
    private WorldTarget _WorldTarget;

    [Export]
    public float WorldTargetPercent = 0;

    public void SetReceiver(Node puppet)
    {
        if(puppet != null)
            Receiver = puppet.GetNode<PuppetTransform>(ReceiverPath);
    }
    private PuppetTransform Receiver = null;

    public override void _Process(double delta)
    {
        if(Receiver == null || ! Receiver.Active())
            return;

        var oldTrans = Receiver.GetRootTransform();

        float angle = Mathf.LerpAngle(oldTrans.Rotation, GlobalRotation, GlobalDelay);
        Vector2 origin = oldTrans.Origin.Lerp(GlobalPosition, GlobalDelay);
        if(_WorldTarget != null)
        {
            var worldTransform = Receiver.ConvertToRootTransform(_WorldTarget.GetTransform());
            angle = Mathf.LerpAngle(angle, worldTransform.Rotation, WorldTargetPercent);
            origin = origin.Lerp(worldTransform.Origin, WorldTargetPercent);
        }
        var trans = new Transform2D(
            angle,
            new Vector2(1, Receiver.GetFlip() ? -1 : 1), 0,
            origin
        );
        
        Receiver.SetRootTransform(trans);
    }
}
