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
    public bool FlipH
    {
        get => _FlipH;
        set
        {
            if(_FlipH != value)
            {
                _FlipH = value;
                UpdateLook();
            }
        }
    }
    protected bool _FlipH;
    public void SetFlipH(bool flip) => FlipH = flip;
    public bool GetFlipH() => FlipH;

    [Export]
    public bool FlipV
    {
        get => _FlipV;
        set
        {
            if(_FlipV != value)
            {
                _FlipV = value;
                UpdateLook();
            }
        }
    }
    protected bool _FlipV;
    public void SetFlipV(bool flip) => FlipV = flip;
    public bool GetFlipV() => FlipV;

    public virtual void Initialize(Puppet3D puppet, Puppet2DControl control)
    {
        Puppet = puppet;

        Position = (control.Position / Puppet.TileSize).ToVec3pos();
        Rotation = new Vector3(0, 0, -control.Rotation);
        Scale = control.Scale.ToVec3scale();
    }
    
    public bool HasRoot() => Puppet != null;

    public Transform2D GetOriginTransform()
    {
        var trans = GlobalTransform;
        var basis = trans.Basis;
        return new (basis.GetEuler(EulerOrder.Yxz).Z, trans.Origin.ToVec2pos(), 0, basis.Scale.ToVec2scale());
    }

    public Vector2 GetRootPosition()
    {
        var trans = Puppet.GlobalTransform.AffineInverse() * GlobalTransform;
        return trans.Origin.ToVec2pos();
    }

    public float GetRootRotation()
    {
        var trans = Puppet.GlobalTransform.AffineInverse() * GlobalTransform;
        return trans.Basis.GetEuler(EulerOrder.Yxz).Z;
    }

    public Vector2 GetRootScale()
    {
        var trans = Puppet.GlobalTransform.AffineInverse() * GlobalTransform;
        return trans.Basis.Scale.ToVec2scale();
    }

    public Transform2D GetRootTransform()
    {
        var trans = Puppet.GlobalTransform.AffineInverse() * GlobalTransform;
        var basis = trans.Basis;
        return new (basis.GetEuler(EulerOrder.Yxz).Z, trans.Origin.ToVec2pos(), 0, basis.Scale.ToVec2scale());
    }

    public void SetLocalPosition(Vector2 pos)
    {
        Position = pos.ToVec3pos();
    }

    public void SetLocalRotation(float angle)
    {
        Rotation = new(0, 0, angle);
    }

    public void SetLocalScale(Vector2 scale)
    {
        Scale = scale.ToVec3scale();
    }

    public void SetOriginTransform(Transform2D trans)
    {
        var trans3 =  Transform3D.Identity;
        trans3 = trans3.Rotated(Vector3.Back, trans.Rotation);
        trans3 = trans3.Translated(trans.Origin.ToVec3pos());
        trans3 = trans3.Scaled(trans.Scale.ToVec3scale());
        GlobalTransform = trans3;
    }

    public void SetRootPosition(Vector2 pos)
    {
        GlobalPosition = Puppet.GlobalPosition + pos.ToVec3pos();
    }

    public void SetRootRotation(float angle)
    {
        GlobalRotation = Puppet.GlobalRotation + new Vector3(0, 0, angle);
    }

    public void SetRootTransform(Transform2D trans)
    {
        var trans3 =  Puppet.GlobalTransform;
        trans3 = trans3.Rotated(Vector3.Back, trans.Rotation);
        trans3 = trans3.Translated(trans.Origin.ToVec3pos());
        trans3 = trans3.Scaled(trans.Scale.ToVec3scale());
        GlobalTransform = trans3;
    }

    public virtual void UpdateLook()
    {
        
    }
}
