using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetSpriteData: Resource
{
    public StringName SpriteGroup;
    public StringName SpriteName;
    public Rect2I SpriteRegion;
    public TileData SpriteData;
}
