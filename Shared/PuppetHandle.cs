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

    public override void _Process(double delta)
    {
        if(ReceiverPath != null)
            Receiver ??= GetNodeOrNull<PuppetTransform>(ReceiverPath);

        if(Receiver == null || ! Receiver.Active())
            return;
        
        var trans = Receiver.GetRootTransform();
        trans = new Transform2D(GlobalRotation, trans.Scale, trans.Skew, GlobalPosition);
        Receiver.SetRootTransform(trans);
    }
}
