using Godot;
using Godot.Collections;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class CharacterData: Resource
{
    [Export]
    public string CharacterName;

    [ExportGroup("Body Scales")]
    [Export]
    public float BodyHeight = 30;
    [Export]
    public float UpperHeight = 15;
    [Export]
    public float HeadHeight = 18;
    [Export]
    public float ArmHeight = 11;
    [Export]
    public float ArmLengthL = 28;
    [Export]
    public float ArmLengthR = 34;
    [Export]
    public Vector2 HandMotionScale = Vector2.One;
    [Export]
    public float StanceWidth = 7;
    [Export]
    public Vector2 FootMotionScale = Vector2.One;

    [ExportGroup("HandPositions")]
    [Export]
    public HandData PalmOutPositions = new();
    [Export]
    public HandData PalmFlatPositions = new();
}