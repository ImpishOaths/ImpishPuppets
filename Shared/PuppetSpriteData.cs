using Godot;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetSpriteData: Resource
{
    [Export]
    public StringName SpriteGroup;
    [Export]
    public StringName SpriteName;
    [Export]
    public Rect2I SpriteRegion;
    [Export]
    public int AlternateID;
    [Export]
    public int SourceIndex;
} 