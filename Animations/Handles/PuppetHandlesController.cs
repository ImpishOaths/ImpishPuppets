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
            LegL?.SetPosition(new Vector2(_StanceWidth, 0));
            LegR?.SetPosition(new Vector2(-_StanceWidth, 0));
        }
    }
    private float _StanceWidth = 10;

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

    public AnimationPlayer Animator {get; private set;}

    public override void _Ready()
    {
        Puppet = GetNodeOrNull(PuppetPath);
        CharacterData data = null;

        if(Puppet != null)
        {
            foreach(var child in FindChildren("*","RemotePuppetTransform").Cast<RemotePuppetTransform>())
            {
                child.SetReceiver(Puppet);
            }
            data = (Puppet as PuppetTransform).GetCharacterData();
        }

        Root = GetNode<Node2D>("Root");
        Root.Scale = Vector2.One / (Puppet is Node3D ? VectorHelpers.PixelSizeRoot : 1f);

        Body = Root.GetNode<Node2D>("Body");
        Lower = Body.GetNode<Node2D>("Lower");
        Upper = Lower.GetNode<Node2D>("Upper");
        Head = Upper.GetNode<Node2D>("Head");
        ArmL = Upper.GetNode<Node2D>("ArmL");
        ArmR = Upper.GetNode<Node2D>("ArmR");
        LegL = Root.GetNode<Node2D>("LegL");
        LegR = Root.GetNode<Node2D>("LegR");

        if(data != null)
        {
            BodyHeight = data.BodyHeight;
            UpperHeight = data.UpperHeight;
            HeadHeight = data.HeadHeight;
            ArmHeight = data.ArmHeight;
            ArmLengthL = data.ArmLengthL;
            ArmLengthR = data.ArmLengthR;
            HandMotionScale = data.HandMotionScale;
            StanceWidth = data.StanceWidth;
            FootMotionScale = data.FootMotionScale;
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

        FaceController = GetNode<RemoteController>("%FaceController");
        HandLController = GetNode<RemoteController>("%HandLController");
        HandRController = GetNode<RemoteController>("%HandRController");
        FootLController = GetNode<RemoteController>("%FootLController");
        FootRController = GetNode<RemoteController>("%FootRController");

        FaceController.SetReceiver(Puppet);
        HandLController.SetReceiver(Puppet);
        HandRController.SetReceiver(Puppet);
        FootLController.SetReceiver(Puppet);
        FootRController.SetReceiver(Puppet);

        Animator = GetNode<AnimationPlayer>("Full");
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
