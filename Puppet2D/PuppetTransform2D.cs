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
public partial class PuppetTransform2D: Node2D, PuppetTransform
{
    [Export]
    public Puppet2D Puppet;
    
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

    public Transform2D GetOriginTransform() => GlobalTransform;
    public void SetOriginTransform(Transform2D transform) => GlobalTransform = transform;

    public Transform2D GetLocalTransform() => Transform;    
    public void SetLocalTransform(Transform2D transform) => Transform = transform;

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
}
