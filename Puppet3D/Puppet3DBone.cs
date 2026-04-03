using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet3DBone: Puppet3DControl
{
    private static QuadMesh SharedMesh = null;
    private MeshInstance3D Mesh;
    private MeshInstance3D BackMesh;

    private PuppetSpriteData CurrentSprite;

    [Export]
    private int Order;

    [ExportGroup("Sprite Info")]
    [Export]
    private StringName SpriteGroup
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
    private StringName SpriteName
    {
        get => _SpriteName;
        set
        {
            bool check = _SpriteName != value;
            _SpriteName = value;
            if(Mesh == null || Puppet == null)
                return;
            if(check)
                SetSprite(Puppet.GetSpriteReference(_SpriteGroup, _SpriteName));
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

    public override void UpdateLook()
    {
        if(Mesh == null)
            return;
        
        if(CurrentSprite != null)
        {
            var flip = new Vector2(_FlipH?-1:1, _FlipV?-1:1);
            Mesh.Scale = (((Vector2)CurrentSprite.SpriteRegion.Size)/Puppet.TileSize * flip).ToVec3scale();
            if(BackMesh != null)
                BackMesh.Scale = (((Vector2)CurrentSprite.SpriteRegion.Size)/Puppet.TileSize * flip).ToVec3scale();
            
            var region = ((Rect2)CurrentSprite.SpriteRegion).Scale(Puppet.TextureSize.Inverse()).ToVec4();
            Mesh.SetInstanceShaderParameter("region", region);
            BackMesh?.SetInstanceShaderParameter("region", region);

            var offset = (CurrentSprite.SpriteData.GetCustomData("Offset").AsVector2() / Puppet.TileSize * flip).ToVec3pos() / Mesh.Scale;
            Mesh.SetInstanceShaderParameter("offset", offset);
            BackMesh?.SetInstanceShaderParameter("offset", offset);
        }
        else
        {
            Mesh.SetInstanceShaderParameter("region", new Vector4(0, 0, 0, 0));
            BackMesh?.SetInstanceShaderParameter("region", new Vector4(0, 0, 0, 0));
        }
    }

    private void GetMesh()
    {
        if(Mesh == null)
        {
            Mesh = GetNodeOrNull<MeshInstance3D>("Mesh");
            if(Mesh == null)
            {
                SharedMesh ??= GD.Load<QuadMesh>("res://addons/ImpishPuppets/Quad.tres");
                Mesh = new MeshInstance3D
                {
                    Name = "Mesh",
                    Mesh = SharedMesh,
                    MaterialOverride = Puppet.DefaultMaterial,
                };
                AddChild(Mesh, true, InternalMode.Front);
            }
        }
    }
    private void GetBackMesh()
    {
        if(BackMesh == null)
        {
            BackMesh = GetNodeOrNull<MeshInstance3D>("BackMesh");
            if(BackMesh == null)
            {
                SharedMesh ??= GD.Load<QuadMesh>("res://addons/ImpishPuppets/Quad.tres");
                BackMesh = new MeshInstance3D
                {
                    Name = "BackMesh",
                    Mesh = SharedMesh,
                    MaterialOverride = Puppet.DefaultMaterial,
                };
                AddChild(BackMesh, true, InternalMode.Front);
            }
        }
    }

    public override void Initialize(Puppet3D puppet, Puppet2DControl bone)
    {
        Puppet = puppet;
        if(bone is Puppet2DBone bone2)
            InitializeBone(puppet, bone2);
    }

    public void InitializeBone(Puppet3D puppet, Puppet2DBone bone)
    {
        Puppet = puppet;
        
        if(Mesh == null)
            GetMesh();
        
        if(bone.SortOrder == Puppet2DBone.SortOrderEnum.BOTH)
            GetBackMesh();

        _FlipH = bone.FlipH;
        _FlipV = bone.FlipV;
        Position = (bone.Position / Puppet.TileSize).ToVec3pos();
        Rotation = new Vector3(0, 0, -bone.Rotation);
        var (group, name) = bone.GetSprite();
        SetSprite(Puppet.GetSpriteReference(group, name));
        Mesh.Rotation = new Vector3(0, 0, -bone.RotationOffset);
        if(BackMesh != null)
            BackMesh.Rotation = new Vector3(0, 0, -bone.RotationOffset);
    }

    public void SetOrder(int order, bool front)
    {
        if(front || BackMesh == null)
        {
            Mesh.Position = new(0, 0, order*Puppet.ZScale);
        }
        else
        {
            BackMesh.Position = new(0, 0, order*Puppet.ZScale);
        }
    }
}
