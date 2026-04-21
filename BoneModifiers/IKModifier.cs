using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKModifier: PuppetBoneModifier
{
    [Export(PropertyHint.NodePathValidTypes, "IKModifier")]
    public NodePath NextPath;
    private IKModifier Next = null;

    [Export]
    public float Length = 10;
    [Export]
    public float AngleNudge;
    [Export]
    public Vector2 TargetOffset;
    [Export]
    public float LockAngleSign = 0f;

    public override void Apply(float delta) {}

    public void ForwardPass(PuppetTransform target, bool doFlip, Vector2? basePos)
    {
        Receiver ??= GetParent<PuppetTransform>();
        if(Receiver == null || ! Receiver.Active())
            return;

        if(NextPath != null)
            Next ??= GetNodeOrNull<IKModifier>(NextPath);

        var trans = Receiver.GetRootTransform();
        var basePosStore = trans.Origin;
        var targetTrans = target.GetRootTransform().TranslatedLocal(TargetOffset);
        var targetPos = targetTrans.Origin;
        var diff = (targetPos - trans.Origin).Normalized() * Length;
        
        var nudgeSign = doFlip?-1:1;
        var angle = diff.Angle() + AngleNudge * nudgeSign;
        if(LockAngleSign != 0.0)
        {
            var angleDiff = Mathf.AngleDifference(angle, targetTrans.Rotation) * LockAngleSign * nudgeSign;
            if(angleDiff < 0)
                angle = targetTrans.Rotation;
        }
        Vector2 pos = targetPos - diff;
        trans = new Transform2D(angle, trans.Scale, trans.Skew, pos);

        if(Next != null)
        {
            Receiver.SetRootTransform(trans);
            Next.ForwardPass(Receiver, doFlip, basePos);
            BackwardPass();
        }
        else
        {
            if(basePos.HasValue)
                trans.Origin = basePos.Value;
            trans.Origin = basePosStore;
            Receiver.SetRootTransform(trans);
        }
    }

    public void BackwardPass()
    {
        var nextTrans = Next.Receiver.GetRootTransform();
        var trans = Receiver.GetRootTransform();
        trans.Origin = nextTrans.Origin + Vector2.FromAngle(nextTrans.Rotation)*Next.Length;
        Receiver.SetRootTransform(trans);
    }

    public override PuppetBoneModifier MakeDuplicate3D(Vector2 resize)
    {
        var duplicate = Duplicate() as IKModifier;
        duplicate.Length *= Mathf.Min(resize.X,resize.Y);
        duplicate.TargetOffset *= resize;
        return duplicate;
    }

}
