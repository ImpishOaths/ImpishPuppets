using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetTransform3D: Node3D, PuppetTransform
{

    [Export]
    public Puppet3D Puppet;
    
    [Export]
    public bool Flip
    {
        get => _Flip;
        set => SetFlip(value);
    }
    private bool _Flip;

    public virtual void Initialize(Puppet3D puppet, PuppetTransform2D trans)
    {
        Puppet = puppet;

        Scale = trans.Scale.Abs().ToVec3scale();
        Position = (trans.Position / (VectorHelpers.PixelSizeRoot*VectorHelpers.PixelSizeRoot)).ToVec3pos();
        Rotation = new Vector3(0, 0, -trans.Rotation);

        if(trans.Flip) //Prevents an annoying bug where it dosen't flip when it's supposed to
        {
            SetFlip(false);
            SetFlip(true);
        }
    }
    
    public bool Active() => Puppet != null && Puppet.InverseTransform != null;

    public Transform2D GetRootTransform() => (Puppet.InverseTransform.Value * GlobalTransform).To2D(_Flip);
    public void SetRootTransform(Transform2D trans) => GlobalTransform = Puppet.GlobalTransform * trans.To3D();

    public Transform2D GetOriginTransform() => GlobalTransform.To2D(_Flip);
    public void SetOriginTransform(Transform2D trans) => GlobalTransform = trans.To3D();

    public virtual Transform2D GetLocalTransform() => Transform.To2D(_Flip);
    public virtual void SetLocalTransform(Transform2D trans) => Transform = trans.To3D();

    public bool GetFlip() => _Flip;
    public virtual void SetFlip(bool flip)
    {
        if(_Flip == flip)
            return;
        
        Rotation = new Vector3(0, flip?Mathf.Pi:0, ((Scale.X < 0) ^ flip) ? Mathf.Pi - Rotation.Z : Rotation.Z);
        Scale = new Vector3(flip?-1:1, flip?-1:1, flip?-1:1);
        _Flip = flip;
    }
}
