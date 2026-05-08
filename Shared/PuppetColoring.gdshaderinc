
vec2 maxIndex(vec2 a, vec2 b)
{
	if(a.x > b.x)
		return a;
	return b;
}

ivec2 chooseColor(vec4 col)
{
	vec2 val = maxIndex(vec2(col.r, 1), vec2(col.g, 2));
	val = maxIndex(val, vec2(col.b, 3));
	if(val.x < 0.33)
		val = vec2(0, 0);
	int a = 1;
	if(col.a < 0.5)
		a = 0;
	return ivec2(int(val.y), a);
}

global uniform vec2 LightDir;
global uniform vec4 LightColor;
global uniform vec4 ShadowColor;
global uniform float LightStrength = 0.005;

global uniform float JiggleRate = 3.0;
global uniform float JiggleScale = 1.0;
global uniform float JiggleAmount = 0.02;
global uniform sampler2D JiggleTexture: filter_linear, repeat_enable;

vec2 getJiggleOffset(vec2 screenUV)
{
	
	float t = sin(floor(TIME*JiggleRate));
	return (texture(JiggleTexture, (screenUV+t)*JiggleScale).xy*2.0-1.0)*pow(JiggleAmount, 2.0);
}

vec4 getLightTint(sampler2D sprites, vec2 uv, vec2 lightDir)
{
	vec4 lightCol = texture(sprites, uv+(lightDir*LightStrength));
	ivec2 lightChoice = chooseColor(lightCol);
	
	if(lightChoice.x == 0 && lightChoice.y > 0)
		return LightColor;
	else
		return vec4(1);
}

vec4 getShadowTint(sampler2D sprites, vec2 uv, vec2 lightDir)
{
	vec4 shadowCol = texture(sprites, uv-(lightDir*LightStrength));
	ivec2 shadowChoice = chooseColor(shadowCol);
	
	if(shadowChoice.x == 0 && shadowChoice.y > 0)
		return ShadowColor;
	else
		return vec4(1);
}