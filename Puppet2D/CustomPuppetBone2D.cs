using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Bone2DIcon.png")]
public partial class CustomPuppetBone2D: PuppetBone2D, Puppet2Dto3DConverter
{
    [Export]
    public SpriteSheet Sheet;
    [Export]
    public Material Material2D
    {
        get => _Material2D;
        set
        {
            _Material2D = value;
            if(Sprite != null)
                Sprite.Material = _Material2D;
        }
    }
    private Material _Material2D;

    [Export]
    public Material Material3D;

    [ExportGroup("Custom Material")]
    [Export]
    public bool UseCustomMaterial = false;

    private Variant GetSpriteData(string dataName)
    {
        if(!UseCustomMaterial)
            return default;
        TileData data = SpriteSheet?.GetSpriteData(_SpriteGroup, _SpriteName);
        if(data == null)
            return default;
        return data.GetCustomData(dataName);
    }
    private void SetSpriteData(string colorName, Variant value)
    {
        if(SpriteSheet != null && UseCustomMaterial)
        {
            SpriteSheet.UpdateData(_SpriteGroup, _SpriteName, colorName, value);
            UpdateLook();
        }
    }
    [Export]
    public Color Color1
    {
        get => GetSpriteData("Color1").AsColor();
        set => SetSpriteData("Color1", value);
    }
    [Export]
    public Color Color2
    {
        get => GetSpriteData("Color2").AsColor();
        set => SetSpriteData("Color2", value);
    }
    [Export]
    public Color Color3
    {
        get => GetSpriteData("Color3").AsColor();
        set => SetSpriteData("Color3", value);
    }
    [Export]
    public bool Shaded
    {
        get => GetSpriteData("Shaded").AsBool();
        set => SetSpriteData("Shaded", value);
    }

    protected override SpriteSheet SpriteSheet => Sheet ?? null;
    protected override Texture2D PuppetTexture => Sheet?.GetSpriteTexture(0);
    protected override Material PuppetMaterial => _Material2D;

    public override Variant _Get(StringName property)
    {
        if(property == "instance_parameters" && UseCustomMaterial)
        {
            return new Godot.Collections.Dictionary()
            {
                {"color1", Sprite.GetInstanceShaderParameter("color1")},
                {"color2", Sprite.GetInstanceShaderParameter("color2")},
                {"color3", Sprite.GetInstanceShaderParameter("color3")},
                {"shaded", Sprite.GetInstanceShaderParameter("shaded")}
            };
        }

        return base._Get(property);
    }

    public void RefreshSpriteTexture()
    {
        if(Sprite == null)
            return;
        Sprite.Texture = PuppetTexture;
        Sprite.Material = PuppetMaterial;
    }

    public override void SetUpSpriteVariables(TileData data)
    {
        if(UseCustomMaterial)
        {
            Sprite.SetInstanceShaderParameter("color1", data.GetCustomData("Color1").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("color2", data.GetCustomData("Color2").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("color3", data.GetCustomData("Color3").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("shaded", data.GetCustomData("Shaded"));
        }
    }

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        return new CustomPuppetBone3D()
        {
            Scale = Scale.Abs().ToVec3scale(),
            Position = (Position / (VectorHelpers.PixelSizeRoot * VectorHelpers.PixelSizeRoot)).ToVec3pos(),
            Rotation = new Vector3(0, 0, -Rotation),
            Puppet = puppet,
            SortOrder = SortOrder,
            Order = Order,
            Visible = Visible,
            RotationOffset = RotationOffset,
            MaterialChoice = MaterialChoice,
            Sheet = Sheet,
            Material3D = Material3D,
            SpriteGroup = _SpriteGroup,
            SpriteName = _SpriteName,
            Flip = _Flip,
            UseCustomMaterial = UseCustomMaterial
        };
    }
}
