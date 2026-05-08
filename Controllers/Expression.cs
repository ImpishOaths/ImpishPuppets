using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Expression: Resource
{
    [Export]
    public StringName ExpressionName;
    [Export]
    public Vector2 Position;
    [Export]
    public Curve ScaleXCurve;
    [Export]
    public Curve ScaleYCurve;
    [Export]
    public Curve RotationCurve;
    [Export]
    public float Time = 1f;
}
