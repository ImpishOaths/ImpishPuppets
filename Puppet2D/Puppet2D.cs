using System.Linq;
using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet2D: Node2D
{
    [Export]
    public Texture2D PuppetTexture {get; private set;}
    public ImageTexture PuppetImageTexture {get; private set;}
    [Export]
    public TileSet SpriteSheet;
    private TileSetAtlasSource SpriteAtlas;
    private Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> SpriteDict = null;

    public override Variant _Get(StringName property)
    {
        if(property == "texture")
            return PuppetImageTexture;
        if(property == "image_source")
            return PuppetTexture;
        if(property == "grid")
            return SpriteAtlas.TextureRegionSize;
        
        return default;
    }

    public override void _EnterTree()
    {
        if(SpriteDict == null)
            InitializeSprites();
    }

    public override void _Ready()
    {
        GetTree().CallGroup("EditorBone", "Initialize");
    }

    private void AddSprite(PuppetSpriteData reference)
    {
        if(! SpriteDict.TryGetValue(reference.SpriteGroup, out var names))
        {
            names = [];
            SpriteDict[reference.SpriteGroup] = names;
        }
        names[reference.SpriteName] = reference;
    }

    private void InitializeSprites()
    {
        SpriteDict ??= [];
        SpriteDict.Clear();

        if(SpriteSheet == null || PuppetTexture == null)
            return;

        SpriteAtlas = (TileSetAtlasSource)SpriteSheet.GetSource(0);
        var image = PuppetTexture.GetImage();
        image.Decompress();
        PuppetImageTexture = ImageTexture.CreateFromImage(image);
        for(int i = 0; i < SpriteAtlas.GetTilesCount(); ++i)
        {
            Vector2I pos = SpriteAtlas.GetTileId(i);
            for(int j = 0; j < SpriteAtlas.GetAlternativeTilesCount(pos); ++j)
            {
                var altId = SpriteAtlas.GetAlternativeTileId(pos, j);
                var spriteData = SpriteAtlas.GetTileData(pos, altId);
                PuppetSpriteData sprite = new()
                {
                    SpriteGroup = (StringName)spriteData.GetCustomData("Group"),
                    SpriteName = (StringName)spriteData.GetCustomData("Name"),
                    SpriteRegion = SpriteAtlas.GetTileTextureRegion(pos),
                    SpriteData = spriteData
                };
                AddSprite(sprite);
            }
        }
    }

    public override void _Notification(int what)
    {
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
        InitializeSprites();
        GetTree().CallGroup("EditorBone", "ReselectSprite", this);
    }

    [ExportToolButton("Add Bone")]
    public Callable AddBoneCallable => Callable.From(AddBone);
    private void AddBone()
    {
        MakeNewBone(this);
    }

    public void MakeNewBone(Node parent)
    {
        var pBone = new Puppet2DBone()
        {
            Puppet = this,
            UseParentMaterial = true,
        };
        pBone.FindSprite();
        pBone.SetSprite(null);
        parent.AddChild(pBone, true);
        pBone.Owner = Owner ?? this;
    }

    [ExportToolButton("Add Control")]
    public Callable AddControlCallable => Callable.From(AddControl);
    private void AddControl()
    {
        MakeNewControl(this);
    }

    public void MakeNewControl(Node parent)
    {
        var pControl = new Puppet2DControl()
        {
            Puppet = this,
            UseParentMaterial = true,
        };
        parent.AddChild(pControl, true);
        pControl.Owner = Owner ?? this;
    }
}
