shader_type canvas_item;
render_mode unshaded;

#include "res://addons/ImpishPuppets/Shared/PuppetColoring.gdshaderinc"

instance uniform vec3 color0: source_color;
instance uniform vec3 color1: source_color;
instance uniform vec3 color2: source_color;
instance uniform vec3 color3: source_color;
instance uniform bool shaded = true;

varying vec2 localLightDir;

void vertex()
{
	localLightDir = (vec4(normalize(LightDir), 0, 1) * MODEL_MATRIX).xy;
}

void fragment()
{
	vec2 jiggleOffset = getJiggleOffset(SCREEN_UV);
	vec2 uv = UV+jiggleOffset;
	
	vec4 texCol = texture(TEXTURE, uv);
	ivec2 baseChoice = chooseColor(texCol);
	vec3 baseCol = {color0, color1, color2, color3}[baseChoice.x];
	
	vec2 ld = normalize(localLightDir);
	vec4 lightTint = getLightTint(TEXTURE, uv, ld);
	vec4 shadowTint = getShadowTint(TEXTURE, uv, ld);
	if(shaded == false || baseChoice.x == 0 || baseChoice.y == 0)
	{
		lightTint = vec4(1);
		shadowTint = vec4(1);
	}
	
	vec3 color = baseCol * lightTint.rgb * shadowTint.rgb;
	COLOR = vec4(color, float(baseChoice.y));
}