shader_type spatial;
render_mode unshaded, cull_disabled;

#include "res://addons/ImpishPuppets/Shared/PuppetColoring.gdshaderinc"

uniform sampler2D spriteSheet: filter_linear, repeat_disable;
uniform sampler2D colorGradient: source_color, filter_nearest, repeat_disable;
uniform bool flatLighting;

varying vec2 localLightDir;

void vertex()
{
	localLightDir = normalize((vec4(normalize(LightDir), 0, 1) * MODEL_MATRIX).xy);
}

void fragment()
{
	vec2 jiggleOffset = getJiggleOffset(SCREEN_UV);
	vec2 uv = UV;
	
	vec4 texCol = texture(spriteSheet, uv+jiggleOffset);
	vec2 baseChoice = chooseColor(texCol);
	vec2 texUV = fract(SCREEN_UV*VIEWPORT_SIZE/200.0)/vec2(4,1);
	vec3 baseCol = texture(colorGradient,vec2(baseChoice.x,0)+texUV).rgb;
	
	vec4 lightTint = vec4(1);
	vec4 shadowTint = vec4(1);
	
	if(!flatLighting && baseChoice.x > 0.0 && baseChoice.y > 0.0)
	{
		vec4 lightCol = texture(spriteSheet, uv+jiggleOffset+(localLightDir*LightStrength));
		vec2 lightChoice = chooseColor(lightCol);
		if(abs(lightChoice.x) < 0.0001 && lightChoice.y > 0.0)
			lightTint = LightColor;
		
		vec4 shadowCol = texture(spriteSheet, uv+jiggleOffset-(localLightDir*LightStrength));
		vec2 shadowChoice = chooseColor(shadowCol);
		if(abs(shadowChoice.x) < 0.0001 && shadowChoice.y > 0.0)
			shadowTint = ShadowColor;
	}
	vec3 color = baseCol * pow(lightTint.rgb,vec3(1.3)) * shadowTint.rgb;
	ALBEDO = color;
	ALPHA = baseChoice.y;
	ALPHA_SCISSOR_THRESHOLD = 1.0;
}

