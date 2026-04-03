using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet3DControl: Node3D, PuppetTransform
{
    [Export]
    public Puppet3D Puppet;

    public virtual void Initialize(Puppet3D puppet, Puppet2DControl control)
    {
        Puppet = puppet;

        Position = (control.Position / Puppet.TileSize).ToVec3pos();
        Rotation = new Vector3(0, 0, -control.Rotation);
        Scale = control.Scale.ToVec3scale();
    }
    
    public bool HasRoot() => Puppet != null && Puppet.InverseTransform != null;

    public Transform2D GetRootTransform() => (Puppet.InverseTransform.Value * GlobalTransform).To2D();
    public void SetRootTransform(Transform2D trans) => GlobalTransform = Puppet.GlobalTransform * trans.To3D();

    public Transform2D GetOriginTransform() => GlobalTransform.To2D();
    public void SetOriginTransform(Transform2D trans) => GlobalTransform = trans.To3D();

    public Transform2D GetLocalTransform() => Transform.To2D();
    public void SetLocalTransform(Transform2D trans) => Transform = trans.To3D();
}
