using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
public partial class ClothingBone3D: PuppetBone3D
{
    protected override Puppet TexPuppet => ClothingContainer.Instance;
    protected override Texture2D PuppetTexture => ClothingContainer.Instance.GetTexture(_SpriteGroup, _SpriteName);
    protected override Material PuppetMaterial => ClothingContainer.Instance.ClothingMaterial3D;

    public override void UpdateLook()
    {
        if(Sprite == null)
            return;

        if(CurrentSprite != null)
        {
            var data = CurrentSprite.SpriteData;
            Sprite.FlipH = data.FlipH;
            Sprite.FlipV = data.FlipV;
            Sprite.Offset = data.GetCustomData("Offset").AsVector2() * new Vector2(1,-1);
            Sprite.Texture = PuppetTexture;
            Sprite.SetInstanceShaderParameter("color1", data.GetCustomData("Color1").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("color2", data.GetCustomData("Color2").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("color3", data.GetCustomData("Color3").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("clothingLayer", CurrentSprite.SheetIndex);
            Sprite.RegionRect = CurrentSprite.SpriteRegion;
        }
        else
        {
            Sprite.FlipH = false;
            Sprite.FlipV = false;
            Sprite.Offset = Vector2.Zero;
            Sprite.RegionRect = new Rect2(0,0,0,0);
        }

        Sprite.Rotation = new Vector3(0, 0, -RotationOffset);
    }
}
