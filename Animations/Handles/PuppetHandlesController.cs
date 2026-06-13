using Godot;
using System;
using System.Linq;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetHandlesController: Node
{
    [Export]
    public string PuppetPath = "../Puppet";
    public Node Puppet;
    [Export]
    public bool Mode3D = false;

    [Export]
    public float BodyHeight
    {
        get => _BodyHeight;
        set
        {
            _BodyHeight = value;
            if(!_BodyHeightOverride)
                Body?.SetPosition(new Vector2(0, -_BodyHeight));
        }
    }
    private float _BodyHeight = 10;

    [Export]
    public bool BodyHeightOverride
    {
        get => _BodyHeightOverride;
        set
        {
            _BodyHeightOverride = value;
            if(!_BodyHeightOverride)
                Body?.SetPosition(new Vector2(0, -_BodyHeight));
        }
    }
    private bool _BodyHeightOverride = false;

    [Export]
    public float UpperHeight
    {
        get => _UpperHeight;
        set
        {
            _UpperHeight = value;
            Upper?.SetPosition(new Vector2(0, -_UpperHeight));
        }
    }
    private float _UpperHeight = 10;

    [Export]
    public float HeadHeight
    {
        get => _HeadHeight;
        set
        {
            _HeadHeight = value;
            Head?.SetPosition(new Vector2(0, -_HeadHeight*_HeadHeightScale));
        }
    }
    private float _HeadHeight = 10;

    [Export]
    public float HeadHeightScale
    {
        get => _HeadHeightScale;
        set
        {
            _HeadHeightScale = value;
            Head?.SetPosition(new Vector2(0, -_HeadHeight*_HeadHeightScale));
        }
    }
    private float _HeadHeightScale = 1;

    [Export]
    public float ArmHeight
    {
        get => _ArmHeight;
        set
        {
            _ArmHeight = value;
            ArmL?.SetPosition(new Vector2(_ArmLengthL, -_ArmHeight));
            ArmR?.SetPosition(new Vector2(-_ArmLengthR, -_ArmHeight));
        }
    }
    private float _ArmHeight = 10;
    
    [Export]
    public float ArmLengthL
    {
        get => _ArmLengthL;
        set
        {
            _ArmLengthL = value;
            ArmL?.SetPosition(new Vector2(_ArmLengthL, -_ArmHeight));
        }
    }
    private float _ArmLengthL = 10;
    
    [Export]
    public float ArmLengthR
    {
        get => _ArmLengthR;
        set
        {
            _ArmLengthR = value;
            ArmR?.SetPosition(new Vector2(-_ArmLengthR, -_ArmHeight));
        }
    }
    private float _ArmLengthR = 10;
    
    [Export]
    public Vector2 HandMotionScale
    {
        get => _HandMotionScale;
        set
        {
            _HandMotionScale = value;
            ArmL?.SetScale(HandMotionScale);
            ArmR?.SetScale(HandMotionScale);
        }
    }
    private Vector2 _HandMotionScale = Vector2.One;

    [Export]
    public float StanceWidth
    {
        get => _StanceWidth;
        set
        {
            _StanceWidth = value;
            var height = Mathf.Tan(_StanceAngle)*_StanceWidth;
            LegL?.SetPosition(new Vector2(_StanceWidth, height));
            LegR?.SetPosition(new Vector2(-_StanceWidth, -height));
        }
    }
    private float _StanceWidth = 10;
    [Export]
    public float StanceAngle
    {
        get => _StanceAngle;
        set
        {
            _StanceAngle = value;
            var height = Mathf.Tan(_StanceAngle)*_StanceWidth;
            LegL?.SetPosition(new Vector2(_StanceWidth, height));
            LegR?.SetPosition(new Vector2(-_StanceWidth, -height));
        }
    }
    private float _StanceAngle = 0;

    [Export]
    public Vector2 FootMotionScale
    {
        get => _FootMotionScale;
        set
        {
            _FootMotionScale = value;
            LegL?.SetScale(FootMotionScale);
            LegR?.SetScale(FootMotionScale);
        }
    }
    private Vector2 _FootMotionScale = Vector2.One;

    [Export]
    public float PuppetRotate
    {
        get => Puppet != null && Puppet is Puppet3D puppet3D ? puppet3D.Rotation.Y : default;
        set
        {
            if(Puppet == null || Puppet is not Puppet3D puppet3D)
                return;
            var add = PuppetFlip?-Mathf.Pi:0;
            puppet3D.Rotation = new(puppet3D.Rotation.X, value, add);
        }
    }
    [Export]
    public bool PuppetFlip
    {
        get => Puppet != null && Puppet is Puppet3D puppet3D && puppet3D.Flip;
        set
        {
            if(Puppet == null || Puppet is not Puppet3D puppet3D)
                return;
            puppet3D.SetFlip(value);
        }
    }


    private RemoteController FaceController;
    private RemoteController HandLController;
    private RemoteController HandRController;
    private RemoteController FootLController;
    private RemoteController FootRController;

    private Node2D Root;
    private Node2D Body;
    private Node2D Lower;
    private Node2D Upper;
    private Node2D Head;
    private Node2D ArmL;
    private Node2D ArmR;
    private Node2D LegL;
    private Node2D LegR;

    private CharacterData CharacterData = null;

    public AnimationPlayer Animator {get; private set;}

    public void ChangePuppet(Node puppet)
    {
        Puppet = puppet;

        if(Puppet != null)
        {
            foreach(var child in FindChildren("*","RemotePuppetTransform").Cast<RemotePuppetTransform>())
            {
                child.SetReceiver(Puppet);
            }
            CharacterData = (Puppet as PuppetTransform).GetCharacterData();
        }
        
        if(CharacterData != null)
        {
            BodyHeight = CharacterData.BodyHeight;
            UpperHeight = CharacterData.UpperHeight;
            HeadHeight = CharacterData.HeadHeight;
            ArmHeight = CharacterData.ArmHeight;
            ArmLengthL = CharacterData.ArmLengthL;
            ArmLengthR = CharacterData.ArmLengthR;
            HandMotionScale = CharacterData.HandMotionScale;
            StanceWidth = CharacterData.StanceWidth;
            FootMotionScale = CharacterData.FootMotionScale;
        }
        else
        {
            BodyHeight = _BodyHeight;
            UpperHeight = _UpperHeight;
            HeadHeight = _HeadHeight;
            ArmHeight = _ArmHeight;
            ArmLengthL = _ArmLengthL;
            ArmLengthR = _ArmLengthR;
            HandMotionScale = _HandMotionScale;
            StanceWidth = _StanceWidth;
            FootMotionScale = _FootMotionScale;
        }

        FaceController.SetReceiver(Puppet);
        HandLController.SetReceiver(Puppet);
        HandRController.SetReceiver(Puppet);
        FootLController.SetReceiver(Puppet);
        FootRController.SetReceiver(Puppet);
    }

    public override void _Ready()
    {
        Root = GetNode<Node2D>("Root");
        if(Mode3D)
            Root.Scale = Vector2.One/VectorHelpers.PixelSizeRoot;

        Body = Root.GetNode<Node2D>("Body");
        Lower = Body.GetNode<Node2D>("Lower");
        Upper = Lower.GetNode<Node2D>("Upper");
        Head = Upper.GetNode<Node2D>("Head");
        ArmL = Upper.GetNode<Node2D>("ArmL");
        ArmR = Upper.GetNode<Node2D>("ArmR");
        LegL = Root.GetNode<Node2D>("LegL");
        LegR = Root.GetNode<Node2D>("LegR");
        
        FaceController = GetNode<RemoteController>("%FaceController");
        HandLController = GetNode<RemoteController>("%HandLController");
        HandRController = GetNode<RemoteController>("%HandRController");
        FootLController = GetNode<RemoteController>("%FootLController");
        FootRController = GetNode<RemoteController>("%FootRController");

        Animator = GetNode<AnimationPlayer>("Full");

        ChangePuppet(GetNode(PuppetPath));
    }

    [ExportToolButton("Register Scales")]
    public Callable RegisterScalesCallable => Callable.From(RegisterScales);
    public void RegisterScales()
    {
        if(Puppet == null)
            return;
        var data = (Puppet as PuppetTransform).GetCharacterData();
        data.BodyHeight = BodyHeight;
        data.UpperHeight = UpperHeight;
        data.HeadHeight = HeadHeight;
        data.ArmHeight = ArmHeight;
        data.ArmLengthL = ArmLengthL;
        data.ArmLengthR = ArmLengthR;
        data.HandMotionScale = HandMotionScale;
        data.StanceWidth = StanceWidth;
        data.FootMotionScale = FootMotionScale;
    }
}
