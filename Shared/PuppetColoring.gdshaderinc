
vec2 maxIndex(vec2 a, vec2 b)
{
	if(a.x > b.x)
		return a;
	return b;
}

vec2 chooseColor(vec4 col)
{
	vec2 val = maxIndex(vec2(col.r, 0.25), vec2(col.g, 0.50));
	val = maxIndex(val, vec2(col.b, 0.75));
	if(val.x < 0.33)
		val = vec2(0, 0);
	float a = 1.0;
	if(col.a < 0.5)
		a = 0.0;
	return vec2(val.y, a);
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

vec3 chooseColor(float choice, vec3 color1, vec3 color2, vec3 color3)
{
	if(choice < 0.2)
		return vec3(0);
	else if(choice < 0.3)
		return color1;
	else if(choice < 0.6)
		return color2;
	else
		return color3;
}