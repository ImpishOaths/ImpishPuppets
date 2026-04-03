using Godot;
using System;

namespace ImpishPuppets;

public interface PuppetTransform
{
    public bool HasRoot();
    public NodePath GetPath();
    
    public Transform2D GetRootTransform();
    public void SetRootTransform(Transform2D transform);

    public Transform2D GetOriginTransform();
    public void SetOriginTransform(Transform2D transform);

    public Transform2D GetLocalTransform();
    public void SetLocalTransform(Transform2D transform);
}

[Tool]
[GlobalClass]
public partial class Puppet2DControl: Node2D, PuppetTransform
{
    [Export]
    public Puppet2D Puppet;

    public bool HasRoot() => Puppet != null && Puppet.InverseTransform != null;
    public Transform2D GetRootTransform() => Puppet.InverseTransform.Value * GlobalTransform;
    public void SetRootTransform(Transform2D transform) => GlobalTransform = Puppet.GlobalTransform * transform;

    public Transform2D GetOriginTransform() => GlobalTransform;
    public void SetOriginTransform(Transform2D transform) => GlobalTransform = transform;

    public Transform2D GetLocalTransform() => Transform;    
    public void SetLocalTransform(Transform2D transform) => Transform = transform;

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
