using Godot;
using System;

namespace ImpishPuppets;

[Tool]
public partial class PuppetSpriteData: Resource
{
    public StringName SpriteGroup;
    public StringName SpriteName;
    public Rect2I SpriteRegion;
    public int AlternateID;
    public TileData SpriteData;
    public int SheetIndex;
}
