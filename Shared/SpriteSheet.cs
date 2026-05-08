using Godot;
using Godot.Collections;
using System.Linq;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class SpriteSheet: TileSet
{
    [ExportToolButton("Reload Sprites")]
    public Callable AddTransformCallable => Callable.From(ReloadSpriteDict);

    [Export]
    public Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> SpriteDict;
    [Export]
    private Array<Texture2D> Textures;

    public System.Collections.Generic.IEnumerable<StringName> GetGroups() => SpriteDict.Keys;
    public System.Collections.Generic.IEnumerable<StringName> GetSpritesInGroup(StringName group)
    {
        if(group == null || group == "" || SpriteDict == null)
            yield break;
        if(SpriteDict.TryGetValue(group, out var sprites) == false)
            yield break;
        foreach(var key in sprites.Keys)
            yield return key;
    }
    public StringName GetFirstGroup()
    {
        if(SpriteDict == null)
            return null;
        if(SpriteDict.Count > 0)
            return SpriteDict.Keys.First();
        return null;
    }
    public StringName GetFirstSpriteInGroup(StringName group)
    {
        if(group == null || group == "" || SpriteDict == null)
            return null;
        if(SpriteDict.TryGetValue(group, out var sprites) && sprites.Count > 0)
            return sprites.Keys.First();
        return null;
    }
    public PuppetSpriteData GetSprite(StringName group, StringName sprite)
    {
        if(group == "" || sprite == "" || group == null || sprite == null || SpriteDict == null)
            return null;
        if(SpriteDict.TryGetValue(group, out var sprites) == false)
            return null;
        if(sprites.TryGetValue(sprite, out var spriteReference))
            return spriteReference;
        return null;
    }
    public TileData GetSpriteData(StringName group, StringName sprite)
    {
        var spriteData = GetSprite(group, sprite);
        if(spriteData == null)
            return null;
        var atlas = (TileSetAtlasSource)GetSource(spriteData.SourceIndex);
        return atlas.GetTileData(spriteData.SpriteRegion.Position/atlas.TextureRegionSize, spriteData.AlternateID);
    }

    public Vector2I GetTextureRegionSize() => ((TileSetAtlasSource)GetSource(0)).TextureRegionSize;

    public Texture2D GetSpriteTexture(int layer) => Textures[layer];

    public void UpdateData(StringName group, StringName sprite, StringName layerName, Variant value)
    {
        GetSpriteData(group, sprite)?.SetCustomData(layerName, value);
        EmitChanged();
    }
    
    public void ReloadSpriteDict()
    {
        SpriteDict = [];
        Textures = [];

        for(int i = 0; i < GetSourceCount(); ++i)
        {
            var source = GetSource(i);
            if(source is not TileSetAtlasSource atlas)
            {
                Textures.Add(null);
                continue;
            }
            Textures.Add(atlas.Texture);

            for(int j = 0; j < atlas.GetTilesCount(); ++j)
            {
                Vector2I pos = atlas.GetTileId(j);
                for(int k = 0; k < atlas.GetAlternativeTilesCount(pos); ++k)
                {
                    var altId = atlas.GetAlternativeTileId(pos, k);
                    var spriteData = atlas.GetTileData(pos, altId);
                    var region = atlas.GetTileTextureRegion(pos);
                    PuppetSpriteData sprite = new()
                    {
                        SpriteGroup = (StringName)spriteData.GetCustomData("Group"),
                        SpriteName = (StringName)spriteData.GetCustomData("Name"),
                        SpriteRegion = region,
                        AlternateID = altId,
                        SourceIndex = i
                    };
                    if(! SpriteDict.TryGetValue(sprite.SpriteGroup, out var names))
                    {
                        names = [];
                        SpriteDict[sprite.SpriteGroup] = names;
                    }
                    names[sprite.SpriteName] = sprite;
                }
            }
        }
    }
}
