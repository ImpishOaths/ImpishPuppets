using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Puppet3DBone: Puppet3DControl
{
    private static QuadMesh SharedMesh = null;
    private MeshInstance3D Mesh;

    private PuppetSpriteData CurrentSprite;

    [ExportGroup("Storage")]
    [Export]
    private int Order;
    [Export]
    private Puppet2DBone.SortOrderEnum SortOrder;
    [Export]
    private float RotationOffset;

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
        
        Mesh.Scale = (((Vector2)CurrentSprite.SpriteRegion.Size)/Puppet.TileSize).ToVec3scale();
            
        if(CurrentSprite != null)
        {
            var bflip = new Vector2(CurrentSprite.SpriteData.FlipH?1:0, CurrentSprite.SpriteData.FlipV?1:0);
            Mesh.SetInstanceShaderParameter("flip", bflip);
            
            var region = ((Rect2)CurrentSprite.SpriteRegion).Scale(Puppet.TextureSize.Inverse()).ToVec4();
            Mesh.SetInstanceShaderParameter("region", region);

            var offset = (CurrentSprite.SpriteData.GetCustomData("Offset").AsVector2() / Puppet.TileSize).ToVec3pos() / Mesh.Scale;
            Mesh.SetInstanceShaderParameter("offset", offset);
        }
        else
        {
            Mesh.SetInstanceShaderParameter("region", new Vector4(0, 0, 0, 0));
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
        Mesh.Position = new(0, 0, Order*Puppet.ZScale);

        if(SpriteGroup != "")
            SetSprite(SpriteGroup, SpriteName);
        else
            SetSprite(null);
    }

    MeshInstance3D GetMesh(string name)
    {
        var mesh = GetNodeOrNull<MeshInstance3D>(name);
        if(mesh == null)
        {
            SharedMesh ??= GD.Load<QuadMesh>("res://addons/ImpishPuppets/Quad.tres");
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

    public override void Initialize(Puppet3D puppet, Puppet2DControl bone)
    {
        Puppet = puppet;
        if(bone is Puppet2DBone bone2)
            InitializeBone(puppet, bone2);
    }

    public void InitializeBone(Puppet3D puppet, Puppet2DBone bone)
    {
        Puppet = puppet;
        
        SortOrder = bone.SortOrder;

        Mesh ??= GetMesh("Mesh");

        Position = (bone.Position / Puppet.TileSize).ToVec3pos();
        Rotation = new Vector3(0, 0, -bone.Rotation);
        var (group, name) = bone.GetSprite();
        SetSprite(group, name);
        RotationOffset = bone.RotationOffset;
    }

    public void SetOrder(int order)
    {
        Mesh.Position = new(0, 0, order*Puppet.ZScale);
        Order = order;
    }

    public void SetSprite(StringName group, StringName name)
    {
        if(Puppet != null)
            SetSprite(Puppet.GetSpriteReference(group, name));
        else
            SetSprite(null);
    }
}
