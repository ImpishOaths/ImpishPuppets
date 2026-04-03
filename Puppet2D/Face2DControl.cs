using Godot;
using System;
using Godot.Collections;

namespace ImpishPuppets;

[Tool]
[GlobalClass]
public partial class Face2DControl: Puppet2DControl
{
    public static readonly StringName EyeGroup = "Eye";
    public static readonly StringName EyebrowGroup = "Eyebrow";
    public static readonly StringName MouthGroup = "Mouth";
    public static readonly Array<StringName> BlinkExceptions = ["Closed","Arch","ArchDown"];
    public static readonly StringName BlinkName = "Closed";

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
            if(_EyeL == _EyeR)
                return _EyeL;
            return "";
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
        get
        {
            if(EyebrowLBone == null)
                return null;
            return EyebrowLBone.GetSprite().name;
        }
        set
        {
            if(EyebrowLBone == null)
                return;
            EyebrowLBone.SetSprite(EyebrowGroup, value);
        }
    }

    private StringName EyebrowR
    {
        get
        {
            if(EyebrowRBone == null)
                return null;
            return EyebrowRBone.GetSprite().name;
        }
        set
        {
            if(EyebrowRBone == null)
                return;
            EyebrowRBone.SetSprite(EyebrowGroup, value);
        }
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
        get
        {
            if(MouthBone == null)
                return null;
            return MouthBone.GetSprite().name;
        }
        set
        {
            if(MouthBone == null)
                return;
            MouthBone.SetSprite(MouthGroup, value);
        }
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
    
    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];
        if(IsReady == false)
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

    private bool IsReady;
    private Puppet2DBone EyeRBone;
    private Puppet2DBone EyeLBone;
    private Puppet2DBone EyebrowLBone;
    private Puppet2DBone EyebrowRBone;
    private Puppet2DBone MouthBone;

    public override void _Ready()
    {
        EyebrowLBone = GetNode<Puppet2DBone>("EyebrowR");
        EyebrowRBone = GetNode<Puppet2DBone>("EyebrowL");
        EyeRBone = GetNode<Puppet2DBone>("EyeR");
        EyeLBone = GetNode<Puppet2DBone>("EyeL");
        MouthBone = GetNode<Puppet2DBone>("Mouth");

        IsReady = true;
        StopBlink();
    }

    public void Blink()
    {
        if(DoBlinks == false || IsBlinking || IsReady == false)
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
        
        EyeRBone.SetSprite(EyeGroup, BlinkName);
        EyeLBone.SetSprite(EyeGroup, BlinkName);
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
        EyeRBone.SetSprite(EyeGroup, EyeR);
        EyeLBone.SetSprite(EyeGroup, EyeL);
        IsBlinking = false;
        BlinkTimer = 0;
        OpenTimer = 0;
    }

    public override void _Process(double delta)
    {
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
}