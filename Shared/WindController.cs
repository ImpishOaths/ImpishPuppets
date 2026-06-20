using Godot;
using System;

[Tool]
public partial class WindController: Node
{
	public static float GlobalWindStrength = 0f;
	public static Vector2 GlobalWindDirection = new(1,0);
	[Export]
	public float WindStrength
	{
		get => GlobalWindStrength;
		set => GlobalWindStrength = value;
	}
	[Export]
	public Vector2 WindDirection
	{
		get => GlobalWindDirection;
		set => GlobalWindDirection = value;
	}

	public override void _Process(double delta)
	{
	}
}
