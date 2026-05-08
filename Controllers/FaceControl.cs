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
    public static readonly StringName ExpressionGroup = "Expression";

    [Export]
    private NodePath PuppetPath;
    private SpriteSheet SpriteSheet;
    [Export]
    private ExpressionList ExpressionList;

    [Export]
    public bool DoBlinks = true;
    [Export]
    public float BlinkDuration = 0.1f;
    [Export]
    public float OpenDuration = 5f;
    [Export]
    public Vector2 FaceScale = Vector2.One;
    
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

    private StringName Expression
    {
        get => _Expression?.ExpressionName;
        set
        {
            if(ExpressionList != null && value != "")
            {
                var express = ExpressionList.Expressions[value];
                if(_Expression == express)
                    return;
                SetExpression(express);
            }
        }
    }
    private Expression _Expression;

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
        if(SpriteSheet == null)
            return properties;
            
        var eyes = string.Join(",", SpriteSheet.GetSpritesInGroup(EyeGroup));
        var eyebrows = string.Join(',', SpriteSheet.GetSpritesInGroup(EyebrowGroup));
        var mouths = string.Join(',', SpriteSheet.GetSpritesInGroup(MouthGroup));
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
        
        string expressions = "";
        if(ExpressionList != null)
            expressions = string.Join(',', ExpressionList.Expressions.Keys);
        properties.Add(new()
        {
            {"name", "Expression"},
            {"type", (int)Variant.Type.StringName},
            {"hint", (int)PropertyHint.Enum},
            {"hint_string", expressions}
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

    private PuppetTransform Head;
    private PuppetBone EyeRBone;
    private PuppetBone EyeLBone;
    private PuppetBone EyebrowLBone;
    private PuppetBone EyebrowRBone;
    private PuppetBone MouthBone;
    private PuppetBone ExpressionBone;

    public override void _Ready()
    {
        var parent = GetParent();
        Head = parent as PuppetTransform;
        EyebrowLBone = parent.GetNodeOrNull<PuppetBone>("EyebrowL");
        EyebrowRBone = parent.GetNodeOrNull<PuppetBone>("EyebrowR");
        EyeLBone = parent.GetNodeOrNull<PuppetBone>("EyeL");
        _EyeL = EyeLBone.SpriteName;
        EyeRBone = parent.GetNodeOrNull<PuppetBone>("EyeR");
        _EyeR = EyeRBone.SpriteName;
        MouthBone = parent.GetNodeOrNull<PuppetBone>("Mouth");
        ExpressionBone = parent.GetNodeOrNull<PuppetBone>("Expression");

        SpriteSheet = GetNodeOrNull<Puppet>(PuppetPath)?.GetSheet();

        StopBlink();
    }

    public override void Initialize()
    {
        _Ready();
    }

    public void Blink()
    {
        if(DoBlinks == false || IsBlinking || SpriteSheet == null)
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
        if(SpriteSheet == null)
            return;
        
        EyeRBone?.SetSprite(EyeGroup, _EyeR);
        EyeLBone?.SetSprite(EyeGroup, _EyeL);
        IsBlinking = false;
        BlinkTimer = 0;
        OpenTimer = 0;
    }

    public override void _Process(double delta)
    {
        if(SpriteSheet == null && PuppetPath != null)
            _Ready();
        HandleBlinks(delta);
        HandleExpression(delta);
    }

    private void HandleBlinks(double delta)
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

    private float ExpressionTimer;
    private float ExpressionMaxTime;
    private bool DoExpression;

    private void SetExpressionTransform()
    {
        float t = ExpressionTimer/ExpressionMaxTime;
        float rotation = _Expression.RotationCurve?.Sample(t) ?? 0;

        float scaleX = _Expression.ScaleXCurve?.Sample(t) ?? 1;
        if(Mathf.Abs(scaleX) < 0.01f)
            scaleX = 0.01f;

        float scaleY = _Expression.ScaleYCurve?.Sample(t) ?? 1;
        if(Mathf.Abs(scaleY) < 0.01f)
            scaleY = 0.01f;
            
        var trans = new Transform2D(rotation, new(scaleX, scaleY), 0, new(0,0));
        ExpressionBone.SetLocalTransform(trans);
    }

    private void HandleExpression(double delta)
    {
        if(DoExpression == false)
            return;
        
        ExpressionTimer += (float)delta;
        if(ExpressionTimer >= ExpressionMaxTime)
        {
            ExpressionTimer = ExpressionMaxTime;
            DoExpression = false;
        }
        SetExpressionTransform();
    }

    private void SetExpression(Expression expression)
    {
        _Expression = expression;
        if(ExpressionBone == null)
            return;
        ExpressionBone.SetSprite(ExpressionGroup, _Expression.ExpressionName);
        var trans = ExpressionBone.GetRootTransform();
        var headTrans = Head.GetRootTransform();
        trans = headTrans.TranslatedLocal(_Expression.Position*FaceScale);
        ExpressionBone.SetRootTransform(trans);

        ExpressionTimer = 0;
        ExpressionMaxTime = _Expression.Time;
        DoExpression = true;
    }

    public override Node ConvertTo3D(Puppet3D puppet)
    {
        var resize = VectorHelpers.PixelResize;
        var duplicate = Duplicate() as FaceControl;
        duplicate.FaceScale *= resize;
        return duplicate;
    }

    public override Array<Dictionary> ControlPropertyList()
    {
        return _GetPropertyList();
    }

    public override bool ControlSet(StringName property, Variant value)
    {
        switch(property)
        {
            case "EyeL":
                EyeL = value.AsStringName();
                return true;
            case "EyeR":
                EyeR = value.AsStringName();
                return true;
            case "EyebrowL":
                EyebrowL = value.AsStringName();
                return true;
            case "EyebrowR":
                EyebrowR = value.AsStringName();
                return true;
            case "Mouth":
                Mouth = value.AsStringName();
                return true;
            case "BothEyes":
                BothEyes = value.AsStringName();
                return true;
            case "BothEyebrows":
                BothEyebrows = value.AsStringName();
                return true;
            case "Expression":
                Expression = value.AsStringName();
                return true;
            default:
                return false;
        }
    }

    public override Variant ControlGet(StringName property)
    {
        return (string)property switch
        {
            "EyeL" => (Variant)EyeL,
            "EyeR" => (Variant)EyeR,
            "EyebrowL" => (Variant)EyebrowL,
            "EyebrowR" => (Variant)EyebrowR,
            "Mouth" => (Variant)Mouth,
            "BothEyes" => (Variant)BothEyes,
            "BothEyebrows" => (Variant)BothEyebrows,
            "Expression" => (Variant)Expression,
            _ => default,
        };
    }

    public override void _Notification(int what)
    {
        if(what == NotificationEditorPreSave)
        {
            Expression = "None";
        }
    }

}