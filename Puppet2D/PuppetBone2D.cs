using System;
using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Flags]
public enum SortOrderEnum
{
    FRONT = 1,
    BACK = 2,
}

public interface PuppetBone: PuppetTransform
{
    public void SetSprite(StringName group, StringName name);
    public StringName SpriteGroup {get; set;}
    public StringName SpriteName {get; set;}
    public float RotationOffset {get; set;}
    public void SetVisible(bool visible);
    public void SetOrder(bool front);
    public Transform2D GetRealTransform();
    public void SetRealTransform(Transform2D trans);
}

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Bone2DIcon.png")]
public partial class PuppetBone2D: PuppetTransform2D, PuppetBone, Puppet2Dto3DConverter
{
    protected virtual SpriteSheet SpriteSheet => Puppet?.SpriteSheet;
    protected virtual Texture2D PuppetTexture => Puppet?.PuppetImageTexture;
    protected virtual Material PuppetMaterial => Puppet?.BoneMaterials2D[_MaterialChoice];

    [Export]
    public Vector2 SpriteOffset
    {
        get
        {
            if(Sprite != null)
                return Sprite.Offset;
            return default;
        }
        set
        {
            if(Sprite != null)
                Sprite.Offset = value;
        }
    }

    [ExportToolButton("Commit Offset")]
    public Callable CommitOffsetCallable => Callable.From(CommitOffset);
    public void CommitOffset()
    {
        if(SpriteSheet == null)
            return;
        SpriteSheet.UpdateData(_SpriteGroup, _SpriteName, "Offset", SpriteOffset);
        UpdateLook();
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
    protected float _RotationOffset = 0;
    
    [Export]
    public int MaterialChoice
    {
        get => _MaterialChoice;
        set
        {
            _MaterialChoice = value;
            if(Puppet?.BoneMaterials2D == null || Sprite == null)
                return;
            var choice = Math.Clamp(_MaterialChoice, 0, Puppet.BoneMaterials2D.Count-1);
            Sprite.Material = Puppet.BoneMaterials2D[choice];
        }
    }
    private int _MaterialChoice = 0;

    [ExportGroup("Ordering")]

    [Export]
    public SortOrderEnum SortOrder
    {
        get => _SortOrder;
        set
        {
            _SortOrder = value;
            SetOrder(_Order);
        }
    }
    protected SortOrderEnum _SortOrder = SortOrderEnum.FRONT;

    [Export]
    protected bool Order
    {
        get => _Order;
        set => SetOrder(value);
    }
    private bool _Order;

    public void SetOrder(bool front)
    {
        _Order = front;
        ZIndex = _SortOrder switch
        {
            SortOrderEnum.FRONT => 1,
            SortOrderEnum.BACK => -1,
            SortOrderEnum.FRONT | SortOrderEnum.BACK => Order ? 1 : -1,
            _ => 1,
        };
    }

    [ExportGroup("Sprite Info")]
    [Export]
    protected Sprite2D Sprite;

    [Export]
    public StringName SpriteGroup
    {
        get => _SpriteGroup;
        set
        {
            if(_SpriteGroup == value)
                return;
            _SpriteGroup = value;
            if(SpriteSheet == null)
                return;
            SetSprite(value, SpriteSheet.GetFirstSpriteInGroup(value));
        }
    }
    protected StringName _SpriteGroup = "";

    [Export]
    public StringName SpriteName
    {
        get => _SpriteName;
        set
        {
            if(_SpriteName == value)
                return;
            _SpriteName = value;
            if(SpriteSheet == null)
                return;
            SetSprite(_SpriteGroup, value);
        }
    }
    protected StringName _SpriteName = "";

    protected bool Saving;

    public override Variant _Get(StringName property)
    {
        if(property == "material" && Puppet != null)
            return PuppetMaterial;

        if(property == "texture" && SpriteSheet != null && Sprite != null)
            return Sprite.Texture;

        if(property == "image_source" && SpriteSheet != null)
            return SpriteSheet.GetSpriteTexture(0);
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
        if(property == "SpriteGroupDropdown")
        {
            SpriteGroup = value.AsStringName();
            if(Sprite != null)
                GetTree().CallGroupFlags(6, "DrawingEditor", "TrySelect");
        }
        if(property == "SpriteNameDropdown")
        {
            SpriteName = value.AsStringName();
            if(Sprite != null)
                GetTree().CallGroupFlags(6, "DrawingEditor", "TrySelect");
        }

        return false;
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];
        if(SpriteSheet == null)
            return properties;
            
        properties.Add(new()
        {
            {"name","Sprite Select"},
            {"usage", (int)PropertyUsageFlags.Group}
        });
        var groups = SpriteSheet.GetGroups();
        properties.Add(new(){
            {"name","SpriteGroupDropdown"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", string.Join(',', groups)}
        });
        if(_SpriteGroup != null && _SpriteGroup != "")
        {
            var sprites = SpriteSheet.GetSpritesInGroup(_SpriteGroup);
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
                Sprite.Texture = PuppetTexture;
            UpdateLook();
        }
    }

    public override void _Ready()
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
                    Material = PuppetMaterial,
                    Texture = PuppetTexture,
                    RegionFilterClipEnabled = true,
                    RegionEnabled = true,
                };
                AddChild(Sprite, true, InternalMode.Front);
            }
        }

        SetSprite(_SpriteGroup, _SpriteName);
        SetOrder(Order);
    }

    public void SetSprite(StringName group, StringName name)
    {
        if(Saving || SpriteSheet == null)
            return;
        
        _SpriteGroup = group;
        _SpriteName = name;

        UpdateLook();
        NotifyPropertyListChanged();
    }

    public void UpdateLook()
    {
        if(Sprite == null)
            return;

        var data = SpriteSheet?.GetSpriteData(_SpriteGroup, _SpriteName);
        
        if(data != null)
        {
            Sprite.FlipH = data.FlipH;
            Sprite.FlipV = data.FlipV;
            Sprite.Offset = data.GetCustomData("Offset").AsVector2();
            Sprite.RegionRect = SpriteSheet.GetSprite(_SpriteGroup, _SpriteName).SpriteRegion;
            Sprite.Material = PuppetMaterial;
            SetUpSpriteVariables(data);
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

    public virtual void SetUpSpriteVariables(TileData data) {}

    public (StringName group, StringName name) GetSprite()
    {
        return (_SpriteGroup, _SpriteName);
    }

    public override Transform2D GetLocalTransform() => Sprite.Transform;
    public override void SetLocalTransform(Transform2D transform) => Sprite.Transform = transform;

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        return new PuppetBone3D()
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
            SpriteGroup = _SpriteGroup,
            SpriteName = _SpriteName,
            Flip = _Flip,
        };
    }

    public override void PropogateOrderFlip(bool order, bool flip)
    {
        SetFlip(flip);
        SetOrder(order);
        foreach(var child in GetChildren())
        {
            if(child is PuppetTransform trans)
                trans.PropogateOrderFlip(order, flip);
        }
    }

    public Transform2D GetRealTransform() => Transform;
    public void SetRealTransform(Transform2D trans) => Transform = trans;
}