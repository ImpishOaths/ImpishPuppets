shader_type canvas_item;

#include "res://addons/ImpishPuppets/Shared/PuppetColoring.gdshaderinc"

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
	vec4 texCol = texture(TEXTURE, UV+jiggleOffset);
	
	vec2 baseChoice = chooseColor(texCol);
	vec3 color = chooseColor(baseChoice.x, color1, color2, color3);
	
	vec4 lightTint = vec4(0);
	vec4 shadowTint = vec4(1);
	
	if(baseChoice.x > 0.0 && baseChoice.y > 0.0)
	{
		vec4 lightCol = texture(TEXTURE, UV+jiggleOffset+(localLightDir*LightStrength));
		vec2 lightChoice = chooseColor(lightCol);
		if(abs(lightChoice.x) < 0.0001 && lightChoice.y > 0.0)
			lightTint = LightColor;
		
		vec4 shadowCol = texture(TEXTURE, UV+jiggleOffset-(localLightDir*LightStrength));
		vec2 shadowChoice = chooseColor(shadowCol);
		if(abs(shadowChoice.x) < 0.0001 && shadowChoice.y > 0.0)
			shadowTint = ShadowColor;
	}
	
	vec3 albedo = (color + lightTint.rgb/20.0) * shadowTint.rgb;
	COLOR = vec4(albedo, baseChoice.y);
}