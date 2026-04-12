using Godot;
using Godot.Collections;
using System;

namespace ImpishPuppets;

[Tool]
public partial class RemoteControl: Node
{
    [Export]
    private NodePath ControllerPath;
    private PuppetController Controller;

    public override void _Ready()
    {
        Controller = GetNodeOrNull<PuppetController>(ControllerPath);
    }
}
