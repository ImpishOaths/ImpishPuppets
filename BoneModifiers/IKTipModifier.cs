using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class IKTipModifier: IKModifier
{
    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetTransform2D,PuppetBone3D,PuppetTransform3D")]
    public NodePath TargetPath;
    private PuppetTransform Target = null;

    //[Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetTransform2D,PuppetBone3D,PuppetTransform3D")]
    //public NodePath BasePath;
    //private PuppetTransform Base = null;
    //[Export]
    //private Vector2 BaseOffset;

    public override void Apply(float delta)
    {
        Target ??= GetNodeOrNull<PuppetTransform>(TargetPath);
        if(Target == null || ! Target.Active())
            return;
        /*
        Vector2? basePos = null;
        if(BasePath != null)
        {
            Base ??= GetNodeOrNull<PuppetTransform>(BasePath);
            if(Base == null || ! Base.Active())
                return;
            var baseTrans = Base.GetRootTransform();
            basePos = baseTrans.TranslatedLocal(BaseOffset).Origin;
        }*/

        //ForwardPass(Target, Target.GetFlip(), basePos);
        ForwardPass(Target, Target.GetFlip(), null);
    }

    
    public override PuppetBoneModifier MakeDuplicate3D(Vector2 resize)
    {
        var duplicate = Duplicate() as IKTipModifier;
        duplicate.Length *= Mathf.Min(resize.X,resize.Y);
        duplicate.TargetOffset *= resize;
        //duplicate.BaseOffset *= resize;
        return duplicate;
    }
}
