using Godot;
using System.Linq;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
public partial class ClothingContainer: Node, Puppet
{
    public static ClothingContainer Instance
    {
        get
        {
            if(_Instance != null)
                return _Instance;
            _Instance = new()
            {
                ClothingTiles = ResourceLoader.Load<TileSet>("res://addons/ImpishPuppets/Clothing/Clothing.tres"),
                ClothingMaterial2D = ResourceLoader.Load<ShaderMaterial>("res://addons/ImpishPuppets/Clothing/Puppet2DClothingMaterial.tres"),
                ClothingMaterial3D = ResourceLoader.Load<ShaderMaterial>("res://addons/ImpishPuppets/Clothing/Puppet3DClothingMaterial.tres"),
                ClothingTextures = ResourceLoader.Load<TextureList>("res://addons/ImpishPuppets/Clothing/ClothingTextures.tres")
            };
            _Instance.Initialize();
            return _Instance;
        }
    }
    private static ClothingContainer _Instance;

    [Export]
    private TileSet ClothingTiles;
    [Export]
    public ShaderMaterial ClothingMaterial2D {get; private set;}
    [Export]
    public ShaderMaterial ClothingMaterial3D {get; private set;}
    [Export]
    public TextureList ClothingTextures;

    private Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> SpriteDict = [];

    public void Initialize()
    {
        SpriteDict = ClothingTiles.MakeSpriteDict();
        ClothingTextures.Initialize();
        ClothingMaterial3D.SetShaderParameter("spriteSheets", ClothingTextures);
    }

    public override void _Ready()
    {
        _Instance = this;
        Initialize();
    }

    public (PuppetSpriteData data, Texture2D texture) GetClothing(StringName group, StringName name)
    {
        var sprite = SpriteDict[group][name];
        var tex = ClothingTextures.Textures[sprite.SheetIndex];
        return (sprite, tex);
    }

    public PuppetSpriteData GetSpriteReference(StringName group, StringName sprite)
    {
        if(group == null || ! SpriteDict.TryGetValue(group, out var sprites))
            return null;
        if(sprite == null || ! sprites.TryGetValue(sprite, out var ret))
            return null;
        return ret;
    }

    public Array<StringName> GetSpriteGroups() => [..SpriteDict.Keys];
    public Array<StringName> GetSpritesInGroup(StringName group)
    {
        if(group != null && SpriteDict.TryGetValue(group, out var sprites))
            return [..sprites.Keys];
        return [];
    }
    public StringName GetFirstGroup()
    {
        if(SpriteDict.Count == 0)
            return null;
        return SpriteDict.First().Key;
    }
    public PuppetSpriteData GetFirstSprite(StringName group)
    {
        if(group == null || SpriteDict == null || !SpriteDict.TryGetValue(group, out var sprites) || sprites.Count == 0)
            return null;
        return sprites.First().Value;
    }

    public Node GetNode() => this;

    public void MakeTilesDirty()
    {
        ClothingTiles.SetMeta("dirty", true);
    }

    public Texture2D GetTexture(StringName group, StringName name)
    {
        if(group == "")
            return null;
        var sprite = SpriteDict[group][name];
        return ClothingTextures.Textures[sprite.SheetIndex];
    }

    public Texture2D GetImageTexture(StringName group, StringName name)
    {
        if(group == "")
            return null;
        var sprite = SpriteDict[group][name];
        return ClothingTextures.ImageTextures[sprite.SheetIndex];
    }

}
