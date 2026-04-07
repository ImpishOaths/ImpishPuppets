using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class HandPoseList: Resource
{
    [Export]
    public Dictionary<StringName, HandPose> Poses;
}