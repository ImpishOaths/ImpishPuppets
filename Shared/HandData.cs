using Godot;
using Godot.Collections;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class HandData: Resource
{
    [Export]
    public Transform2D ThumbUp = Transform2D.Identity;
    [Export]
    public Transform2D Pinky = Transform2D.Identity;
    [Export]
    public Transform2D Ring = Transform2D.Identity;
    [Export]
    public Transform2D Middle = Transform2D.Identity;
    [Export]
    public Transform2D Index = Transform2D.Identity;
    [Export]
    public Transform2D ThumbDown = Transform2D.Identity;
}