using Godot;
using System;

namespace ImpishPuppets;

[Tool]
public partial class DepthMarker: Node
{
    [Export]
    public NodePath Marked = null;
}
