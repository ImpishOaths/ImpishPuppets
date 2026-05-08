using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class HandPoseList: Resource
{
    [Export]
    public Dictionary<StringName, HandPose> Poses;
}