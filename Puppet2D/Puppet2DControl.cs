using Godot;
using System;

namespace ImpishPuppets;

public interface PuppetTransform
{
    public bool Active();
    public NodePath GetPath();
    
    public Transform2D GetRootTransform();
    public void SetRootTransform(Transform2D transform);

    public Transform2D GetOriginTransform();
    public void SetOriginTransform(Transform2D transform);

    public Transform2D GetLocalTransform();
    public void SetLocalTransform(Transform2D transform);

    public bool GetFlip();
    public void SetFlip(bool flip);
}

[Tool]
[GlobalClass]
public partial class Puppet2DControl: Node2D, PuppetTransform
{
    [Export]
    public Puppet2D Puppet;
    
    [Export]
    public bool Flip
    {
        get => _Flip;
        set => SetFlip(value);
    }
    private bool _Flip;

    public bool Active() => Puppet != null && Puppet.InverseTransform != null;
    public Transform2D GetRootTransform() => Puppet.InverseTransform.Value * GlobalTransform;
    public void SetRootTransform(Transform2D transform) => GlobalTransform = Puppet.GlobalTransform * transform;

    public Transform2D GetOriginTransform() => GlobalTransform;
    public void SetOriginTransform(Transform2D transform) => GlobalTransform = transform;

    public Transform2D GetLocalTransform() => Transform;    
    public void SetLocalTransform(Transform2D transform) => Transform = transform;

    public bool GetFlip() => _Flip;
    public void SetFlip(bool flip)
    {
        if(_Flip == flip)
            return;
        Scale = new Vector2(flip?-1:1,1);
        _Flip = flip;
    }

    [ExportToolButton("Add Bone")]
    public Callable AddBoneCallable => Callable.From(AddBone);
    private void AddBone()
    {
        if(Puppet == null)
            return;
        
        Puppet.MakeNewBone(this);
    }
    [ExportToolButton("Add Control")]
    public Callable AddControlCallable => Callable.From(AddControl);
    private void AddControl()
    {
        if(Puppet == null)
            return;
        
        Puppet.MakeNewControl(this);
    }
}
