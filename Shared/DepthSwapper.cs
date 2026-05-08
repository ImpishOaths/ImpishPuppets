using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class DepthSwapper: Node
{
    [Export]
    public string SwapPath;
}
