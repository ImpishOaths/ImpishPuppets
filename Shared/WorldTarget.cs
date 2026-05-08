using Godot;
using System;
using System.Collections.Generic;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class WorldTarget: Node
{
    private static readonly Dictionary<StringName, WorldTarget> Targets = [];
    public static WorldTarget GetTarget(StringName name)
    {
        if(Targets.TryGetValue(name, out var value))
            return value;
        return null;
    }
    public Transform2D GetTransform()
    {
        if(Mode3D)
        {
            return Parent3D.GlobalTransform.To2D();
        }
        else
        {
            return Parent2D.GlobalTransform;
        }
    }

    private bool Mode3D = false;
    private Node2D Parent2D = null;
    private Node3D Parent3D = null;

    public override void _EnterTree()
    {
        Targets[Name] = this;
        var parent = GetParent();
        if(parent is Node3D three)
        {
            Parent3D = three;
            Mode3D = true;
        }
        else if(parent is Node2D two)
        {
            Parent2D = two;
        }
    }

    public override void _ExitTree()
    {
        Targets.Remove(Name);
    }


}
