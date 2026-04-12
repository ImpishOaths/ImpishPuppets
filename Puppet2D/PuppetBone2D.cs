using System;
using Godot;
using Godot.Collections;

namespace ImpishPuppets;

public enum SortOrderEnum
{
    FRONT,
    BACK,
    BOTH,
}

public interface PuppetBone: PuppetTransform
{
    public void SetSprite(StringName group, StringName name);
    public StringName SpriteGroup {get; set;}
    public StringName SpriteName {get; set;}
    public float RotationOffset {get; set;}
    public void SetVisible(bool visible);
    public void SetOrder(bool front);
}

[Tool]
[GlobalClass]
public partial class PuppetBone2D: PuppetTransform2D, PuppetBone
{
    private PuppetSpriteData CurrentSprite = null;
    private Sprite2D Sprite;

    [Export]
    public Vector2 SpriteOffset
    {
        get
        {
            if(CurrentSprite != null)
            {
                if(CurrentSprite.SpriteData.HasCustomData("Offset"))
                    return CurrentSprite.SpriteData.GetCustomData("Offset").AsVector2();
                return default;
            }
            return default;
        }
        set
        {
            if(CurrentSprite != null)
            {
                if(CurrentSprite.SpriteData.HasCustomData("Offset") && Puppet.Owner == null)
                {
                    CurrentSprite.SpriteData.SetCustomData("Offset", value);
                    Puppet.SpriteSheet.SetMeta("dirty", true);
                }
                UpdateLook();
            }
        }
    }

    [Export]
    public SortOrderEnum SortOrder
    {
        get => _SortOrder;
        set
        {
            _SortOrder = value;
            SetOrder(true);
        }
    }
    private SortOrderEnum _SortOrder;

    public void SetOrder(bool front)
    {
        ZIndex = _SortOrder switch
        {
            SortOrderEnum.FRONT => 1,
            SortOrderEnum.BACK => -1,
            SortOrderEnum.BOTH => front ? 1 : -1,
            _ => 1,
        };
    }

    [Export]
    public float RotationOffset
    {
        get => _RotationOffset;
        set
        {
            _RotationOffset = value;
            UpdateLook();
        }
    }
    private float _RotationOffset = 0;

    [ExportGroup("Sprite Info")]
    [Export]
    public StringName SpriteGroup
    {
        get => _SpriteGroup;
        set
        {
            bool check = _SpriteGroup != value;
            _SpriteGroup = value;
            if(Sprite == null || Puppet == null)
                return;
            if(check)
                SetGroup(_SpriteGroup);
            CallDeferred("TrySelect");
        }
    }
    private StringName _SpriteGroup;

    [Export]
    public StringName SpriteName
    {
        get => _SpriteName;
        set
        {
            bool check = _SpriteName != value;
            _SpriteName = value;
            if(Sprite == null || Puppet == null)
                return;
            if(check)
                SetSprite(_SpriteGroup, _SpriteName);
            CallDeferred("TrySelect");
        }
    }
    private StringName _SpriteName;

    private bool Saving;

    private void TrySelect()
    {
        GetTree().CallGroupFlags(6, "DrawingEditor", "TrySelect");
    }

    public override Variant _Get(StringName property)
    {
        if(property == "material")
            return Puppet.Material;

        if(property == "texture" && Sprite != null)
            return Sprite.Texture;

        if(property == "image_source" && Puppet != null)
            return Puppet.PuppetTexture;
        if(property == "region_rect" && Sprite != null)
            return Sprite.RegionRect;
        if(property == "region_enabled" && Sprite != null)
            return Sprite.RegionEnabled;
        
        if(property == "SpriteGroupDropdown")
            return SpriteGroup;
        if(property == "SpriteNameDropdown")
            return SpriteName;

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if(Puppet == null)
            return false;

        if(property == "SpriteGroupDropdown")
            SpriteGroup = value.AsStringName();
        if(property == "SpriteNameDropdown")
            SpriteName = value.AsStringName();

        return false;
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];
        if(Puppet == null)
            return properties;
            
        properties.Add(new()
        {
            {"name","Sprite Select"},
            {"usage", (int)PropertyUsageFlags.Group}
        });
        var groups = Puppet.GetSpriteGroups();
        properties.Add(new(){
            {"name","SpriteGroupDropdown"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", string.Join(',', groups)}
        });
        if(_SpriteGroup != null && _SpriteGroup != "")
        {
            var sprites = Puppet.GetSpritesInGroup(_SpriteGroup);
            properties.Add(new(){
                {"name","SpriteNameDropdown"},
                {"type", (int)Variant.Type.StringName},
                {"hint", (int)PropertyHint.Enum},
                {"hint_string", string.Join(',', sprites)}
            });
        }
        return properties;
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
                Sprite.Texture = Puppet.PuppetImageTexture;
            UpdateLook();
        }
    }

    public override void _EnterTree()
    {
        Initialize();
    }

    public void Initialize()
    {
        if(Puppet == null)
        {
            Node current = GetParent();
            while(current != null)
            {
                if(current is Puppet2D puppet)
                {
                    Puppet = puppet;
                    break;
                }
                current = current.GetParent();
            }
        }

        if(Sprite == null)
        {
            Sprite = GetNodeOrNull<Sprite2D>("Sprite");
            if(Sprite == null)
            {
                Sprite = new Sprite2D
                {
                    Name = "Sprite",
                    UseParentMaterial = true,
                    Texture = Puppet.PuppetImageTexture,
                    RegionFilterClipEnabled = true,
                    RegionEnabled = true,
                };
                AddChild(Sprite, true, InternalMode.Front);
            }
        }

        if(SpriteGroup != "")
            SetSprite(SpriteGroup, SpriteName);
        else
            SetSprite(null);
    }

    private void SetGroup(StringName group)
    {
        SetSprite(Puppet.GetFirstSprite(group));
    }

    public void SetSprite(PuppetSpriteData sprite)
    {
        if(Saving)
            return;

        CurrentSprite = sprite;
        if(CurrentSprite != null)
        {
            _SpriteGroup = CurrentSprite.SpriteGroup;
            _SpriteName = CurrentSprite.SpriteName;
        }
        else
        {
            _SpriteGroup = "";
            _SpriteName = "";
        }

        UpdateLook();
        NotifyPropertyListChanged();
    }

    private void SetEditor(Puppet2D editor)
    {
        Puppet = editor;
    }

    public void UpdateLook()
    {
        if(Sprite == null)
            return;
        
        if(CurrentSprite != null)
        {
            Sprite.FlipH = CurrentSprite.SpriteData.FlipH;
            Sprite.FlipV = CurrentSprite.SpriteData.FlipV;
            Sprite.Offset = CurrentSprite.SpriteData.GetCustomData("Offset").AsVector2();
            Sprite.RegionRect = CurrentSprite.SpriteRegion;
        }
        else
        {
            Sprite.FlipH = false;
            Sprite.FlipV = false;
            Sprite.Offset = Vector2.Zero;
            Sprite.RegionRect = new Rect2(0,0,0,0);
        }
        SetOrder(true);
        Sprite.Rotation = _RotationOffset;
    }

    public (StringName group, StringName name) GetSprite()
    {
        return (_SpriteGroup, _SpriteName);
    }

    public void SetSprite(StringName group, StringName name)
    {
        if(Puppet != null)
            SetSprite(Puppet.GetSpriteReference(group, name));
        else
            SetSprite(null);
    }
}