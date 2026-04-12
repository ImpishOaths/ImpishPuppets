using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetBone3D: PuppetTransform3D, PuppetBone
{
    private static QuadMesh SharedMesh = null;
    private MeshInstance3D Mesh;

    private PuppetSpriteData CurrentSprite;

    [ExportGroup("Storage")]
    [Export]
    private Vector2I FrontBackOrder;
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
    private float _RotationOffset;

    [ExportGroup("Sprite Info")]
    [Export]
    public StringName SpriteGroup
    {
        get => _SpriteGroup;
        set
        {
            bool check = _SpriteGroup != value;
            _SpriteGroup = value;
            if(Mesh == null || Puppet == null)
                return;
            if(check)
                SetGroup(_SpriteGroup);
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
            if(Mesh == null || Puppet == null)
                return;
            if(check)
                SetSprite(_SpriteGroup, _SpriteName);
        }
    }
    private StringName _SpriteName;

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
        if(CurrentSprite != null)
        {
            var sprites = Puppet.GetSpritesInGroup(CurrentSprite.SpriteGroup);
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
        if(Puppet == null)
            return;
        
        SetSprite(Puppet.GetFirstSprite(group));
    }

    public void SetSprite(PuppetSpriteData sprite)
    {
        if(Puppet == null)
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

    public void UpdateLook()
    {
        if(Mesh == null)
            return;

        if(CurrentSprite != null)
        {
            var bflip = new Vector2I(CurrentSprite.SpriteData.FlipH?1:0, CurrentSprite.SpriteData.FlipV?1:0);
            Mesh.SetInstanceShaderParameter("flip", bflip);
            
            var region = ((Rect2)CurrentSprite.SpriteRegion).Scale(Puppet.TextureSize.Inverse()).ToVec4();
            Mesh.SetInstanceShaderParameter("region", region);

            var offset = CurrentSprite.SpriteData.GetCustomData("Offset").AsVector2() / (Mesh.Scale.ToVec2pos() * Puppet.TileSize);
            Mesh.SetInstanceShaderParameter("offset", offset);
            Mesh.Scale = (((Vector2)CurrentSprite.SpriteRegion.Size)/Puppet.TileSize).ToVec3scale();
        }
        else
        {
            Mesh.SetInstanceShaderParameter("flip", new Vector2I(0, 0));
            Mesh.SetInstanceShaderParameter("offset", new Vector2(0, 0));
            Mesh.SetInstanceShaderParameter("region", new Vector4(0, 0, 0, 0));
            Mesh.Scale = (Vector2.One/Puppet.TileSize).ToVec3scale();
        }

        Mesh.Rotation = new Vector3(0, 0, -RotationOffset);
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

        Mesh ??= GetMesh("Mesh");

        if(SpriteGroup != "")
            SetSprite(SpriteGroup, SpriteName);
        else
            SetSprite(null);
        
        UpdateLook();
        SetOrder(Order);
    }

    MeshInstance3D GetMesh(string name)
    {
        var mesh = GetNodeOrNull<MeshInstance3D>(name);
        if(mesh == null)
        {
            SharedMesh ??= GD.Load<QuadMesh>("res://addons/ImpishPuppets/Shared/Quad.tres");
            mesh = new MeshInstance3D
            {
                Name = name,
                Mesh = SharedMesh,
                MaterialOverride = Puppet.DefaultMaterial,
            };
            AddChild(mesh, true, InternalMode.Front);
        }
        return mesh;
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

        Mesh ??= GetMesh("Mesh");

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
            _ => 1,
        };
        Mesh.SetInstanceShaderParameter("zOffset", (float)order);
    }

    public void SetSprite(StringName group, StringName name)
    {
        if(Puppet != null)
            SetSprite(Puppet.GetSpriteReference(group, name));
        else
            SetSprite(null);
    }
}
