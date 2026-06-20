using Godot;
using Godot.Collections;
using System.IO;
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

        ResourceSaver.Save(this);
    }

    [ExportToolButton("Bake Texture")]
    public Callable BackTexturesCallable => Callable.From(BakeTextures);
    private void BakeTextures()
    {
        foreach(var tex in Textures)
        {
            var bakedImg = MakeBakedTexture(tex);
            var path = ProjectSettings.GlobalizePath(tex.ResourcePath);
            bakedImg.SavePng(Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "_baked.png");

            EditorInterface.Singleton.GetResourceFilesystem().Scan();
        }
    }

    private Image MakeBakedTexture(Texture2D tex, int upscale = 2)
    {
        Color CompareColor(Color color)
        {
            const float AlphaThreshhold = 0.5f;
            const float BlackThreshhold = 0.33f;

            if(color.A < AlphaThreshhold)
                return new Color(0, 0, 0, 0);

            int choice = 1;
            float max = color[0];
            if(color[1] > max)
            {
                choice = 2;
                max = color[1];
            }
            if(color[2] > max)
            {
                choice = 3;
                max = color[2];
            }
            if(max < BlackThreshhold)
            {
                choice = 0;
            }
            return choice switch
            {
                1 => new(1, 0, 0, 1),
                2 => new(0, 1, 0, 1),
                3 => new(0, 0, 1, 1),
                _ => new(0, 0, 0, 1),
            };
        }

        var img = tex.GetImage();
        var size = img.GetSize()*upscale;
        img.Resize(size.X, size.Y, Image.Interpolation.Bilinear);
        for(int x = 0; x < size.X; ++x)
        {
            for(int y = 0; y < size.Y; ++y)
            {
                var pix = img.GetPixel(x, y);
                img.SetPixel(x, y, CompareColor(pix));
            }
        }
        return img;
    }

    private void MakeNormalTexture()
    {
        
    }
}
