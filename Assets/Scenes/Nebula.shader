Shader "ShaderMan/Nebula"
{
	Properties{
		_MainTex("MainTex",2D) = "white"{} 
		_StarColor("StarColor",Color) = (0.7,0.8,1.0)
		_Absorption("Absorption", Range(0,5)) = 0.1
		_Scattering("Scattering", Range(0,5)) = 0.5
		_Density("Density", Range(0,5)) = 4.0
		_Radius("Radius", Range(0,3)) = 1.0
		_Zoom("Zoom", Range(0,2)) = 0.0
		_StepSize("StepSize", Range(0,1)) = 0.025
		[ShowAsVector2] iMouse("Mouse", vector) = (0.0,0.0,0.0)
	}
	SubShader
	{
	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	Pass
	{
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	#define mod(x,y) (x-y*floor(x/y))

	struct v2f {
                fixed4 pos : SV_POSITION;
                fixed4 color : COLOR;
				fixed2 uv:TEXCOORD0;
    };	

	struct VertexInput {
		fixed4 vertex : POSITION;
		fixed2 uv:TEXCOORD0;
		fixed4 tangent : TANGENT;
		fixed3 normal : NORMAL;
		//VertexInput
	};
	struct VertexOutput {
		fixed4 pos : SV_POSITION;
		fixed2 uv:TEXCOORD0;
		//VertexOutput
	};
	sampler2D _MainTex; 

	
	VertexOutput vert (VertexInput v)
	{
		VertexOutput o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv = v.uv;
		//VertexFactory
		return o;
	}
    
    #define MAX_STEPS 128
    #define STEP_SIZE 1
	#define _FoV 55.0

	#define JITTER

	//#define ROTATE

	//static  fixed3 _StarColor = fixed3(0.7,0.8,1.0);
	fixed3 _StarColor;
	fixed _Absorption = 0.1;
	fixed _Scattering = 0.5;
	fixed _Density = 4.0;
	fixed _Radius = 1.0;
	fixed _Zoom;
	fixed3 iMouse;
	static fixed3 _StepSize;
	//static  fixed _Absorption = 0.1;
	//static  fixed _Scattering = 0.5;
	//static  fixed _Density = 4.0;
	//static  fixed _Radius = 1.0;

//#define ROTATE


//Ray-sphere intersection
bool raycastSphere(fixed3 ro, fixed3 rd, out fixed3 p0, out fixed3 p1, fixed3 center, fixed r)
{
    fixed A = 1.0; //dot(rd, rd);
    fixed B = 2.0 * (rd.x * (ro.x - center.x) + rd.y * (ro.y - center.y) + rd.z * (ro.z - center.z));
    fixed C = dot(ro - center, ro - center) - (r * r);

    fixed D = B * B - 4.0 * A * C;
    if (D < 0.0)
    {
        return false;
    }
    else
    {
        fixed t0 = (-B - D)/(2.0 * A);
        fixed t1 = (-B + D)/(2.0 * A);
        p0 = ro + rd * t0;
        p1 = ro + rd * t1;
        return true;
    }
}

//iq's lut based value noise
fixed noise(fixed3 x)
{
	x+=_Time.y;
    fixed3 p = floor(x);
    fixed3 f = frac(x);
	f = f*f*(3.0-2.0*f);
	fixed2 uv = (p.xy+fixed2(37.0,17.0)*p.z) + f.xy;
	//fixed2 rg = tex2Dlod( _MainTex, fixed4(uv+0.5)/256.0, 0.0).yx;
	fixed2 rg = tex2Dlod( _MainTex,fixed4( (uv+0.5)/256.0, 0.0,0)).yx;
    fixed result = lerp( rg.x, rg.y, f.z );
    result = (2.0 * result) - 1.0;
	return result;
}

//Brownian pink noise
fixed fbm(fixed3 seed, int octaves, fixed freq, fixed lac)
{
    fixed val;
    fixed j = 1.0;
    for (int i = 0; i < octaves; i++, j+=1.0)
    {
        val += noise(seed * freq * j) / pow(j, lac);
    }

    return val;
}

fixed sdSphere(fixed3 pos, fixed3 center, fixed radius)
{
    pos.z *= 0.7; //Prolate spheroid
    //pos += noise(pos * 12.0) * 0.05;
    return length(center-pos) - radius;
}

fixed sampleVolume(fixed3 pos)
{
    fixed d = sdSphere(pos, 0.0, 0.5);
    fixed sphere = max(0.0, 0.3 - abs(d));
    
    fixed ring = max(0.0, (0.4-abs(pos.z)));
    
    if (ring <= 0.0 && sphere <= 0.0)
        return 0.0;
    
    ring *= max(0.2,fbm(40.0+pos, 12, 4.0, 0.75)) * 6.0;
    fixed r = length(pos);
    ring *= 1.0-smoothstep(0.5,0.2,r);
    fixed3 n = normalize(-pos);
    ring *= smoothstep(0.1,1.0,(0.5 + abs(noise(n)*0.1))/r);
    fixed n2 = abs(fbm(100.0+pos, 3, 2.0, 2.0) * 10.0)*pow(r,32.0);
    ring = max(0.0, ring - n2 /max(0.01,abs(pos.x)));
    ring *= abs(pos.x * pos.x) - abs(pos.y) * 0.2 + 0.5;
        
    fixed result = sphere + ring;
    return result * _Density;
}

fixed sampleProplyds(fixed3 pos)
{
    fixed result = smoothstep(0.99,0.999,abs(fbm(20.0+pos, 2, 24.0, 2.0)));
    fixed d = abs(sdSphere(pos, 0.0, 0.4));
    result *= max(0.0,0.05-d) * 800.0;
    result *= max(0.0, 0.3-abs(pos.z));
    return result;
}

//Raymarching loop
fixed4 raymarch(fixed3 pos, fixed3 dir, fixed ds, int s)
{
    fixed4 result = fixed4(0.,0.0,0.0,1.0);
    int steps = min(s, MAX_STEPS);
    for (int i = 0; i < steps; i++)
    {
        fixed p = sampleVolume(pos);       
        fixed r = length(pos);
        fixed3 em = lerp(fixed3(0.0, 0.25, 1.0), fixed3(0.9, 1.0, 0.1), r/_Radius-0.075);
        fixed ext = max(0.000001, (_Absorption + _Scattering) * p);
        fixed trans = exp(-ext * ds);
        em.r += pow(r, 6.0) * 4.0;
        
        fixed3 lum = em * p;
        fixed3 integral = (lum - (lum * trans))/ext;
        
        result.rgb += integral * result.a;
        result.a *= trans;
        
        fixed p2 = sampleProplyds(pos);
        result.rgb -= p2 * ds;
        result.a *= exp(-p2*ds);
        
        if (result.a <= 0.01)
            return result;
        

        pos += dir * ds;
    }
        
    return result;
}

//4x4 Bayer matrix for ordered dithering
const fixed4x4 _Bayer4x4 = fixed4x4(fixed4(0,0.5,0.125,0.625),
                        fixed4(0.75,0.25,0.875,.375), 
                        fixed4(0.1875,0.6875,0.0625,0.5625), 
                        fixed4(0.9375,0.4375,0.8125,0.3125));
fixed4 frag(v2f i ) : SV_Target
{
    fixed3 rayOrigin = fixed3(0.0, 0.0, -5.0 + (5.0 * _Zoom));
    //Compute eye vector from field of view
    fixed2 uv = i.uv;
    fixed d = 1/tan(radians(_FoV/2.0));
    fixed2 nuv = (-1.0 + 2.0 * uv); 
    fixed3 rayDir = normalize(fixed3(nuv, d));
    fixed4 col = fixed4(0.0,0.0,0.0,1.0);
    
    fixed3 p0, p1;
    if (raycastSphere(rayOrigin, rayDir, p0, p1, 0.0, _Radius))
    {        
        fixed dist = 100;//length(p1 - p0)*5;
        int s = 10000;//int(dist/STEP_SIZE) + 1;
        fixed4 integral = raymarch(p0, rayDir, STEP_SIZE, s);
        col.rgb = lerp(integral.rgb, col.rgb, integral.a);
        //Central star
    	fixed star = 0.1/dot(nuv, nuv);
        star *= 2.0/dot(rayOrigin, rayOrigin);
    	col.rgb+= (1.0-exp(-star)) *  _StarColor;
    }	
    return col;
    //fragColor = col;
}
	

	

    
    

	ENDCG
	}
  }
}
