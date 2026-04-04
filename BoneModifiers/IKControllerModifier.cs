using Godot;
using System;
using System.Collections.Generic;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKControllerModifier: PuppetBoneModifier
{
    [Export(PropertyHint.NodePathValidTypes, "IKElementModifier")]
    public Godot.Collections.Array<NodePath> IKChainPaths = null;
    public List<IKElementModifier> IKChain = null;

    [Export(PropertyHint.NodePathValidTypes, "Puppet2DBone,Puppet2DControl,Puppet3DBone,Puppet3DControl")]
    public NodePath BasePath;
    private PuppetTransform Base = null;
    
    [Export]
    public Vector2 BaseOffset;

    public List<IKElementModifier> GetIKChain()
    {
        List<IKElementModifier> chain = [];
        foreach(var path in IKChainPaths)
        {
            if(path == null)
                continue;
            chain.Add(GetNode<IKElementModifier>(path));
        }
        return chain;
    }

    public override void Apply(float delta)
    {
        Base ??= GetNodeOrNull<PuppetTransform>(BasePath);
        if(Base == null || ! Base.Active())
            return;
        if(IKChainPaths == null)
            return;
        IKChain ??= GetIKChain();

        var target = Receiver;
        float bendDirection = Receiver.GetFlip() ? -1 : 1;
        foreach(var element in IKChain)
        {
            element.Receiver.SetFlip(Receiver.GetFlip());
            ForwardPass(element, target, bendDirection);
            target = element.Receiver;
        }
        var targetPos = Base.GetRootTransform().Origin + BaseOffset;
        for(int i = IKChain.Count-1; i >= 0; i--)
        {
            targetPos = BackwardPass(IKChain[i], targetPos);
        }
    }

    private static void ForwardPass(IKElementModifier receiver, PuppetTransform target, float bendDirection)
    {
        var trans = receiver.Receiver.GetRootTransform();
        var targetTrans = target.GetRootTransform().TranslatedLocal(receiver.TargetOffset);
        var targetPos = targetTrans.Origin;
        var diff = (targetPos - trans.Origin).Normalized() * receiver.Length;
        
        var angle = diff.Angle() + receiver.AngleNudge * bendDirection;
        if(receiver.LockAngleSign != 0.0)
        {
            var angleDiff = Mathf.AngleDifference(angle, targetTrans.Rotation) * receiver.LockAngleSign * bendDirection;
            if(angleDiff < 0)
                angle = targetTrans.Rotation;
        }
        Vector2 pos = targetPos - diff;
        trans = new Transform2D(angle, trans.Scale, trans.Skew, pos);
        receiver.Receiver.SetRootTransform(trans);
    }

    private static Vector2 BackwardPass(IKElementModifier receiver, Vector2 targetPos)
    {
        var trans = receiver.Receiver.GetRootTransform();
        trans.Origin = targetPos;
        receiver.Receiver.SetRootTransform(trans);
        return targetPos + Vector2.FromAngle(trans.Rotation) * receiver.Length;
    }

    public override PuppetBoneModifier Make3DDuplicate(Vector2 resize)
    {
        var duplicate = Duplicate() as IKControllerModifier;
        duplicate.BaseOffset *= resize;
        return duplicate;
    }
}
