shader_type spatial;
render_mode unshaded, cull_disabled;

#include "res://addons/ImpishPuppets/Shared/PuppetColoring.gdshaderinc"

uniform sampler2DArray spriteSheets: filter_linear, repeat_disable;

instance uniform int clothingLayer;
instance uniform vec3 color1;
instance uniform vec3 color2;
instance uniform vec3 color3;

varying vec2 localLightDir;

void vertex()
{
	localLightDir = normalize((vec4(normalize(LightDir), 0, 1) * MODEL_MATRIX).xy);
}

void fragment()
{
	vec2 jiggleOffset = getJiggleOffset(SCREEN_UV);
	vec2 uv = UV;
	
	float layer = float(clothingLayer);
	
	vec4 texCol = texture(spriteSheets, vec3(uv+jiggleOffset, layer));
	vec2 baseChoice = chooseColor(texCol);
	vec3 color = chooseColor(baseChoice.x, color1, color2, color3);
	
	vec4 lightTint = vec4(1);
	vec4 shadowTint = vec4(1);
	
	if(baseChoice.x > 0.0 && baseChoice.y > 0.0)
	{
		vec4 lightCol = texture(spriteSheets, vec3(uv+jiggleOffset+(localLightDir*LightStrength), layer));
		vec2 lightChoice = chooseColor(lightCol);
		if(abs(lightChoice.x) < 0.0001 && lightChoice.y > 0.0)
			lightTint = LightColor;
		
		vec4 shadowCol = texture(spriteSheets, vec3(uv+jiggleOffset-(localLightDir*LightStrength), layer));
		vec2 shadowChoice = chooseColor(shadowCol);
		if(abs(shadowChoice.x) < 0.0001 && shadowChoice.y > 0.0)
			shadowTint = ShadowColor;
	}
	
	vec3 albedo = color * pow(lightTint.rgb,vec3(1.3)) * shadowTint.rgb;
	ALBEDO = albedo;
	ALPHA = baseChoice.y;
	ALPHA_SCISSOR_THRESHOLD = 1.0;
}

