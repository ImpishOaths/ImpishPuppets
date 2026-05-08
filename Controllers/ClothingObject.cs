using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class ClothingObject: Resource
{
    [Export]
    public SpriteSheet Sheet;
    [Export]
    public bool UseCustomMaterial;
    [Export]
    public Material Material2D;
    [Export]
    public Material Material3D;
}
