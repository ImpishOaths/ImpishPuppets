using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Bone3DIcon.png")]
public partial class PuppetBone3D: PuppetTransform3D, PuppetBone
{
    public const float ZSpacing = 0.003f;
    protected virtual SpriteSheet SpriteSheet => Puppet?.SpriteSheet;
    protected virtual Texture2D PuppetTexture => SpriteSheet.GetSpriteTexture(0);
    protected virtual Material PuppetMaterial => Puppet?.BoneMaterials3D[MaterialChoice];

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
    public int MaterialChoice;
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

    public void SetSprite(StringName group, StringName name)
    {
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
            Sprite.Offset = data.GetCustomData("Offset").AsVector2() * new Vector2(1,-1);
            Sprite.RegionRect = SpriteSheet.GetSprite(_SpriteGroup, _SpriteName).SpriteRegion;
            Sprite.MaterialOverride = PuppetMaterial;
            SetUpSpriteVariables(data);
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

    public virtual void SetUpSpriteVariables(TileData data) {}

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

        Sprite ??= GetSprite();

        SetSprite(SpriteGroup, SpriteName);
        
        UpdateLook();
        SetOrder(Order);
    }

    Sprite3D GetSprite()
    {
        var sprite = GetNodeOrNull<Sprite3D>("Sprite");
        if(sprite == null)
        {
            sprite = new Sprite3D
            {
                Name = "Sprite",
                Texture = PuppetTexture,
                MaterialOverride = PuppetMaterial,
                RegionEnabled = true,
                PixelSize = 1f/(VectorHelpers.PixelSizeRoot*VectorHelpers.PixelSizeRoot),
                CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
            };
            AddChild(sprite, true, InternalMode.Front);
        }
        return sprite;
    }
    
    public void SetOrder(bool front)
    {
        Order = front;
        int order = SortOrder switch
        {
            SortOrderEnum.FRONT | SortOrderEnum.BACK => Order ? FrontBackOrder.X : FrontBackOrder.Y,
            SortOrderEnum.FRONT => FrontBackOrder.X,
            SortOrderEnum.BACK => FrontBackOrder.Y,
            _ => FrontBackOrder.X,
        };
        Sprite.Position = new Vector3(0, 0, order*ZSpacing);
    }

    public override Transform2D GetLocalTransform() => Sprite.Transform.To2D(Flip);
    public override void SetLocalTransform(Transform2D transform)
    {
        var trans = transform.To3D();
        trans.Origin += Vector3.Back * Sprite.Position.Z;
        Sprite.Transform = trans;
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

    public Transform2D GetRealTransform() => Transform.To2D(_Flip);
    public void SetRealTransform(Transform2D trans)
    {
        trans.Origin *= VectorHelpers.PixelResize;
        Transform = trans.To3D();
    }
}
