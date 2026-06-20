#if TOOLS
using Godot;
using System;

namespace ImpishPuppets;

[Tool]
public partial class ImpishPuppetsPlugin : EditorPlugin
{
    private void CheckAddGlobal(StringName name, RenderingServer.GlobalShaderParameterType type, Variant variant)
    {
        if(RenderingServer.GlobalShaderParameterGet(name).VariantType == Variant.Type.Nil)
            RenderingServer.GlobalShaderParameterAdd(
                name,
                type,
                variant);
    }
    public override void _EnterTree()
    {
        CheckAddGlobal(
            "AlphaThreshhold",
            RenderingServer.GlobalShaderParameterType.Float,
            0.5f);
        CheckAddGlobal(
            "BlackThreshhold",
            RenderingServer.GlobalShaderParameterType.Float,
            0.33f);

        CheckAddGlobal(
            "LightDir",
            RenderingServer.GlobalShaderParameterType.Vec2,
            new Vector2(0.5f,-1));
        CheckAddGlobal(
            "LightColor",
            RenderingServer.GlobalShaderParameterType.Color,
            new Color(1.8247963f, 1.8247963f, 1.8247963f, 1));
        CheckAddGlobal(
            "ShadowColor",
            RenderingServer.GlobalShaderParameterType.Color,
            new Color(0.5f, 0.5f, 0.5f, 1));
        CheckAddGlobal(
            "LightStrength",
            RenderingServer.GlobalShaderParameterType.Float,
            0.005f);

        CheckAddGlobal(
            "JiggleRate",
            RenderingServer.GlobalShaderParameterType.Float,
            3.0f);
        CheckAddGlobal(
            "JiggleScale",
            RenderingServer.GlobalShaderParameterType.Float,
            1.0f);
        CheckAddGlobal(
            "JiggleAmount",
            RenderingServer.GlobalShaderParameterType.Float,
            0.02f);
        CheckAddGlobal(
            "JiggleTexture",
            RenderingServer.GlobalShaderParameterType.Sampler2D,
            ResourceLoader.Load<Texture2D>("res://addons/ImpishPuppets/Shared/JiggleTexture.tres"));
    }
}
#endif
