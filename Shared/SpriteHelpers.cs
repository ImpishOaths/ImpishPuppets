using System;
using Godot;
using Godot.Collections;

namespace ImpishPuppets;

public static class SpriteHelpers
{
    private static void AddToSpriteDict(TileSetAtlasSource spriteAtlas, Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> spriteDict, int index)
    {
        for(int i = 0; i < spriteAtlas.GetTilesCount(); ++i)
        {
            Vector2I pos = spriteAtlas.GetTileId(i);
            for(int j = 0; j < spriteAtlas.GetAlternativeTilesCount(pos); ++j)
            {
                var altId = spriteAtlas.GetAlternativeTileId(pos, j);
                var spriteData = spriteAtlas.GetTileData(pos, altId);
                var region = spriteAtlas.GetTileTextureRegion(pos);
                PuppetSpriteData sprite = new()
                {
                    SpriteGroup = (StringName)spriteData.GetCustomData("Group"),
                    SpriteName = (StringName)spriteData.GetCustomData("Name"),
                    SpriteRegion = region,
                    SpriteData = spriteData,
                    AlternateID = altId,
                    SheetIndex = index
                };
                if(! spriteDict.TryGetValue(sprite.SpriteGroup, out var names))
                {
                    names = [];
                    spriteDict[sprite.SpriteGroup] = names;
                }
                names[sprite.SpriteName] = sprite;
            }
        }
    }
    public static Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> MakeSpriteDict(this TileSet spriteSheet)
    {
        Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> spriteDict = [];
        int layers = 0;
        for(int i = 0; i < spriteSheet.GetSourceCount(); ++i)
        {
            var source = spriteSheet.GetSource(i);
            if(source is not TileSetAtlasSource atlas)
                continue;
            AddToSpriteDict(atlas, spriteDict, layers++);
        }
        return spriteDict;
    }
}