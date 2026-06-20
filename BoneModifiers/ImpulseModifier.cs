using Godot;
using System;

namespace ImpishPuppets;

[Tool]
public abstract partial class ImpulseModifier: PuppetBoneModifier
{
    [Export]
    protected Vector2 ImpulseDir = new(1, 1);
    [Export]
    protected Vector2 MinMax = new(-1, 1);
    [Export]
    protected float Sensitivity = 10f;
    [Export]
    protected float RestoreForce = 200f;
    [Export]
    protected float Drag = 0.9f;
    
    private Vector2? PreviousPosition = null;
    private float Value = 0;
    private float Velocity = 0;

    private void SimulateImpulse(Vector2 pos, float rootAngle, float delta)
    {
        if(PreviousPosition == null)
            PreviousPosition = pos;

        var diff = pos - PreviousPosition.Value - WindController.GlobalWindDirection*WindController.GlobalWindStrength;
        var normDir = ImpulseDir.Normalized().Rotated(rootAngle);

        var power = diff.Dot(normDir) * Sensitivity;

        Value += Velocity * delta;
        if(MinMax.X < MinMax.Y)
        {
            var preValue = Value;
            Value = Mathf.Clamp(Value, MinMax.X, MinMax.Y);
            if(Value != preValue)
                Velocity *= -0.5f;
        }
        
        Velocity += (power - Value*RestoreForce) * delta;
        Velocity *= Drag;

        PreviousPosition = pos;
    }

    public override void Apply(float delta)
    {
        var parentTrans = Receiver.GetOriginTransform();
        SimulateImpulse(parentTrans.Origin, parentTrans.Rotation, delta);
        ApplyImpulse(Value);
    }

    public abstract void ApplyImpulse(float value);
}
