using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKModifier: PuppetBoneModifier
{
    [Export(PropertyHint.NodePathValidTypes, "IKModifier")]
    private NodePath NextPath;
    public IKModifier Next = null;
    [Export]
    public float Length = 10;
    [Export]
    public float AngleNudge;
    [Export]
    public Vector2 TargetOffset;
    [Export]
    public float LockAngleSign = 0f;

    public override void Apply(float delta) {}
    public override void Initialize()
    {
        Next = GetIKModifier(NextPath);
    }

    protected IKModifier GetIKModifier(NodePath path)
    {
        
        var mod = GetNodeOrNull(path);
        if(mod == null)
            return null;
        if(mod is IKModifier ikMod)
            return ikMod;
        return null;
    }

    public Vector2 GetChainRootPos()
    {
        if(Next != null)
            return Next.GetChainRootPos();
        return Receiver.GetRootPosition();
    }

    public void ForwardPass(PuppetTransform target, float nudgeSign)
    {
        var receiverPos = Receiver.GetRootPosition();
        var targetTrans = target.GetRootTransform().TranslatedLocal(TargetOffset);
        var targetPos = targetTrans.Origin;
        var diff = (targetPos - receiverPos).Normalized() * Length;
        Receiver.SetRootPosition(targetPos - diff);
        var angle = diff.Angle() + AngleNudge * nudgeSign;
        if(LockAngleSign != 0.0)
        {
            var angleDiff = Mathf.AngleDifference(angle, targetTrans.Rotation) * LockAngleSign * nudgeSign;
            if(angleDiff < 0)
                angle = targetTrans.Rotation;
        }
        Receiver.SetRootRotation(angle);

        Next?.ForwardPass(Receiver, nudgeSign);
    }

    public Vector2 BackwardPass(Vector2 position)
    {
        if(Next != null)
        {
            position = Next.BackwardPass(position);
        }

        Receiver.SetRootPosition(position);
        return position + Vector2.FromAngle(Receiver.GetRootRotation())*Length;
    }
}
