using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class RemotePuppetTransform: Node2D
{
    private static readonly float GlobalDelay = 1f;

    [Export]
    public string ReceiverPath;
    [Export]
    public bool Flip;

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

        float angle = GlobalRotation;
        if(Flip)
        {
            angle += Mathf.Pi;
        }
        angle = Mathf.LerpAngle(oldTrans.Rotation, angle, GlobalDelay);
        Vector2 origin = oldTrans.Origin.Lerp(GlobalPosition, GlobalDelay);
        if(_WorldTarget != null)
        {
            var worldTransform = Receiver.ConvertToRootTransform(_WorldTarget.GetTransform());
            angle = Mathf.LerpAngle(angle, worldTransform.Rotation, WorldTargetPercent);
            origin = origin.Lerp(worldTransform.Origin, WorldTargetPercent);
        }
        var trans = new Transform2D(
            angle,
            new Vector2(1, (Receiver.GetFlip() || Flip) ? -1 : 1), 0,
            origin
        );

        Receiver.SetRootTransform(trans);
        if(Receiver is not PuppetTransform3D receiver3D)
            return;
        receiver3D.RotateObjectLocal(Vector3.Up, AxisAngle);
    }

    public void SetAxisRotation(float angle)
    {
        if(Receiver is not PuppetTransform3D receiver3D)
            return;
        receiver3D.RotateObjectLocal(Vector3.Up, -AxisAngle);
        AxisAngle = angle;
        receiver3D.RotateObjectLocal(Vector3.Up, AxisAngle);
    }
    [Export]
    private float AxisAngle = 0;
}
