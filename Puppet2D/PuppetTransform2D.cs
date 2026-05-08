using Godot;
using System;

namespace ImpishPuppets;

public interface PuppetTransform
{
    public bool Active();
    public CharacterData GetCharacterData();
    
    public Transform2D GetRootTransform();
    public void SetRootTransform(Transform2D transform);
    public Transform2D ConvertToRootTransform(Transform2D global);

    public Transform2D GetOriginTransform();
    public void SetOriginTransform(Transform2D transform);

    public Transform2D GetLocalTransform();
    public void SetLocalTransform(Transform2D transform);

    public bool GetFlip();
    public void SetFlip(bool flip);
    public void PropogateOrderFlip(bool order, bool flip);
}

public interface Puppet2Dto3DConverter
{
    public Node ConvertTo3D(Puppet3D puppet);
}

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Transform2DIcon.png")]
public partial class PuppetTransform2D: Node2D, PuppetTransform, Puppet2Dto3DConverter
{
    [Export]
    public Puppet2D Puppet;
    public CharacterData GetCharacterData() => Puppet.CharacterData;
    
    [Export]
    public bool Flip
    {
        get => _Flip;
        set => SetFlip(value);
    }
    protected bool _Flip;

    public bool Active() => Puppet != null && Puppet.InverseTransform != null;
    public Transform2D GetRootTransform() => Puppet.InverseTransform.Value * GlobalTransform;
    public void SetRootTransform(Transform2D transform) => GlobalTransform = Puppet.GlobalTransform * transform;
    public Transform2D ConvertToRootTransform(Transform2D global) => Puppet.InverseTransform.Value * global;

    public Transform2D GetOriginTransform() => GlobalTransform;
    public void SetOriginTransform(Transform2D transform) => GlobalTransform = transform;

    public virtual Transform2D GetLocalTransform() => Transform;    
    public virtual void SetLocalTransform(Transform2D transform) => Transform = transform;

    public bool GetFlip() => _Flip;
    public void SetFlip(bool flip)
    {
        if(_Flip == flip)
            return;

        _Flip = flip;
        
        Scale = new Vector2(_Flip?-1:1,1);
    }

    [ExportToolButton("Add Bone")]
    public Callable AddBoneCallable => Callable.From(AddBone);
    private void AddBone()
    {
        if(Puppet == null)
            return;
        
        Puppet.MakeNewBone(this);
    }
    [ExportToolButton("Add Transform")]
    public Callable AddTransformCallable => Callable.From(AddTransform);
    private void AddTransform()
    {
        if(Puppet == null)
            return;
        
        Puppet.MakeNewTransform(this);
    }

    public virtual Node ConvertTo3D(Puppet3D puppet)
    {
        return new PuppetTransform3D()
        {
            Scale = Scale.Abs().ToVec3scale(),
            Position = (Position / (VectorHelpers.PixelSizeRoot * VectorHelpers.PixelSizeRoot)).ToVec3pos(),
            Rotation = new Vector3(0, 0, -Rotation),
            Puppet = puppet,
            Flip = _Flip
        };
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
