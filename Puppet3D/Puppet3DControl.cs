using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet3DControl: Node3D, PuppetTransform
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

    public virtual void Initialize(Puppet3D puppet, Puppet2DControl control)
    {
        Puppet = puppet;

        Position = (control.Position / Puppet.TileSize).ToVec3pos();
        Rotation = new Vector3(0, 0, -control.Rotation);
        _Flip = control.Flip;
        SetFlip(control.Flip);
    }
    
    public bool Active() => Puppet != null && Puppet.InverseTransform != null;

    public Transform2D GetRootTransform() => (Puppet.InverseTransform.Value * GlobalTransform).To2D(_Flip);
    public void SetRootTransform(Transform2D trans) => GlobalTransform = Puppet.GlobalTransform * trans.To3D();

    public Transform2D GetOriginTransform() => GlobalTransform.To2D(_Flip);
    public void SetOriginTransform(Transform2D trans) => GlobalTransform = trans.To3D();

    public Transform2D GetLocalTransform() => Transform.To2D(_Flip);
    public void SetLocalTransform(Transform2D trans) => Transform = trans.To3D();

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
