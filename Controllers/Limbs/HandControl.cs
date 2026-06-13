using Godot;
using Godot.Collections;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class HandControl: PuppetController
{
    private static readonly StringName HandGroup = "Hand";
    private static readonly StringName FingerUpName = "FingerUp";
    private static readonly StringName FingerDownName = "FingerDown";
    private static readonly StringName PalmFlatName = "PalmFlat";
    private static readonly StringName PalmOutName = "Palm";

    [Export]
    public bool RightHand;

    private PuppetTransform Hand;
    private CharacterData CharacterData;

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
        var parent = GetParent();
        Hand = parent as PuppetTransform;
        CharacterData = Hand.GetCharacterData();

        PalmFront = parent.GetNodeOrNull<PuppetBone>("PalmFront");
        ThumbUp = parent.GetNodeOrNull<PuppetBone>("ThumbUp");
        Pinky = parent.GetNodeOrNull<PuppetBone>("Pinky");
        Ring = parent.GetNodeOrNull<PuppetBone>("Ring");
        Middle = parent.GetNodeOrNull<PuppetBone>("Middle");
        Index = parent.GetNodeOrNull<PuppetBone>("Index");
        ThumbDown = parent.GetNodeOrNull<PuppetBone>("ThumbDown");
        PalmBack = parent.GetNodeOrNull<PuppetBone>("PalmBack");

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
    public StringName SelectedPose
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
    private StringName _SelectedPose;

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
        {
            if(pose.PalmFlat)
                SetFlatHand(pose);
            else
                SetOutHand(pose);
        }
    }

    private void SetOutHand(HandPose pose)
    {
        static void setFinger(PuppetBone bone, bool up, bool front, Transform2D? trans)
        {
            bone?.SetVisible(true);
            bone?.SetSprite(HandGroup, up ? FingerUpName : FingerDownName);
            bone?.SetOrder(front);
            if(trans.HasValue)
                bone?.SetRealTransform(trans.Value);
        }

        HandData handData = CharacterData?.PalmOutPositions;

        bool front = RightHand ^ pose.FlipSide;
        setFinger(Pinky, pose.PinkyUp, front, handData?.Pinky);
        setFinger(Ring, pose.RingUp, front, handData?.Ring);
        setFinger(Middle, pose.MiddleUp, front, handData?.Middle);
        setFinger(Index, pose.IndexUp, front, handData?.Index);

        ThumbUp?.SetVisible(pose.ThumbUp);
        ThumbUp?.SetOrder(front);
        if(handData != null)
            ThumbUp?.SetRealTransform(handData.ThumbUp);
        ThumbDown?.SetVisible(!pose.ThumbUp);
        ThumbDown?.SetOrder(front);
        if(handData != null)
            ThumbDown?.SetRealTransform(handData.ThumbDown);

        PalmFront?.SetOrder(front);
        PalmFront?.SetVisible(pose.PalmBack);
        PalmFront?.SetSprite(HandGroup, PalmOutName);
        PalmBack?.SetOrder(front);
        PalmBack?.SetVisible(!pose.PalmBack);
        PalmBack?.SetSprite(HandGroup, PalmOutName);

        bool flip = RightHand ^ pose.PalmBack;
        Hand?.SetFlip(flip);
        UpperArm?.PropogateOrderFlip(front, flip);
        LowerArm?.PropogateOrderFlip(front, flip);
    }

    private void SetFlatHand(HandPose pose)
    {
        static void setFinger(PuppetBone bone, bool up, bool front, Transform2D? trans)
        {
            bone?.SetVisible(up);
            bone?.SetSprite(HandGroup, FingerUpName);
            bone?.SetOrder(front);
            if(trans.HasValue)
                bone?.SetRealTransform(trans.Value);
        }
        HandData handData = CharacterData?.PalmFlatPositions;

        bool front = RightHand ^ pose.FlipSide;
        setFinger(Pinky, pose.PinkyUp, front, handData?.Pinky);
        setFinger(Ring, pose.RingUp, front, handData?.Ring);
        setFinger(Middle, pose.MiddleUp, front, handData?.Middle);
        setFinger(Index, pose.IndexUp, front, handData?.Index);

        ThumbUp?.SetVisible(pose.ThumbUp);
        ThumbUp?.SetOrder(front);
        if(handData != null)
            ThumbUp?.SetRealTransform(handData.ThumbUp);
        ThumbDown?.SetVisible(!pose.ThumbUp);
        ThumbDown?.SetOrder(front);
        if(handData != null)
            ThumbDown?.SetRealTransform(handData.ThumbDown);

        PalmFront?.SetOrder(front);
        PalmFront?.SetVisible(false);
        PalmFront?.SetSprite(HandGroup, PalmFlatName);
        PalmBack?.SetOrder(front);
        PalmBack?.SetVisible(true);
        PalmBack?.SetSprite(HandGroup, PalmFlatName);

        bool flip = RightHand ^ pose.PalmBack;
        Hand?.SetFlip(flip);
        UpperArm?.PropogateOrderFlip(front, flip);
        LowerArm?.PropogateOrderFlip(front, flip);
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

    [ExportToolButton("Register Finger Transforms")]
    public Callable RegisterFingersCallable => Callable.From(RegisterFingerTransforms);
    public void RegisterFingerTransforms()
    {
        HandData handData;
        var Pose = Poses.Poses[_SelectedPose];
        if(Pose.PalmFlat)
            handData = CharacterData.PalmFlatPositions;
        else
            handData = CharacterData.PalmOutPositions;

        handData.ThumbUp = ThumbUp.GetRealTransform();
        handData.Pinky = Pinky.GetRealTransform();
        handData.Ring = Ring.GetRealTransform();
        handData.Middle = Middle.GetRealTransform();
        handData.Index = Index.GetRealTransform();
        handData.ThumbDown = ThumbDown.GetRealTransform();

        ResourceSaver.Save(handData);
    }

}
