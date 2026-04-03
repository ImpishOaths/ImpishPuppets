using Godot;
using Godot.Collections;

namespace ImpishPuppets;

public static class VectorHelpers
{
    public static Vector3 ToVec3pos(this Vector2 vec) => new(vec.X, -vec.Y, 0);
    public static Vector2 ToVec2pos(this Vector3 vec) => new(vec.X, -vec.Y);
    public static Vector2 ToVec2scale(this Vector3 vec) => new(vec.X, vec.Y);
    public static Vector3 ToVec3scale(this Vector2 vec) => new(vec.X, vec.Y, 1);
    public static Vector4 ToVec4(this Rect2 rect) => new(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);
    public static Rect2 Scale(this Rect2 rect, Vector2 vec) => new(rect.Position*vec, rect.Size*vec);
    public static Transform2D To2D(this Transform3D trans3) => new(trans3.Basis.GetEuler().Z, trans3.Basis.Scale.ToVec2scale(), 0, trans3.Origin.ToVec2pos());
    public static Transform3D To3D(this Transform2D trans2) => new(new Basis(Quaternion.FromEuler(new Vector3(0, 0, trans2.Rotation))).Scaled(trans2.Scale.ToVec3scale()), trans2.Origin.ToVec3pos());

    public static Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> MakeSpriteDict(this TileSet spriteSheet)
    {
        var spriteAtlas = (TileSetAtlasSource)spriteSheet.GetSource(0);
        Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> SpriteDict = [];
        for(int i = 0; i < spriteAtlas.GetTilesCount(); ++i)
        {
            Vector2I pos = spriteAtlas.GetTileId(i);
            for(int j = 0; j < spriteAtlas.GetAlternativeTilesCount(pos); ++j)
            {
                var altId = spriteAtlas.GetAlternativeTileId(pos, j);
                var spriteData = spriteAtlas.GetTileData(pos, altId);
                PuppetSpriteData sprite = new()
                {
                    SpriteGroup = (StringName)spriteData.GetCustomData("Group"),
                    SpriteName = (StringName)spriteData.GetCustomData("Name"),
                    SpriteRegion = spriteAtlas.GetTileTextureRegion(pos),
                    SpriteData = spriteData
                };

                if(! SpriteDict.TryGetValue(sprite.SpriteGroup, out var names))
                {
                    names = [];
                    SpriteDict[sprite.SpriteGroup] = names;
                }
                names[sprite.SpriteName] = sprite;
            }
        }
        return SpriteDict;
    }
}