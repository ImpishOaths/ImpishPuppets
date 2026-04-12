using System.Linq;
using Godot;
using Godot.Collections;

namespace ImpishPuppets;

public interface Puppet
{
    public Array<StringName> GetSpriteGroups();
    public Array<StringName> GetSpritesInGroup(StringName group);
    public StringName GetFirstGroup();
    public PuppetSpriteData GetFirstSprite(StringName group);
    public PuppetSpriteData GetSpriteReference(StringName group, StringName sprite);
    public Vector2 GetResize();
}

[Tool]
[GlobalClass]
public partial class Puppet2D: Node2D, Puppet
{
    [Export]
    public Texture2D PuppetTexture {get; private set;}
    public ImageTexture PuppetImageTexture {get; private set;}
    [Export]
    public TileSet SpriteSheet;
    private Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> SpriteDict = null;

    public Transform2D? InverseTransform {get; private set;} = null;
    public override void _PhysicsProcess(double delta)
    {
        InverseTransform = GlobalTransform.AffineInverse();
    }

    public Vector2 GetResize() => Vector2.One;

    public override Variant _Get(StringName property)
    {
        if(property == "texture")
            return PuppetImageTexture;
        if(property == "image_source")
            return PuppetTexture;
        if(property == "grid")
            return ((TileSetAtlasSource)SpriteSheet.GetSource(0)).TextureRegionSize;
        
        return default;
    }

    public override void _EnterTree()
    {
        if(SpriteSheet == null || PuppetTexture == null)
            return;
        SpriteDict ??= SpriteSheet.MakeSpriteDict();
        var image = PuppetTexture.GetImage();
        image.Decompress();
        PuppetImageTexture = ImageTexture.CreateFromImage(image);
    }

    public override void _Notification(int what)
    {
        if(what == NotificationEditorPreSave && SpriteSheet != null && SpriteSheet.GetMeta("dirty", false).As<bool>())
        {
            SpriteSheet.SetMeta("dirty", false);
            var tempSet = ResourceLoader.Load<TileSet>(SpriteSheet.ResourcePath);
            var spriteAtlas = (TileSetAtlasSource)tempSet.GetSource(0);
            foreach(var group in SpriteDict.Values)
            {
                foreach(var sprite in group.Values)
                {
                    var data = spriteAtlas.GetTileData(sprite.SpriteRegion.Position/tempSet.TileSize, sprite.AlternateID);
                    var offset = sprite.SpriteData.GetCustomData("Offset").As<Vector2>();
                    data.SetCustomData("Offset", offset);
                    sprite.SpriteData = data;
                }
            }
            GD.Print("Offsets Updated");
        }

        if(what == NotificationEditorPostSave)
            SaveImage();
    }

    public void SaveImage()
    {
        if(PuppetImageTexture == null || ! PuppetImageTexture.GetMeta("dirty", false).As<bool>())
            return;
        PuppetImageTexture.SetMeta("dirty", false);
        var path = PuppetTexture.ResourcePath;
        GD.Print($"Saved at {path}");
        PuppetImageTexture.GetImage().SavePng(path);
        EditorInterface.Singleton.GetResourceFilesystem().ReimportFiles([path]);
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

    [ExportToolButton("Refresh Sprites")]
    public Callable RefreshSpritesCallable => Callable.From(RefreshSprites);
    private void RefreshSprites()
    {
        SpriteDict = SpriteSheet.MakeSpriteDict();
    }

    [ExportToolButton("Add Bone")]
    public Callable AddBoneCallable => Callable.From(AddBone);
    private void AddBone()
    {
        MakeNewBone(this);
    }

    public void MakeNewBone(Node parent)
    {
        var pBone = new PuppetBone2D()
        {
            Puppet = this,
            UseParentMaterial = true,
        };
        pBone.Initialize();
        parent.AddChild(pBone, true);
        pBone.Owner = Owner ?? this;
    }

    [ExportToolButton("Add Transform")]
    public Callable AddTransformCallable => Callable.From(AddTransform);
    private void AddTransform()
    {
        MakeNewTransform(this);
    }

    public void MakeNewTransform(Node parent)
    {
        var pTransform = new PuppetTransform2D()
        {
            Puppet = this,
            UseParentMaterial = true,
        };
        parent.AddChild(pTransform, true);
        pTransform.Owner = Owner ?? this;
    }
}
