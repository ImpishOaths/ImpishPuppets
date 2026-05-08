using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Puppet3DIcon.png")]
public partial class Puppet3D: PuppetTransform3D, Puppet
{
    public Puppet3D()
    {
        Puppet = this;
    }
    
    [Export]
    public PackedScene Puppet2D;
    [Export]
    public CharacterData CharacterData;
    [Export]
    public Godot.Collections.Array<Material> BoneMaterials3D;
    [Export]
    public SpriteSheet SpriteSheet {get; private set;}
    public SpriteSheet GetSheet() => SpriteSheet;
    
    public Transform3D? InverseTransform {get; private set;} = null;
    public override void _PhysicsProcess(double delta)
    {
        InverseTransform = GlobalTransform.AffineInverse();
    }

    [ExportToolButton("Reload Puppet")]
    public Callable ReloadPuppetCallable => Callable.From(ReloadPuppet);
    private void ReloadPuppet()
    {
        var puppet2D = Puppet2D.Instantiate<Puppet2D>();
        SpriteSheet = puppet2D.SpriteSheet;
        BoneMaterials3D = puppet2D.BoneMaterials3D;
        CharacterData = puppet2D.CharacterData;
        
        Node storage = new();
        AddChild(storage);
        foreach(var child in GetChildren())
            if(child != storage)
                child.Reparent(storage);
        storage.QueueFree();

        List<Node> boneOrder = [];

        void duplicateStructure(Node parent3D, Node child2D)
        {
            if(child2D is DepthSwapper)
                boneOrder.Add(child2D);
            if(child2D is not Puppet2Dto3DConverter converter)
                return;
            Node child3D = converter.ConvertTo3D(this);
            child3D.Name = child2D.Name;
            parent3D.AddChild(child3D);
            child3D.Owner = Owner ?? this;
            if(child3D is PuppetBone3D)
                boneOrder.Add(child3D);

            foreach(var grandchild2D in child2D.GetChildren())
                duplicateStructure(child3D, grandchild2D);
        }
        foreach(var child2D in puppet2D.GetChildren())
            duplicateStructure(this, child2D);
        
        int backwardTotal = 0;
        for(var i = 0; i < boneOrder.Count; ++i)
        {
            if(boneOrder[i] is DepthSwapper swapper)
            {
                Node toSwap = GetNodeOrNull(swapper.SwapPath);
                if(toSwap == null)
                    continue;
                var index = boneOrder.IndexOf(toSwap);
                if(index == -1)
                    continue;
                boneOrder[index] = swapper;
                boneOrder[i] = toSwap;
            }
            else if(boneOrder[i] is PuppetBone3D bone && bone.SortOrder.HasFlag(SortOrderEnum.BACK))
                backwardTotal += 1;
        }

        int forward = 0;
        int backward = -backwardTotal;
        foreach(var node in boneOrder)
        {
            if(node is not PuppetBone3D bone)
                continue;
            if(bone.SortOrder.HasFlag(SortOrderEnum.FRONT))
                bone.FrontBackOrder.X = forward++;
            if(bone.SortOrder.HasFlag(SortOrderEnum.BACK))
                bone.FrontBackOrder.Y = backward++;
            bone.SetOrder(bone.Order);
        }

        puppet2D.QueueFree();
    }
}
