using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Bone3DIcon.png")]
public partial class CustomPuppetBone3D: PuppetBone3D
{
    [Export]
    public SpriteSheet Sheet;
    [Export]
    public Material Material3D;
    [Export]
    public bool UseCustomMaterial;

    protected override SpriteSheet SpriteSheet => Sheet ?? null;
    protected override Texture2D PuppetTexture => Sheet?.GetSpriteTexture(0);
    protected override Material PuppetMaterial => Material3D;

    public void RefreshSpriteTexture()
    {
        if(Sprite == null)
            return;
        Sprite.Texture = PuppetTexture;
        Sprite.MaterialOverride = PuppetMaterial;
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
}
