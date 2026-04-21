using Godot;
using Godot.Collections;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class HandControl: PuppetController
{
    private static readonly StringName HandGroup = "Hand";
    //private static readonly StringName ThumbName = "Thumb";
    //private static readonly StringName ThumbDownName = "ThumbDown";
    private static readonly StringName FingerName = "Finger";
    private static readonly StringName FingerDownName = "FingerDown";

    [Export]
    public bool RightHand;

    private PuppetTransform Hand;

    private PuppetBone PalmFront;
    private PuppetBone ThumbUp;
    private PuppetBone Pinky;
    private PuppetBone Ring;
    private PuppetBone Middle;
    private PuppetBone Index;
    private PuppetBone ThumbDown;
    private PuppetBone PalmBack;

    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetBone3D")]
    private NodePath UpperArmPath;
    private PuppetBone UpperArm;

    [Export(PropertyHint.NodePathValidTypes, "PuppetBone2D,PuppetBone3D")]
    private NodePath LowerArmPath;
    private PuppetBone LowerArm;

    public override void _Ready()
    {
        Hand = GetParentOrNull<PuppetTransform>();

        PalmFront = GetNodeOrNull<PuppetBone>("../PalmFront");
        ThumbUp = GetNodeOrNull<PuppetBone>("../ThumbUp");
        Pinky = GetNodeOrNull<PuppetBone>("../Pinky");
        Ring = GetNodeOrNull<PuppetBone>("../Ring");
        Middle = GetNodeOrNull<PuppetBone>("../Middle");
        Index = GetNodeOrNull<PuppetBone>("../Index");
        ThumbDown = GetNodeOrNull<PuppetBone>("../ThumbDown");
        PalmBack = GetNodeOrNull<PuppetBone>("../PalmBack");

        UpperArm = GetNodeOrNull<PuppetBone>(UpperArmPath);
        LowerArm = GetNodeOrNull<PuppetBone>(LowerArmPath);
    }

    public override void Initialize()
    {
        _Ready();
        SetPose(_SelectedPose);
    }


    [Export]
    public HandPoseList Poses;
    
    [Export]
    public string SelectedPose
    {
        get => _SelectedPose;
        set
        {
            if(_SelectedPose != value)
            {
                _SelectedPose = value;
                SetPose(value);
            }
        }
    }
    private string _SelectedPose;

    public override Variant _Get(StringName property)
    {
        if(property == "PoseDropdown")
            return _SelectedPose;
        
        return default;
    }

    public override bool _Set(StringName property, Variant variant)
    {
        if(property == "PoseDropdown")
            SelectedPose = variant.AsStringName();
        
        return false;
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];

        if(Poses == null)
            return properties;
        
        properties.Add(new(){
            {"name","PoseDropdown"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", string.Join(',', Poses.Poses.Keys)}
        });

        return properties;
    }

    private void SetPose(StringName name)
    {
        if(Poses != null && Poses.Poses.TryGetValue(name, out var pose))
            SetPose(pose);
    }

    private void SetPose(HandPose pose)
    {
        static void setFinger(PuppetBone bone, bool up, bool front)
        {
            bone?.SetSprite(HandGroup, up ? FingerName : FingerDownName);
            bone?.SetOrder(front);
        }

        bool front = RightHand ^ pose.HandBehind;
        setFinger(Pinky, pose.PinkyUp, front);
        setFinger(Ring, pose.RingUp, front);
        setFinger(Middle, pose.MiddleUp, front);
        setFinger(Index, pose.IndexUp, front);

        ThumbUp?.SetVisible(pose.ThumbUp);
        ThumbUp?.SetOrder(front);
        ThumbDown?.SetVisible(!pose.ThumbUp);
        ThumbDown?.SetOrder(front);

        PalmFront?.SetOrder(front);
        PalmFront?.SetVisible(pose.PalmBack);
        PalmBack?.SetOrder(front);
        PalmBack?.SetVisible(!pose.PalmBack);

        bool flip = RightHand ^ pose.PalmBack;
        Hand?.SetFlip(flip);
        UpperArm?.SetFlip(flip);
        UpperArm?.SetOrder(front);
        LowerArm?.SetFlip(flip);
        LowerArm?.SetOrder(front);
    }

    public override PuppetController MakeDuplicate3D()
    {
        return Duplicate() as HandControl;
    }

    public override Array<Dictionary> ControlPropertyList()
    {
        return _GetPropertyList();
    }

    public override bool ControlSet(StringName property, Variant variant)
    {
        return _Set(property, variant);
    }

    public override Variant ControlGet(StringName property)
    {
        return _Get(property);
    }

}
