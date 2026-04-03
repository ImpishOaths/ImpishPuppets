using Godot;
using Godot.Collections;
using System.Linq;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet3D: Node3D
{
    [Export]
    public PackedScene Puppet2D;
    [Export]
    public Material DefaultMaterial;
    [Export]
    public float ZScale = 0.01f;

    private Texture2D PuppetTexture;
    private TileSet SpriteSheet;
    public Vector2 TextureSize {get; private set;}
    public Vector2 TileSize {get; private set;}
    private TileSetAtlasSource SpriteAtlas;
    private Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> SpriteDict = null;

    public override void _EnterTree()
    {
        if(Puppet2D != null)
        {
            var puppet = Puppet2D.Instantiate<Puppet2D>();
            LoadPuppetSprites(puppet);
        }
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
    
    public PuppetSpriteData GetSpriteReference(StringName group, StringName sprite)
    {
        if(group == null || ! SpriteDict.TryGetValue(group, out var sprites))
            return null;
        if(sprite == null || ! sprites.TryGetValue(sprite, out var ret))
            return null;
        return ret;
    }

    private void LoadPuppetSprites(Puppet2D puppet)
    {
        PuppetTexture = puppet.PuppetTexture;
        SpriteSheet = puppet.SpriteSheet;
        TextureSize = PuppetTexture.GetSize();
        TileSize = SpriteSheet.TileSize;

        InitializeSprites();
    }

    private void BuildPuppet(Puppet2D puppet)
    {
        Node storage = new();
        AddChild(storage);
        foreach(var child in GetChildren())
            if(child != storage)
                child.Reparent(storage);
        storage.QueueFree();
        
        System.Collections.Generic.List<Puppet3DBone> ForwardOrder = [];
        System.Collections.Generic.List<Puppet3DBone> BackwardOrder = [];

        void makeComponent(Node parent, Node node)
        {
            Node newNode;
            switch(node)
            {
                case Puppet2DBone bone:
                    var bone3d = new Puppet3DBone();
                    bone3d.Initialize(this, bone);

                    switch(bone.SortOrder)
                    {
                        case Puppet2DBone.SortOrderEnum.FRONT:
                            ForwardOrder.Add(bone3d);
                            break;
                        case Puppet2DBone.SortOrderEnum.BACK:
                            BackwardOrder.Add(bone3d);
                            break;
                        case Puppet2DBone.SortOrderEnum.BOTH:
                            ForwardOrder.Add(bone3d);
                            BackwardOrder.Add(bone3d);
                            break;
                    }

                    newNode = bone3d;
                    break;
                case Puppet2DControl control:
                    var control3D = new Puppet3DControl();
                    control3D.Initialize(this, control);
                    newNode = control3D;
                    break;
                case PuppetBoneModifier modifier:
                    newNode = modifier.Duplicate();
                    break;
                default:
                    return;
            }

            newNode.Name = node.Name;
            parent.AddChild(newNode, true);
            newNode.Owner = Owner ?? this;

            foreach(var child in node.GetChildren())
                makeComponent(newNode, child);
        }
    }

    [ExportToolButton("Reload Puppet")]
    public Callable ReloadPuppetCallable => Callable.From(ReloadPuppet);
    private void ReloadPuppet()
    {
        var puppet = Puppet2D.Instantiate<Puppet2D>();
        LoadPuppetSprites(puppet);


        System.Collections.Generic.List<Puppet3DBone> ForwardOrder = [];
        System.Collections.Generic.List<Puppet3DBone> BackwardOrder = [];

        void initializeComponent(Node parent, Node node)
        {
            Node newNode;
            switch(node)
            {
                case Puppet2DBone bone:
                    var bone3d = new Puppet3DBone();
                    bone3d.Initialize(this, bone);

                    switch(bone.SortOrder)
                    {
                        case Puppet2DBone.SortOrderEnum.FRONT:
                            ForwardOrder.Add(bone3d);
                            break;
                        case Puppet2DBone.SortOrderEnum.BACK:
                            BackwardOrder.Add(bone3d);
                            break;
                        case Puppet2DBone.SortOrderEnum.BOTH:
                            ForwardOrder.Add(bone3d);
                            BackwardOrder.Add(bone3d);
                            break;
                    }

                    newNode = bone3d;
                    break;
                case Puppet2DControl control:
                    var control3D = new Puppet3DControl();
                    control3D.Initialize(this, control);
                    newNode = control3D;
                    break;
                case PuppetBoneModifier modifier:
                    newNode = modifier.Duplicate();
                    break;
                default:
                    return;
            }

            newNode.Name = node.Name;
            parent.AddChild(newNode, true);
            newNode.Owner = Owner ?? this;

            foreach(var child in node.GetChildren())
                initializeComponent(newNode, child);
        }

        foreach(var child in puppet.GetChildren())
        {
            initializeComponent(this, child);
        }

        int order = 0;
        foreach(var bone in ForwardOrder)
            bone.SetOrder(order++, true);
        BackwardOrder.Reverse();
        order = -1;
        foreach(var bone in BackwardOrder)
            bone.SetOrder(order--, false);
    }

}
