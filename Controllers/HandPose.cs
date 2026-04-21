using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class HandPose: Resource
{
    [Export]
    public bool HandBehind = false;
    [Export]
    public bool PalmFlat = false;
    [Export]
    public bool PalmBack = false;
    [Export]
    public bool PinkyUp = false;
    [Export]
    public bool RingUp = false;
    [Export]
    public bool MiddleUp = false;
    [Export]
    public bool IndexUp = false;
    [Export]
    public bool ThumbUp = false;
}
