using System.Linq;
using Godot;
using Godot.Collections;

namespace ImpishPuppets;

public interface Puppet
{
    public SpriteSheet GetSheet();
}

[Tool]
[GlobalClass]
[Icon("res://addons/ImpishPuppets/Icons/Puppet2DIcon.png")]
public partial class Puppet2D: PuppetTransform2D, Puppet
{
    public Puppet2D()
    {
        Puppet = this;
    }

    [Export]
    public SpriteSheet SpriteSheet;
    [Export]
    public CharacterData CharacterData;

    public SpriteSheet GetSheet() => SpriteSheet;
    public ImageTexture PuppetImageTexture = null;
    [Export]
    public Array<Material> BoneMaterials2D;
    [Export]
    public Array<Material> BoneMaterials3D;

    public Transform2D? InverseTransform {get; private set;} = null;
    public override void _PhysicsProcess(double delta)
    {
        InverseTransform = GlobalTransform.AffineInverse();
    }

    public override Variant _Get(StringName property)
    {
        if(property == "texture")
            return PuppetImageTexture;
        if(property == "image_source")
            return SpriteSheet.GetSpriteTexture(0);
        if(property == "material" && (BoneMaterials2D?.Count ?? 0) > 0)
            return BoneMaterials2D[0];
        if(property == "grid")
            return SpriteSheet.GetTextureRegionSize();
        
        return default;
    }

    public override void _EnterTree()
    {
        if(SpriteSheet != null)
        {
            var text = SpriteSheet.GetSpriteTexture(0);
            var image = text.GetImage();
            image.Decompress();
            PuppetImageTexture = ImageTexture.CreateFromImage(image);
        }
    }

    public void MakeNewBone(Node parent)
    {
        var pBone = new PuppetBone2D()
        {
            Puppet = this,
        };
        parent.AddChild(pBone, true);
        pBone.Owner = Owner ?? this;
    }

    public void MakeNewTransform(Node parent)
    {
        var pTransform = new PuppetTransform2D()
        {
            Puppet = this,
        };
        parent.AddChild(pTransform, true);
        pTransform.Owner = Owner ?? this;
    }
}
