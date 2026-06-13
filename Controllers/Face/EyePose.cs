using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class EyePose: Resource
{
	[Export]
	public Vector2 PupilOffset;
	[Export]
	public bool ShowPupil = true;
	[Export(PropertyHint.Enum,"Full,Narrow,Closed,Arch,ArchDown")]
	public StringName EyeShape;
	[Export]
	public bool DoBlinks = true;
}
