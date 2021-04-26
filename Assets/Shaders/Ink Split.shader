Shader "Unlit/Ink Split"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    	_ShadeSpeed("Shade Speed", float) = 0.1
    	_E1Speed("Effect 1 Speed", float) = 0.05
    	_E2Speed("Effect 2 Speed", float) = 0.05
    	_E3Speed("Effect 3 Speed", float) = .005
    	_ColorSpeed("Color Speed", float) = .5
   		_BGColor("BG Color", Color) = (0.95, 0.96, 0.8,0.1) 
   		_Color("Color", Color) = (0.95, 0.96, 0.8,0.1) 
    	_Mix("mix", Range(0,1)) = .00
    	_Brightness("Brightness", Range(-1,1)) = .1
    	_Contrast("Contrast", Range(-1,3)) = .00
    	_F1("F1", Range(0,0.5)) = .00
    	_F2("F2", Range(0.0,2)) = .00
    	_EdgeSmooth("Edge smooth",Range(0,1)) = 0.2
    	_LeftSmooth("Left smooth",Range(0,1)) = 0.2
    	_RightSmooth("Right smooth",Range(0,1)) = 0.6
    	
    	
   		_Color1("Color1", Color) = (0.95, 0.0, 0.8,0.1) 
   		_Color2("Color2", Color) = (0., 0.96, 0.8,0.1) 
   		_Color3("Color3", Color) = (0.95, 0.96, 0.,0.1) 
    	
    	_A("A", float) = 5
    }
    SubShader
    {
        Tags { 
			 "Queue"="Transparent"
        	"RenderType"="Transparent" 
        	}
		Blend SrcAlpha OneMinusSrcAlpha
    	//Cull back
    	//ZWrite Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#define mod(x,y) (x-y*floor(x/y))

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ShadeSpeed;
            float _E1Speed;
            float _E2Speed;
            float _E3Speed;
            float4 _BGColor;
            float4 _Color;
            float _A;
            float _ColorSpeed;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float _Mix;
            float _Brightness;
            float _Contrast;
            float _F1;
            float _F2;
			float _EdgeSmooth;
            float _LeftSmooth;
            float _RightSmooth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // https://www.shadertoy.com/view/Md33zB
// 3D simplex noise from: https://www.shadertoy.com/view/XsX3zB
static const float F3 =  0.3333333;
static const float G3 =  0.1666667;

float3 random3(float3 c) {
    float j = 4096.0*sin(dot(c,float3(17.0, 59.4, 15.0)));
    float3 r;
    r.z = frac(512.0*j);
    j *= .125;
    r.x = frac(512.0*j);
    j *= .125;
    r.y = frac(512.0*j);
    return r-0.5;
}

float simplex3d(float3 p) {
	 float3 s = floor(p + dot(p, F3));
	 float3 x = p - s + dot(s, G3);
	 
	 float3 e = step(0.0, x - x.yzx);
	 float3 i1 = e*(1.0 - e.zxy);
	 float3 i2 = 1.0 - e.zxy*(1.0 - e);
	 
	 float3 x1 = x - i1 + G3;
	 float3 x2 = x - i2 + 2.0*G3;
	 float3 x3 = x - 1.0 + 3.0*G3;
	 
	 float4 w, d;
	 
	 w.x = dot(x, x);
	 w.y = dot(x1, x1);
	 w.z = dot(x2, x2);
	 w.w = dot(x3, x3);
	 
	 w = max(0.6 - w, 0.0);
	 
	 d.x = dot(random3(s), x);
	 d.y = dot(random3(s + i1), x1);
	 d.z = dot(random3(s + i2), x2);
	 d.w = dot(random3(s + 1.0), x3);
	 
	 w *= w;
	 w *= w;
	 d *= w;
	 
	 return dot(d, 52.0);
}

float fbm(float3 p)
{
	float f = 0.0;	
	float frequency = 1.0;
	float amplitude = 0.5;
	for (int i = 0; i < 4; i++)
	{
		f += simplex3d(p * frequency) * amplitude;
		amplitude *= 0.5;
		frequency *= 2.0 + float(i) / 100.0;
	}
	return min(f, 1.0);
}

float random (in float2 st) 
{ 
    return frac(sin(dot(st.xy,float2(12.9798,78.323)))* 43858.5563313);
}

// -----------------------------------------------------------------------------

// Recreating the effect from After Effects
float2 rectToPolar(float2 p, float2 ms) {
	p -= ms / 2.0;
	const float PI = 3.1415926534;
	float r = length(p);
	float a = ((atan2(p.y, p.x) / PI) * 0.5 + 0.5) * ms.x;
	return float2(a, r);	
}

// A line as mask, with 'f' as feather
float Line(float v, float from, float to, float f)
{
	float d = max(from - v, v - to);
	return 1.0 - smoothstep(0.0, f, d);
}

// -----------------------------------------------------------------------------

float effect(float2 p, float o) {
    p *= 2.0;
    //float f1 = fbm(float3(p * float2(13.0, 1.0) + 100.0 + float2(0.0, o), _Time.y * .005) ) * 0.5;
    float f1 = simplex3d(float3(p * float2(1.0, 5.0), _Time.y * _E1Speed) ) * 0.5 + 0.5;
    
 	float e = fbm(float3(p * float2(15.0, 1.0) + float2(f1 * 0.85, o), _Time.y * _E3Speed));
    
    e = abs(e) * sqrt(p.y / 5.0);
    
    float c2 = simplex3d(float3(p * float2(6.0, 2.0), _Time.y * _E2Speed));
    
    c2 = (c2 * 0.5) + 0.5;
    c2 *= 0.5; //sqrt(p.y / 5.0);
    
    e += c2;
    
    return e * 0.5;
}

// ShockWave technique
float sw(float2 p, float2 ms) {
		
	p = rectToPolar(p, ms);
	
	// Offset it on the x
	p.x = mod(p.x + 0.5, ms.x);
	
	// Create the seem mask at that offset
	const float b = 0.5;
	const float d = 0.04;
	float seem = Line(p.x, -1.0, d, b) + Line(p.x, ms.x - d, ms.x + 1.0, b);
	seem = min(seem, 1.0);
	
	float s1 = effect(p, 0.0);
	
	// Create another noise to fade to, but the seem has the be at a different position
	p.x = mod(p.x + 0.6, ms.x);
	float s2 = effect(p, -1020.0);
	
	// Blend them together
	//float s = s1;
	float s = lerp(s1, s2, seem);
	
	//float m = line(p.y, -0.1, 0.2 + s * 0.9, 0.2);
    // appear effect 
    //float perc = min( max(abs(sin(_Time.y * 0.1)), _Time.y * 0.1), 1.0);
    float perc = 0.8;
    
    float f1 = perc * _F1;
    float f2 = perc * _F2;
    
    float m = Line(p.y, -0.1, f1 + s * f2, _EdgeSmooth);
	
	return smoothstep(_LeftSmooth, _RightSmooth, m);
}

fixed4 frag (v2f i) : SV_Target
//void mainImage( out float4 fragColor, in float2 fragCoord )
{
	float2 p = i.uv;
	//float2 p = fragCoord.xy / iResolution.yy;
	float m = 1;
    //float m = iResolution.x / iResolution.y;
	float2 ms = float2(m, 1.0);
    float c = 0.0;
    float s = sw(p, ms);
    c += s;
    float t = random(p * 4.0);
    float shade = fbm(float3(p * 3.0, _Time.y * _ShadeSpeed) ) * 0.5 + 0.5;
    shade = sqrt( pow(shade * 0.8, 5.5) );
	float4 color = lerp(_Color1, _Color2, fbm(float3(i.uv, _Time.y * _ColorSpeed)) * 0.5 + 0.5);;
	color.a = shade*_A;	
	float4 pic = lerp(color, _Color, _Mix);
	//pic.a = pic.a*_A;
	//pic.a = 1;
   	fixed4 col = lerp(_BGColor, pic, c);
    
    // Some grain
    col -= (1.0 - s) * t * 0.04;

  col.rgb = ((col.rgb - 0.5f) * max(_Contrast, 0)) + 0.5f;
  col.rgb += _Brightness;
  // Return final pixel color.
  col.rgb *= col.a;
	
    
	//fragColor = float4(col, 1.0);
	return col;
}

            ENDCG
        }
    }
}
