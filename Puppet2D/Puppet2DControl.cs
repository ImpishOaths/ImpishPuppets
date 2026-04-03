using Godot;
using System;

namespace ImpishPuppets;

public interface PuppetTransform
{
    public bool HasRoot();

    public Transform2D GetRootTransform();
    public Transform2D GetOriginTransform();
    //public Transform2D GetLocalTransform();

    public Vector2 GetRootPosition();
    public float GetRootRotation();
    public Vector2 GetRootScale();

    public void SetRootPosition(Vector2 pos);
    //public void SetOriginPosition(Vector2 pos);
    public void SetLocalPosition(Vector2 pos);

    public void SetRootRotation(float angle);
    //public void SetOriginRotation(Vector2 pos);
    public void SetLocalRotation(float angle);

    //public void SetRootScale(Vector2 scale);
    //public void SetOriginScale(Vector2 scale);
    public void SetLocalScale(Vector2 scale);

    public void SetRootTransform(Transform2D trans);
    public void SetOriginTransform(Transform2D trans);
    //public void SetLocalTransform(Transform2D trans);

    public void SetFlipH(bool flip);
    public bool GetFlipH();
    public void SetFlipV(bool flip);
    public bool GetFlipV();
}

[Tool]
[GlobalClass]
public partial class Puppet2DControl: Node2D, PuppetTransform
{
    [Export]
    public Puppet2D Puppet;

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

    public bool HasRoot() => Puppet != null;
    public Transform2D GetRootTransform()
    {
        return  Puppet.GlobalTransform.AffineInverse() * GlobalTransform;
    }
    public Transform2D GetOriginTransform()
    {
        return GlobalTransform;
    }
    public Transform2D GetLocalTransform()
    {
        return Transform;
    }

    public Vector2 GetRootPosition()
    {
        return Puppet.GlobalTransform.AffineInverse() * GlobalPosition;
    }
    public float GetRootRotation()
    {
        float scaleSign = Mathf.Sign(Puppet.GlobalTransform.Scale.X * Puppet.GlobalTransform.Scale.Y);
        return (GlobalRotation - Puppet.GlobalRotation) * scaleSign;
    }
    public Vector2 GetRootScale()
    {
        return GlobalScale / Puppet.GlobalScale;
    }

    public void SetRootTransform(Transform2D trans)
    {
        GlobalTransform = Puppet.GlobalTransform * trans;
    }
    public void SetOriginTransform(Transform2D trans)
    {
        GlobalTransform = trans;
    }

    public void SetRootPosition(Vector2 pos)
    {
        GlobalPosition = Puppet.GlobalTransform * pos;
    }
    public virtual void SetLocalPosition(Vector2 pos)
    {
        Position = pos;
    }

    public void SetRootRotation(float angle)
    {
        float scaleSign = Mathf.Sign(Puppet.GlobalTransform.Scale.X * Puppet.GlobalTransform.Scale.Y);
        GlobalRotation = (angle * scaleSign) + Puppet.GlobalRotation;
    }
    public virtual void SetLocalRotation(float angle)
    {
        Rotation = angle;
    }
    public virtual void SetLocalScale(Vector2 scale)
    {
        Scale = scale.Abs() * new Vector2(_FlipH?-1f:1f, _FlipV?-1f:1f);
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

    public virtual void UpdateLook()
    {
        if(Puppet == null)
            return;
        
        SetLocalScale(Scale);
    }
}
