using Godot;
using Godot.Collections;
using System.Linq;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet3D: Node3D, Puppet
{
    [Export]
    public PackedScene Puppet2D;
    [Export]
    public Material DefaultMaterial;

    private Texture2D PuppetTexture;
    private TileSet SpriteSheet;
    public Vector2 TextureSize {get; private set;}
    public Vector2 TileSize {get; private set;}
    private Dictionary<StringName, Dictionary<StringName, PuppetSpriteData>> SpriteDict = null;
    
    public Transform3D? InverseTransform {get; private set;} = null;
    public override void _PhysicsProcess(double delta)
    {
        InverseTransform = GlobalTransform.AffineInverse();
    }

    public override void _EnterTree()
    {
        if(Puppet2D == null)
            return;
        
        var puppet = Puppet2D.Instantiate<Puppet2D>();
        LoadPuppetSprites(puppet);
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
        SpriteDict = SpriteSheet.MakeSpriteDict();
    }

    public Vector2 GetResize() => new(1f/TileSize.X, 1f/TileSize.Y);

    [ExportToolButton("Reload Puppet")]
    public Callable ReloadPuppetCallable => Callable.From(ReloadPuppet);
    private void ReloadPuppet()
    {
        var puppet = Puppet2D.Instantiate<Puppet2D>();
        LoadPuppetSprites(puppet);
        
        Node storage = new();
        AddChild(storage);
        foreach(var child in GetChildren())
            if(child != storage)
                child.Reparent(storage);
        storage.QueueFree();

        System.Collections.Generic.List<PuppetBone3D> bone3ds = [];
        System.Collections.Generic.List<PuppetController> controllers = [];
        int backwardCount = 0;

        foreach(var child in puppet.GetChildren())
            initializeComponent(this, child);

        void initializeComponent(Node parent, Node node)
        {
            Node newNode;
            switch(node)
            {
                case PuppetBone2D bone2D:
                    var bone3d = new PuppetBone3D();
                    bone3d.Initialize(this, bone2D);
                    bone3ds.Add(bone3d);
                    bone3d.Visible = bone2D.Visible;
                    if(bone3d.SortOrder != SortOrderEnum.FRONT)
                        backwardCount++;

                    newNode = bone3d;
                    break;
                case PuppetTransform2D trans2D:
                    var trans3D = new PuppetTransform3D();
                    trans3D.Initialize(this, trans2D);
                    newNode = trans3D;
                    break;
                case PuppetBoneModifier modifier:
                    newNode = modifier.MakeDuplicate3D(GetResize());
                    break;
                case PuppetController controller:
                    var controller3D = controller.MakeDuplicate3D();
                    newNode = controller3D;
                    controllers.Add(controller3D);
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

        int forward = 0;
        int backward = backwardCount+1;
        foreach(var bone in bone3ds)
        {
            var order = Vector2I.Zero;
            if(bone.SortOrder == SortOrderEnum.FRONT || bone.SortOrder == SortOrderEnum.BOTH)
                order.X = forward++;
            if(bone.SortOrder == SortOrderEnum.BACK || bone.SortOrder == SortOrderEnum.BOTH)
                order.Y = -backward--;
            bone.SetOrderValues(order);
        }

        foreach(var control in controllers)
            control.Initialize();
    }
}
