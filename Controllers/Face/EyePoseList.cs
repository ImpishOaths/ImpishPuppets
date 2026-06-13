using Godot;
using System;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class EyePoseList : Resource
{
	[Export]
	public Dictionary<StringName, EyePose> Poses;
}
