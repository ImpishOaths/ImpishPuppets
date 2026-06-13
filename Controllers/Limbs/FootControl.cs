using Godot;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class FootControl : PuppetController
{
    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetBone3D")]
    private NodePath FootPath;
    private PuppetBone Foot;

    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetBone3D")]
    private NodePath FootAccessoryPath;
    private PuppetBone FootAccessory;

    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetBone3D")]
    private NodePath UpperLegPath;
    private PuppetBone UpperLeg;

    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetBone3D")]
    private NodePath LowerLegPath;
    private PuppetBone LowerLeg;

    [Export]
    private bool RightFoot;

    [Export]
    private bool LeftFacing;
    [Export]
    private bool FlipSide;

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];

        properties.Add(new(){
            {"name","PoseDropdown"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", "Right,Left,FlipRight,FlipLeft"}
        });

        return properties;
    }

    public override Variant _Get(StringName property)
    {
        if(property == "PoseDropdown")
        {
            if(LeftFacing)
                if(FlipSide)
                    return "FlipLeft";
                else
                    return "Left";
            else
                if(FlipSide)
                    return "FlipRight";
                else
                    return "Right";
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if(property == "PoseDropdown")
        {
            switch(value.AsStringName())
            {
                case "Right":
                    SetPose(false, false);
                    break;
                case "Left":
                    SetPose(true, false);
                    break;
                case "FlipRight":
                    SetPose(false, true);
                    break;
                case "FlipLeft":
                    SetPose(true, true);
                    break;
                default:
                    return false;
            }
            return true;
        }

        return false;
    }

    public override void _Ready()
    {
        Foot = GetNodeOrNull<PuppetBone>(FootPath);
        FootAccessory = GetNodeOrNull<PuppetBone>(FootAccessoryPath);
        UpperLeg = GetNodeOrNull<PuppetBone>(UpperLegPath);
        LowerLeg = GetNodeOrNull<PuppetBone>(LowerLegPath);
    }

    public override Variant ControlGet(StringName property)
    {
        return _Get(property);
    }

    public override Array<Dictionary> ControlPropertyList()
    {
        return _GetPropertyList();
    }

    public override bool ControlSet(StringName property, Variant variant)
    {
        return _Set(property, variant);
    }

    public void SetPose(bool leftFacing, bool flipSide)
    {
        LeftFacing = leftFacing;
        FlipSide = flipSide;

        Foot?.SetFlip(leftFacing);
        Foot?.SetOrder(FlipSide ^ RightFoot);
        FootAccessory?.SetFlip(leftFacing);
        FootAccessory?.SetOrder(FlipSide ^ RightFoot);
        
        UpperLeg?.SetFlip(leftFacing);
        UpperLeg?.SetOrder(FlipSide ^ RightFoot);
        LowerLeg?.SetFlip(leftFacing);
        LowerLeg?.SetOrder(FlipSide ^ RightFoot);
    }

    public override void Initialize()
    {
        _Ready();
        SetPose(LeftFacing, FlipSide);
    }
}
