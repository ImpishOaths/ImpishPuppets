using Godot;
using System;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class FaceControl: PuppetController
{
    public static readonly StringName EyeGroup = "Eye";
    public static readonly StringName EyebrowGroup = "Eyebrow";
    public static readonly StringName MouthGroup = "Mouth";
    public static readonly Array<StringName> BlinkExceptions = ["Closed","Arch","ArchDown"];
    public static readonly StringName BlinkName = "Closed";

    [Export]
    private NodePath PuppetPath;
    private Puppet Puppet;

    [Export]
    public bool DoBlinks = true;
    [Export]
    public float BlinkDuration = 0.1f;
    [Export]
    public float OpenDuration = 5f;
    
    private bool PauseBlinking;
    private bool ContinueBlinking;

    private float BlinkTimer;
    private float OpenTimer;
    private bool IsBlinking;

    private StringName EyeL
    {
        get => _EyeL;
        set
        {
            if(_EyeL != value)
            {
                _EyeL = value;
                Blink();
            }
        }
    }
    private StringName _EyeL;

    private StringName EyeR
    {
        get => _EyeR;
        set
        {
            if(_EyeR != value)
            {
                _EyeR = value;
                Blink();
            }
        }
    }
    private StringName _EyeR;

    private StringName BothEyes
    {
        get
        {
            if(_EyeR == _EyeL)
                return _EyeR;
            return null;
        }
        set
        {
            EyeR = value;
            EyeL = value;
            if(DoBlinks == false)
            {
                StopBlink();
            }
        }
    }

    private StringName EyebrowL
    {
        get => EyebrowLBone?.SpriteName;
        set => EyebrowLBone?.SetSprite(EyebrowGroup, value);
    }

    private StringName EyebrowR
    {
        get => EyebrowRBone?.SpriteName;
        set => EyebrowRBone?.SetSprite(EyebrowGroup, value);
    }

    private StringName BothEyebrows
    {
        get
        {
            if(EyebrowR == EyebrowL)
                return EyebrowR;
            return null;
        }
        set
        {
            EyebrowR = value;
            EyebrowL = value;
        }
    }

    private StringName Mouth
    {
        get => MouthBone?.SpriteName;
        set => MouthBone?.SetSprite(MouthGroup, value);
    }

    public override bool _Set(StringName property, Variant value)
    {
        if(property == "DoBlinks")
        {
            DoBlinks = value.AsBool();
            if(!DoBlinks)
                StopBlink();
            else
                OpenTimer = 0;
            return true;
        }
        return false;
    }

    public override void _Notification(int what)
    {
        if(what == NotificationEditorPreSave)
        {
            if(IsBlinking)
                StopBlink();
        }
    }
    
    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];
        if(Puppet == null)
            return properties;
            
        var eyes = string.Join(",", Puppet.GetSpritesInGroup(EyeGroup));
        var eyebrows = string.Join(',', Puppet.GetSpritesInGroup(EyebrowGroup));
        var mouths = string.Join(',', Puppet.GetSpritesInGroup(MouthGroup));
        properties.Add(new()
        {
            {"name","Individuals"},
            {"usage", (int)PropertyUsageFlags.Group}
        });
        properties.Add(new(){
            {"name","EyeL"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", eyes}
        });
        properties.Add(new(){
            {"name","EyeR"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", eyes}
        });
        properties.Add(new(){
            {"name","EyebrowL"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", eyebrows}
        });
        properties.Add(new(){
            {"name","EyebrowR"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", eyebrows}
        });
        
        properties.Add(new()
        {
            {"name","Full"},
            {"usage", (int)PropertyUsageFlags.Group}
        });
        properties.Add(new(){
            {"name","BothEyebrows"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", eyebrows}
        });
        properties.Add(new(){
            {"name","BothEyes"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", eyes}
        });
        properties.Add(new(){
            {"name","Mouth"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", mouths}
        });
        return properties;
    }

    private PuppetBone EyeRBone;
    private PuppetBone EyeLBone;
    private PuppetBone EyebrowLBone;
    private PuppetBone EyebrowRBone;
    private PuppetBone MouthBone;

    public override void _Ready()
    {
        EyebrowLBone = GetNodeOrNull<PuppetBone>("../EyebrowL");
        EyebrowRBone = GetNodeOrNull<PuppetBone>("../EyebrowR");
        EyeLBone = GetNodeOrNull<PuppetBone>("../EyeL");
        _EyeL = EyeLBone.SpriteName;
        EyeRBone = GetNodeOrNull<PuppetBone>("../EyeR");
        _EyeR = EyeRBone.SpriteName;
        MouthBone = GetNodeOrNull<PuppetBone>("../Mouth");

        Puppet = GetNodeOrNull<Puppet>(PuppetPath);

        StopBlink();
    }

    public override void Initialize()
    {
        _Ready();
    }

    public void Blink()
    {
        if(DoBlinks == false || IsBlinking || Puppet == null)
            return;

        if(BlinkExceptions.Contains(EyeR) || BlinkExceptions.Contains(EyeL))
        {
            PauseBlinking = true;
            StopBlink();
            return;
        }
        if(PauseBlinking)
        {
            ContinueBlinking = true;
            return;
        }
        PauseBlinking = false;
        
        EyeRBone?.SetSprite(EyeGroup, BlinkName);
        EyeLBone?.SetSprite(EyeGroup, BlinkName);
        IsBlinking = true;
        BlinkTimer = 0;
        OpenTimer = 0;
    }

    public void OnContinueBlinking()
    {
        PauseBlinking = false;
        ContinueBlinking = false;
        StopBlink();
    }

    public void StopBlink()
    {
        if(Puppet == null)
            return;
        
        EyeRBone?.SetSprite(EyeGroup, _EyeR);
        EyeLBone?.SetSprite(EyeGroup, _EyeL);
        IsBlinking = false;
        BlinkTimer = 0;
        OpenTimer = 0;
    }

    public override void _Process(double delta)
    {
        if(Puppet == null && PuppetPath != null)
            _Ready();

        if(ContinueBlinking)
            OnContinueBlinking();
        
        if(PauseBlinking || DoBlinks == false)
            return;
        
        if(IsBlinking)
        {
            BlinkTimer += (float)delta;
            if(BlinkTimer >= BlinkDuration)
            {
                StopBlink();
                OpenTimer += GD.Randf()*2f-1f;
            }
        }
        else
        {
            OpenTimer += (float)delta;
            if(OpenTimer >= OpenDuration)
            {
                Blink();
            }
        }
    }

    public override PuppetController MakeDuplicate3D()
    {
        var duplicate = Duplicate() as FaceControl;
        return duplicate;
    }

    public override Array<Dictionary> ControlPropertyList()
    {
        throw new NotImplementedException();
    }

    public override bool ControlSet(StringName property, Variant variant)
    {
        throw new NotImplementedException();
    }

    public override Variant ControlGet(StringName property)
    {
        throw new NotImplementedException();
    }
}