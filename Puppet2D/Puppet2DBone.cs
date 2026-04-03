using System;
using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet2DBone: Puppet2DControl
{
    private PuppetSpriteData CurrentSprite = null;
    
    private Sprite2D Sprite;

    [Export]
    private Vector2 Offset
    {
        get => Get("offset").AsVector2();
        set => Set("offset", value);
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

    public enum SortOrderEnum
    {
        FRONT,
        BACK,
        BOTH
    }

    [Export]
    public SortOrderEnum SortOrder
    {
        get => _SortOrder;
        set
        {
            _SortOrder = value;
            ZIndex = _SortOrder == SortOrderEnum.BACK ? -1 : 1;
        }
    }
    private SortOrderEnum _SortOrder;

    [ExportGroup("Sprite Info")]
    [Export]
    private StringName SpriteGroup
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
    private StringName SpriteName
    {
        get => _SpriteName;
        set
        {
            bool check = _SpriteName != value;
            _SpriteName = value;
            if(Sprite == null || Puppet == null)
                return;
            if(check)
                SetSprite(Puppet.GetSpriteReference(_SpriteGroup, _SpriteName));
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
        
        if(property == "offset" && CurrentSprite != null)
        {
            if(CurrentSprite.SpriteData.HasCustomData("Offset"))
                return CurrentSprite.SpriteData.GetCustomData("Offset");
            return default;
        }

        if(property == "texture" && Sprite != null)
            return Sprite.Texture;

        if(property == "image_source" && Puppet != null)
            return Puppet.PuppetTexture;
        if(property == "region_rect" && Sprite != null)
            return Sprite.RegionRect;
        if(property == "region_enabled" && Sprite != null)
            return Sprite.RegionEnabled;
        
        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if(Puppet == null)
            return false;

        if(property == "offset" && CurrentSprite != null)
        {
            Vector2 vec = value.AsVector2();
            if(CurrentSprite.SpriteData.HasCustomData("Offset"))
                CurrentSprite.SpriteData.SetCustomData("Offset", vec);
            UpdateLook();
            return true;
        }

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
            {"name","SpriteGroup"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", string.Join(',', groups)}
        });
        if(_SpriteGroup != null && _SpriteGroup != "")
        {
            var sprites = Puppet.GetSpritesInGroup(_SpriteGroup);
            properties.Add(new(){
                {"name","SpriteName"},
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
        }
    }

    public void FindEditorParent()
    {
        if(Puppet == null)
        {
            Node current = GetParent();
            while(current != null)
            {
                if(current is Puppet2D editor)
                {
                    Puppet = editor;
                    break;
                }
                current = current.GetParent();
            }
        }
    }

    public void FindSprite()
    {
        if(Sprite == null)
        {
            Sprite = GetNodeOrNull<Sprite2D>("Sprite");
            if(Sprite == null)
            {
                Sprite = new Sprite2D
                {
                    Name = "Sprite",
                    UseParentMaterial = true,
                    Texture = Puppet.PuppetImageTexture
                };
                AddChild(Sprite, true, InternalMode.Front);
            }
        }
    }

    public override void _EnterTree()
    {
        Initialize();
    }

    public void Initialize()
    {
        FindEditorParent();
        FindSprite();
        ReselectSprite();
    }

    private void ReselectSprite()
    {
        if(Puppet == null)
            return;
        
        if(SpriteGroup != "")
            SetSprite(SpriteGroup, SpriteName);
        else
            SetSprite(null);
    }

    private void SetGroup(StringName group)
    {
        if(Puppet == null)
            return;
        
        SetSprite(Puppet.GetFirstSprite(group));
    }

    public void SetSprite(PuppetSpriteData sprite)
    {
        if(Puppet == null || Saving)
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

    public override void _Ready()
    {
        AddToGroup("EditorBone", true);
        Sprite.RegionFilterClipEnabled = true;
        Sprite.RegionEnabled = true;
    }

    private void SetEditor(Puppet2D editor)
    {
        Puppet = editor;
    }

    public override void UpdateLook()
    {
        if(Sprite == null)
            return;
        
        if(CurrentSprite != null)
        {
            Sprite.FlipH = CurrentSprite.SpriteData.FlipH ^ FlipH;
            Sprite.FlipV = CurrentSprite.SpriteData.FlipV ^ FlipV;
            Sprite.Offset = CurrentSprite.SpriteData.GetCustomData("Offset").AsVector2() * new Vector2(Sprite.FlipH?-1:1, Sprite.FlipV?-1:1);
            Sprite.RegionRect = CurrentSprite.SpriteRegion;
        }
        else
        {
            Sprite.FlipH = FlipH;
            Sprite.FlipV = FlipV;
            Sprite.Offset = Vector2.Zero;
            Sprite.RegionRect = new Rect2(0,0,0,0);
        }
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

    public override void SetLocalPosition(Vector2 pos)
    {
        Sprite.Position = pos;
    }

    public override void SetLocalRotation(float angle)
    {
        Sprite.Rotation = angle;
    }
}