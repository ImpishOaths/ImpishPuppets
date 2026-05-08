using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Transform3DIcon.png")]
public partial class PuppetTransform3D: Node3D, PuppetTransform
{
    [Export]
    public Puppet3D Puppet;
    public CharacterData GetCharacterData() => Puppet.CharacterData;
    
    [Export]
    public bool Flip
    {
        get => _Flip;
        set => SetFlip(value);
    }
    protected bool _Flip;
    
    public bool Active() => Puppet != null && Puppet.InverseTransform != null;

    public Transform2D GetRootTransform() => (Puppet.InverseTransform.Value * GlobalTransform).To2D(_Flip);
    public void SetRootTransform(Transform2D trans) => GlobalTransform = Puppet.GlobalTransform * trans.To3D();
    public Transform2D ConvertToRootTransform(Transform2D global) => (Puppet.InverseTransform.Value * global.To3D()).To2D(_Flip);

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

    public virtual void PropogateOrderFlip(bool order, bool flip)
    {
        SetFlip(flip);
        foreach(var child in GetChildren())
        {
            if(child is PuppetTransform trans)
                trans.PropogateOrderFlip(order, flip);
        }
    }
}
