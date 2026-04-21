using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetBone3D: PuppetTransform3D, PuppetBone
{
    protected virtual Puppet TexPuppet => Puppet;
    protected virtual Texture2D PuppetTexture => Puppet.PuppetTexture;
    protected virtual Material PuppetMaterial
    {
        get
        {
            if(CurrentSprite?.SpriteData?.GetCustomData("FlatLighting").AsBool() ?? false)
            {
                return Puppet.PuppetMaterialFlat;
            }
            return Puppet.PuppetMaterial;
        }
    }

    protected PuppetSpriteData CurrentSprite;

    [ExportGroup("Storage")]
    [Export]
    protected Sprite3D Sprite;
    [Export]
    public Vector2I FrontBackOrder;
    [Export]
    public SortOrderEnum SortOrder;
    [Export]
    public bool Order;
    [Export]
    public float RotationOffset
    {
        get => _RotationOffset;
        set => _RotationOffset = value;
    }
    protected float _RotationOffset;

    [ExportGroup("Sprite Info")]
    [Export]
    public StringName SpriteGroup
    {
        get => _SpriteGroup;
        set
        {
            bool check = _SpriteGroup != value;
            _SpriteGroup = value;
            if(Sprite == null || TexPuppet == null)
                return;
            if(check)
                SetGroup(_SpriteGroup);
        }
    }
    protected StringName _SpriteGroup = "";

    [Export]
    public StringName SpriteName
    {
        get => _SpriteName;
        set
        {
            bool check = _SpriteName != value;
            _SpriteName = value;
            if(Sprite == null || TexPuppet == null)
                return;
            if(check)
                SetSprite(_SpriteGroup, _SpriteName);
        }
    }
    protected StringName _SpriteName = "";

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];
        if(TexPuppet == null)
            return properties;
            
        properties.Add(new()
        {
            {"name","Sprite Select"},
            {"usage", (int)PropertyUsageFlags.Group}
        });
        var groups = TexPuppet.GetSpriteGroups();
        properties.Add(new(){
            {"name","SpriteGroup"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", string.Join(',', groups)}
        });
        if(CurrentSprite != null)
        {
            var sprites = TexPuppet.GetSpritesInGroup(CurrentSprite.SpriteGroup);
            properties.Add(new(){
                {"name","SpriteName"},
                {"type", (int)Variant.Type.StringName},
                {"hint", (int)PropertyHint.Enum},
                {"hint_string", string.Join(',', sprites)}
            });
        }
        return properties;
    }

    private void SetGroup(StringName group)
    {
        if(TexPuppet == null)
            return;
        
        SetSprite(TexPuppet.GetFirstSprite(group));
    }

    public void SetSprite(PuppetSpriteData sprite)
    {
        if(TexPuppet == null)
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

    public virtual void UpdateLook()
    {
        if(Sprite == null)
            return;

        if(CurrentSprite != null)
        {
            var data = CurrentSprite.SpriteData;
            Sprite.FlipH = data.FlipH;
            Sprite.FlipV = data.FlipV;
            Sprite.Offset = data.GetCustomData("Offset").AsVector2() * new Vector2(1,-1);
            Sprite.MaterialOverride = PuppetMaterial;
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

    public override void _EnterTree()
    {
        if(Puppet == null)
        {
            Node current = GetParent();
            while(current != null)
            {
                if(current is Puppet3D puppet)
                {
                    Puppet = puppet;
                    break;
                }
                current = current.GetParent();
            }
        }

        Sprite ??= GetSprite("Sprite");

        if(SpriteGroup != "")
            SetSprite(SpriteGroup, SpriteName);
        else
            SetSprite(null);
        
        UpdateLook();
        SetOrder(Order);
    }

    Sprite3D GetSprite(string name)
    {
        var sprite = GetNodeOrNull<Sprite3D>(name);
        if(sprite == null)
        {
            sprite = new Sprite3D
            {
                Name = name,
                Texture = PuppetTexture,
                RegionEnabled = true,
                MaterialOverride = PuppetMaterial,
                PixelSize = 1f/(VectorHelpers.PixelSizeRoot*VectorHelpers.PixelSizeRoot)
            };
            AddChild(sprite, true, InternalMode.Front);
            sprite.Owner = Owner;
        }
        return sprite;
    }

    public override void Initialize(Puppet3D puppet, PuppetTransform2D control)
    {
        base.Initialize(puppet, control);
        if(control is PuppetBone2D bone)
            InitializeBone(bone);
    }

    public void InitializeBone(PuppetBone2D bone)
    {
        SortOrder = bone.SortOrder;

        Sprite ??= GetSprite("Sprite");
        
        Visible = bone.Visible;

        RotationOffset = bone.RotationOffset;
        
        if(bone.Flip) //Prevents an annoying bug where it dosen't flip when it's supposed to
        {
            SetFlip(false);
            SetFlip(true);
        }
        
        var (group, name) = bone.GetSprite();
        SetSprite(group, name);
    }

    public void SetOrderValues(Vector2I order)
    {
        FrontBackOrder = order;
        SetOrder(Order);
    }
    
    public void SetOrder(bool front)
    {
        Order = front;
        int order = SortOrder switch
        {
            SortOrderEnum.FRONT => FrontBackOrder.X,
            SortOrderEnum.BACK => FrontBackOrder.Y,
            SortOrderEnum.BOTH => Order ? FrontBackOrder.X : FrontBackOrder.Y,
            SortOrderEnum.CLOTHING => FrontBackOrder.X,
            _ => FrontBackOrder.X,
        };
        Sprite.Position = new Vector3(0, 0, order*0.0001f);
    }

    public void SetSprite(StringName group, StringName name)
    {
        if(TexPuppet != null)
            SetSprite(TexPuppet.GetSpriteReference(group, name));
        else
            SetSprite(null);
    }

    public override Transform2D GetLocalTransform() => Sprite.Transform.To2D(Flip);
    public override void SetLocalTransform(Transform2D transform)
    {
        var trans = transform.To3D();
        trans.Origin += Vector3.Back * Sprite.Position.Z;
        Sprite.Transform = trans;
    }
}
