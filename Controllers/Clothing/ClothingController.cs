using Godot;
using System;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class ClothingController: Node, Puppet2Dto3DConverter
{
    [Export]
    public Dictionary<StringName, NodeList> NodeMap;
    [Export]
    public bool Mode3D = false;

    [ExportGroup("Testing")]
    [Export]
    private StringName TestBodyPart;
    [Export]
    private ClothingObject TestClothing;
    [ExportToolButton("Test Clothing")]
    public Callable TestClothingCallable => Callable.From(()=>AttachClothing(TestBodyPart, TestClothing));
    [ExportToolButton("Clear Clothing")]
    public Callable ClearClothingCallable => Callable.From(()=>AttachClothing(TestBodyPart, null));

    public void AttachClothing(StringName bodyPart, ClothingObject clothes)
    {
        if(NodeMap.TryGetValue(bodyPart, out var nodeList) == false)
            return;
        var parent = GetParent();
        foreach(var path in nodeList.Nodes)
        {
            if(Mode3D)
            {
                var bone = parent.GetNodeOrNull<CustomPuppetBone3D>(path);
                if(bone == null)
                    continue;
                GD.Print("hi");
                if(clothes != null)
                    Attach3D(bone, clothes);
                else
                    Clear3D(bone);
            }
            else
            {
                var bone = parent.GetNodeOrNull<CustomPuppetBone2D>(path);
                if(bone == null)
                    continue;
                if(clothes != null)
                    Attach2D(bone, clothes);
                else
                    Clear2D(bone);
            }
        }
    }

    private void Attach2D(CustomPuppetBone2D bone, ClothingObject clothing)
    {
        bone.Sheet = clothing.Sheet;
        bone.Material2D = clothing.Material2D;
        bone.Material3D = clothing.Material3D;
        bone.UseCustomMaterial = clothing.UseCustomMaterial;
        bone.RefreshSpriteTexture();
        bone.UpdateLook();
        bone.Visible = true;
    }

    private void Clear2D(CustomPuppetBone2D bone)
    {
        bone.Sheet = null;
        bone.Material2D = null;
        bone.Material3D = null;
        bone.UseCustomMaterial = false;
        bone.UpdateLook();
        bone.Visible = false;
    }

    private void Attach3D(CustomPuppetBone3D bone, ClothingObject clothing)
    {
        bone.Sheet = clothing.Sheet;
        bone.Material3D = clothing.Material3D;
        bone.UseCustomMaterial = clothing.UseCustomMaterial;
        bone.RefreshSpriteTexture();
        bone.UpdateLook();
        bone.Visible = true;
    }

    private void Clear3D(CustomPuppetBone3D bone)
    {
        bone.Sheet = null;
        bone.Material3D = null;
        bone.UseCustomMaterial = false;
        bone.UpdateLook();
        bone.Visible = false;
    }

    public Node ConvertTo3D(Puppet3D puppet)
    {
        return new ClothingController()
        {
            NodeMap = NodeMap,
            Mode3D = true,
            TestBodyPart = "",
            TestClothing = null,
        };
    }
}
