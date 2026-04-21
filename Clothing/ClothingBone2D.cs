using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
public partial class ClothingBone2D: PuppetBone2D
{
    protected override Puppet TexPuppet => ClothingContainer.Instance;
    protected override Texture2D PuppetTexture => ClothingContainer.Instance.GetImageTexture(_SpriteGroup, _SpriteName);
    protected override Material PuppetMaterial => ClothingContainer.Instance.ClothingMaterial2D;

    [Export]
    public Color Color1
    {
        get
        {
            if(CurrentSprite?.SpriteData == null)
                return Colors.Black;
            return CurrentSprite.SpriteData.GetCustomData("Color1").AsColor();
        }
        set
        {
            if(CurrentSprite?.SpriteData == null)
                return;
            CurrentSprite.SpriteData.SetCustomData("Color1", value);
            UpdateLook();
        }
    }
    [Export]
    public Color Color2
    {
        get
        {
            if(CurrentSprite?.SpriteData == null)
                return Colors.Black;
            return CurrentSprite.SpriteData.GetCustomData("Color2").AsColor();
        }
        set
        {
            if(CurrentSprite?.SpriteData == null)
                return;
            CurrentSprite.SpriteData.SetCustomData("Color2", value);
            UpdateLook();
        }
    }
    [Export]
    public Color Color3
    {
        get
        {
            if(CurrentSprite?.SpriteData == null)
                return Colors.Black;
            return CurrentSprite.SpriteData.GetCustomData("Color3").AsColor();
        }
        set
        {
            if(CurrentSprite?.SpriteData == null)
                return;
            CurrentSprite.SpriteData.SetCustomData("Color3", value);
            UpdateLook();
        }
    }

    public override Variant _Get(StringName property)
    {
        if(property == "image_source")
            return ClothingContainer.Instance.GetTexture(_SpriteGroup, _SpriteName);
        if(property == "instance_parameters" && CurrentSprite != null)
        {
            return new Dictionary<StringName, Variant>()
            {
                {"color1", Color1.ToAlbedo()},
                {"color2", Color2.ToAlbedo()},
                {"color3", Color3.ToAlbedo()},
            };
        }
        return base._Get(property);
    }

    public override void _Notification(int what)
    {
        if(what == NotificationEditorPreSave)
        {
            Saving = true;
            if(Sprite != null)
                Sprite.Texture = null;
        }

        if(what == NotificationEditorPostSave)
        {
            Saving = false;
            if(Sprite != null)
                Sprite.Texture = PuppetTexture;
            UpdateLook();
        }
    }

    public override void UpdateLook()
    {
        if(Sprite == null)
            return;
        
        if(CurrentSprite != null)
        {
            var data = CurrentSprite.SpriteData;
            Sprite.FlipH = data.FlipH;
            Sprite.FlipV = data.FlipV;
            Sprite.Offset = data.GetCustomData("Offset").AsVector2();
            Sprite.SetInstanceShaderParameter("color1", data.GetCustomData("Color1").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("color2", data.GetCustomData("Color2").AsColor().ToAlbedo());
            Sprite.SetInstanceShaderParameter("color3", data.GetCustomData("Color3").AsColor().ToAlbedo());
            Sprite.RegionRect = CurrentSprite.SpriteRegion;
            Sprite.Texture = PuppetTexture;
        }
        else
        {
            Sprite.FlipH = false;
            Sprite.FlipV = false;
            Sprite.Offset = Vector2.Zero;
            Sprite.RegionRect = new Rect2(0,0,0,0);
        }
        Sprite.Rotation = _RotationOffset;
    }
}
