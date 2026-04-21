using Godot;
using GCollections = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet3D: Node3D, Puppet
{
    [Export]
    public PackedScene Puppet2D;
    [Export]
    public ShaderMaterial PuppetMaterial {get; private set;}
    public ShaderMaterial PuppetMaterialFlat {get; private set;}

    public Texture2D PuppetTexture {get; private set;}
    private TileSet SpriteSheet;
    public Vector2 TextureSize {get; private set;}
    private GCollections.Dictionary<StringName, GCollections.Dictionary<StringName, PuppetSpriteData>> SpriteDict = null;
    
    public Node GetNode() => this;

    public Transform3D? InverseTransform {get; private set;} = null;
    public override void _PhysicsProcess(double delta)
    {
        InverseTransform = GlobalTransform.AffineInverse();
    }

    public override void _EnterTree()
    {
        PuppetMaterialFlat = PuppetMaterial.Duplicate() as ShaderMaterial;
        PuppetMaterialFlat.SetShaderParameter("flatLighting", true);
        
        if(Puppet2D == null)
            return;
        
        var puppet = Puppet2D.Instantiate<Puppet2D>();
        LoadPuppetSprites(puppet);
        puppet.QueueFree();
    }

    public GCollections.Array<StringName> GetSpriteGroups() => [..SpriteDict.Keys];
    public GCollections.Array<StringName> GetSpritesInGroup(StringName group)
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
        //TileSize = SpriteSheet.TileSize;
        SpriteDict = SpriteSheet.MakeSpriteDict();
    }

    //public Vector2 GetResize() => new(1f/TileSize.X, 1f/TileSize.Y);

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

        List<Node> bone3ds = [];
        List<PuppetController> controllers = [];
        List<(int index, NodePath toSwap)> swaps = [];
        int frontCount = 0;
        int backwardCount = 0;

        void initializeComponent(Node parent, Node node)
        {
            Node newNode;
            switch(node)
            {
                case DepthMarker mark:
                    newNode = new Node();
                    swaps.Add((bone3ds.Count, mark.Marked));
                    bone3ds.Add(newNode);
                    break;
                case PuppetBone2D bone2D:
                    PuppetBone3D bone3d;
                    if(bone2D is ClothingBone2D)
                        bone3d = new ClothingBone3D();
                    else
                        bone3d = new PuppetBone3D();
                    bone3d.Initialize(this, bone2D);
                    bone3ds.Add(bone3d);

                    if(bone3d.SortOrder == SortOrderEnum.BOTH || bone3d.SortOrder == SortOrderEnum.BACK)
                        backwardCount++;
                    if(bone3d.SortOrder == SortOrderEnum.BOTH || bone3d.SortOrder == SortOrderEnum.FRONT)
                        frontCount++;

                    newNode = bone3d;
                    break;
                case PuppetTransform2D trans2D:
                    var trans3D = new PuppetTransform3D();
                    trans3D.Initialize(this, trans2D);
                    newNode = trans3D;
                    break;
                case PuppetBoneModifier modifier:
                    newNode = modifier.MakeDuplicate3D(Vector2.One/VectorHelpers.PixelSizeRoot);
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

        foreach(var child in puppet.GetChildren())
            initializeComponent(this, child);

        foreach(var (index, toSwap) in swaps)
        {
            var node = bone3ds[index];
            var bone = node.GetNode(toSwap);
            var boneIndex = bone3ds.FindIndex(bone.Equals);
            bone3ds[boneIndex] = node;
            bone3ds[index] = bone;
            node.QueueFree();
        }

        int forward = 0;
        int clothing = 0;
        int backward = backwardCount+1;
        foreach(var node in bone3ds)
        {
            if(node is not PuppetBone3D bone)
                continue;
            
            var order = Vector2I.Zero;
            if(bone.SortOrder == SortOrderEnum.FRONT || bone.SortOrder == SortOrderEnum.BOTH)
                order.X = forward++;
            if(bone.SortOrder == SortOrderEnum.BACK || bone.SortOrder == SortOrderEnum.BOTH)
                order.Y = -backward--;
            if(bone.SortOrder == SortOrderEnum.CLOTHING)
                order.X = frontCount + clothing++;
            bone.SetOrderValues(order);
        }

        foreach(var control in controllers)
            control.Initialize();
        
        puppet.QueueFree();
    }

    public void MakeTilesDirty() {}
}
