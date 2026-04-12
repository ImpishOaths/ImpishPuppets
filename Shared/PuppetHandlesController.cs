using Godot;
using System;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class PuppetHandlesController: Node2D
{
    [Export]
    public NodePath PuppetPath;
    public Puppet Puppet;

    [Export]
    public Vector2 LowerScale
    {
        get => _LowerScale;
        set
        {
            _LowerScale = value;
            if(Lower != null)
                Lower.CharacterScale = _LowerScale;
        }
    }
    private Vector2 _LowerScale = Vector2.One;
    [Export]
    public Vector2 UpperScale
    {
        get => _UpperScale;
        set
        {
            _UpperScale = value;
            if(Upper != null)
                Upper.CharacterScale = _UpperScale;
        }
    }
    private Vector2 _UpperScale = Vector2.One;
    [Export]
    public Vector2 HeadScale
    {
        get => _HeadScale;
        set
        {
            _HeadScale = value;
            if(Head != null)
                Head.CharacterScale = _HeadScale;
        }
    }
    private Vector2 _HeadScale = Vector2.One;
    [Export]
    public Vector2 HandScale
    {
        get => _HandScale;
        set
        {
            _HandScale = value;
            if(HandL != null)
                HandL.CharacterScale = _HandScale;
            
            if(HandR != null)
                HandR.CharacterScale = _HandScale;
        }
    }
    private Vector2 _HandScale = Vector2.One;
    [Export]
    public Vector2 FootScale
    {
        get => _FootScale;
        set
        {
            _FootScale = value;
            if(FootL != null)
                FootL.CharacterScale = _FootScale;
            
            if(FootR != null)
                FootR.CharacterScale = _FootScale;
        }
    }
    private Vector2 _FootScale = Vector2.One;

    private PuppetHandle Lower;
    private PuppetHandle Upper;
    private PuppetHandle Head;
    private PuppetHandle HandL;
    private PuppetHandle HandR;
    private PuppetHandle FootL;
    private PuppetHandle FootR;

    public override void _Ready()
    {
        Lower = GetNode<PuppetHandle>("Lower");
        Upper = GetNode<PuppetHandle>("Lower/Upper");
        Head = GetNode<PuppetHandle>("Lower/Upper/Head");
        HandL = GetNode<PuppetHandle>("Lower/Upper/HandL");
        HandR = GetNode<PuppetHandle>("Lower/Upper/HandR");
        FootL = GetNode<PuppetHandle>("FootL");
        FootR = GetNode<PuppetHandle>("FootR");

        Puppet = GetNodeOrNull<Puppet>(PuppetPath);
        if(Puppet != null)
            Scale = Puppet.GetResize();

        LowerScale = _LowerScale;
        UpperScale = _UpperScale;
        HeadScale = _HeadScale;
        HandScale = _HandScale;
        FootScale = _FootScale;
    }
}
