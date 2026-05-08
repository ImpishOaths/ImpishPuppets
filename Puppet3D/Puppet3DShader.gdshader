shader_type spatial;
render_mode unshaded, cull_disabled;

#include "res://addons/ImpishPuppets/Shared/PuppetColoring.gdshaderinc"

uniform sampler2D spriteSheet: filter_linear, repeat_disable;

uniform sampler2D gradient: source_color, filter_nearest, repeat_disable;
uniform bool shaded = true;

varying vec2 localLightDir;

void vertex()
{
	localLightDir = (vec4(normalize(LightDir), 0, 1) * MODEL_MATRIX).xy;
}

void fragment()
{
	vec2 jiggleOffset = getJiggleOffset(SCREEN_UV);
	vec2 uv = UV+jiggleOffset;
	
	vec4 texCol = texture(spriteSheet, uv);
	ivec2 baseChoice = chooseColor(texCol);
	
	vec3 baseCol = texture(gradient, vec2(float(baseChoice.x)/4.0, 0)).rgb;
	
	vec2 ld = normalize(localLightDir);
	vec4 lightTint = getLightTint(spriteSheet, uv, ld);
	vec4 shadowTint = getShadowTint(spriteSheet, uv, ld);
	if(shaded == false || baseChoice.x == 0 || baseChoice.y == 0)
	{
		lightTint = vec4(1);
		shadowTint = vec4(1);
	}
	
	vec3 color = baseCol * pow(lightTint.rgb,vec3(2)) * shadowTint.rgb;
	ALBEDO = color;
	ALPHA = float(baseChoice.y);
	ALPHA_SCISSOR_THRESHOLD = 1.0;
}