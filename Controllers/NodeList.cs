using Godot;
using System;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class NodeList: Resource
{
    [Export]
    public Array<string> Nodes;
}
