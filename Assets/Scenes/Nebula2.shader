Shader "Unlit/Nebula2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

    #define MAX_STEPS 128
	#define STEP_SIZE 0.025
	#define _FoV 55.0

	#define JITTER

	#define mod(x,y) (x-y*floor(x/y))
	//#define ROTATE

	static  const float3 _StarColor = float3(0.7,0.8,1.0);
	static  const float _Absorption = 0.1;
	static  const float _Scattering = 0.5;
	static  const float _Density = 4.0;
	static  const float _Radius = 1.0;

//#define ROTATE


//Ray-sphere intersection
bool raycastSphere(float3 ro, float3 rd, out float3 p0, out float3 p1, float3 center, float r)
{
    float A = 1.0; //dot(rd, rd);
    float B = 2.0 * (rd.x * (ro.x - center.x) + rd.y * (ro.y - center.y) + rd.z * (ro.z - center.z));
    float C = dot(ro - center, ro - center) - (r * r);

    float D = B * B - 4.0 * A * C;
    if (D < 0.0)
    {
        return false;
    }
    else
    {
        float t0 = (-B - D)/(2.0 * A);
        float t1 = (-B + D)/(2.0 * A);
        p0 = ro + rd * t0;
        p1 = ro + rd * t1;
        return true;
    }
}

float3 rotateY(float3 p, float t)
{
    float cosTheta = cos(t);
    float sinTheta = sin(t);
    float3x3 rot = float3x3(cosTheta, 0.0, sinTheta,
        			0.0, 1.0, 0.0,
    			    -sinTheta, 0.0, cosTheta);
    
    return mul(rot,p);
}

float3 rotateZ(float3 p, float t)
{
    float cosTheta = cos(t);
    float sinTheta = sin(t);
    float3x3 rot = float3x3(cosTheta, -sinTheta, 0.0,
                    sinTheta, cosTheta, 0.0,
                    0.0, 0.0, 1.0);
    
    return mul(rot ,p);
}

//iq's lut based value noise
float noise(float3 x)
{
    float3 p = floor(x);
    float3 f = frac(x);
	f = f*f*(3.0-2.0*f);
	float2 uv = (p.xy+float2(37.0,17.0)*p.z) + f.xy;
	//float2 rg = tex2Dlod( _MainTex, float4(uv+0.5)/256.0, 0.0).yx;
	float2 rg = tex2Dlod( _MainTex,float4( (uv+0.5)/256.0, 0.0,0)).yx;
    float result = lerp( rg.x, rg.y, f.z );
    result = (2.0 * result) - 1.0;
	return result;
}

//Brownian pink noise
float fbm(float3 seed, int octaves, float freq, float lac)
{
    float val;
    float j = 1.0;
    for (int i = 0; i < octaves; i++, j+=1.0)
    {
        val += noise(seed * freq * j) / pow(j, lac);
    }

    return val;
}

float sdSphere(float3 pos, float3 center, float radius)
{
    pos.z *= 0.9; //Prolate spheroid
    //pos += noise(pos * 12.0) * 0.05;
    return length(center-pos) - radius;
}

float sampleVolume(float3 pos)
{
    float d = sdSphere(pos, 0.0, 0.5);
    float sphere = max(0.0, 0.3 - abs(d));
    
    float ring = max(0.0, (0.4-abs(pos.z)));
    
    if (ring <= 0.0 && sphere <= 0.0)
        return 0.0;
    
    ring *= max(0.2,fbm(40.0+pos, 12, 4.0, 0.75)) * 6.0;
    float r = length(pos);
    ring *= 1.0-smoothstep(0.5,0.2,r);
    float3 n = normalize(-pos);
    ring *= smoothstep(0.1,1.0,(0.5 + abs(noise(n)*0.1))/r);
    float n2 = abs(fbm(100.0+pos, 3, 2.0, 2.0) * 10.0)*pow(r,32.0);
    ring = max(0.0, ring - n2 /max(0.01,abs(pos.x)));
    ring *= abs(pos.x * pos.x) - abs(pos.y) * 0.2 + 0.5;
        
    float result = sphere + ring;
    return result * _Density;
}

float sampleProplyds(float3 pos)
{
    float result = smoothstep(0.99,0.999,abs(fbm(20.0+pos, 2, 24.0, 2.0)));
    float d = abs(sdSphere(pos, 0.0, 0.4));
    result *= max(0.0,0.05-d) * 800.0;
    result *= max(0.0, 0.3-abs(pos.z));
    return result;
}

//Raymarching loop
float4 raymarch(float3 pos, float3 dir, float ds, int s)
{
    float4 result = float4(0.,0.0,0.0,1.0);
    int steps = min(s, MAX_STEPS);
    for (int i = 0; i < steps; i++)
    {
        float p = sampleVolume(pos);       
        float r = length(pos);
        float3 em = lerp(float3(0.0, 0.25, 1.0), float3(0.9, 1.0, 0.1), r/_Radius-0.075);
        float ext = max(0.000001, (_Absorption + _Scattering) * p);
        float trans = exp(-ext * ds);
        em.r += pow(r, 6.0) * 4.0;
        
        float3 lum = em * p;
        float3 integral = (lum - (lum * trans))/ext;
        
        result.rgb += integral * result.a;
        result.a *= trans;
        
        float p2 = sampleProplyds(pos);
        result.rgb -= p2 * ds;
        result.a *= exp(-p2*ds);
        
        if (result.a <= 0.01)
            return result;
        

        pos += dir * ds;
    }
        
    return result;
}

//4x4 Bayer matrix for ordered dithering
static const float4x4 _Bayer4x4 = float4x4(float4(0,0.5,0.125,0.625),
                        float4(0.75,0.25,0.875,.375), 
                        float4(0.1875,0.6875,0.0625,0.5625), 
                        float4(0.9375,0.4375,0.8125,0.3125));
fixed4 frag(v2f i ) : SV_Target
//void mainImage( out float4 fragColor, in float2 fragCoord )
{
	float2 iMouse = 0;
    float zoom = 1;//iMouse.y/iResolution.y;
    float3 rayOrigin = float3(0.0, 0.0, -5.0 + (5.0 * zoom));
    //Compute eye vector from field of vie1w
    float2 uv = i.uv;
    float ar = 1;//iResolution.x/iResolution.y;
    float d = ar/tan(radians(_FoV/2.0));
    float2 nuv = (-1.0 + 2.0 * uv) * float2(ar, 1.0);
    float3 rayDir = normalize(float3(nuv, d));
    
    #ifdef ROTATE
    float t = iMouse.x * 0.01 + iTime * 0.2;
	#else
    float t = iMouse.x * 0.01;
    #endif
    
    rayDir = rotateY(rayDir, t);
    rayOrigin = rotateY(rayOrigin, t);
    
    float zRot = radians(40.0);
    rayDir = rotateZ(rayDir, zRot);
    rayOrigin = rotateZ(rayOrigin, zRot);
    
    float4 col = float4(0.0,0.0,0.0,1.0);
    
    //Background
    float starfield = fbm(rayDir, 2, 200.0, 1.0);
    starfield = smoothstep(0.7,1.45,starfield);
    col.rgb += starfield * starfield * lerp(float3(1.0,0.8,0.2),float3(0.0,0.5,1.0),starfield*starfield);
    
    float3 p0, p1;
    if (raycastSphere(rayOrigin, rayDir, p0, p1, 0.0, _Radius))
    {        
        #ifdef JITTER
        //Bayer matrix ordered depth jittering
        float width = (uv.x);// * iResolution.x);
        float height = (uv.y);// * iResolution.y);
        width = mod(width, 4.0);
        height = mod(height, 4.0);
        float offset = _Bayer4x4[int(width)*_Time.y][int(height)*_Time.y];
        p0 -= rayDir * offset * STEP_SIZE;
        #endif
        
        float dist = length(p1 - p0);
        int s = int(dist/STEP_SIZE) + 1;
        
        float4 integral = raymarch(p0, rayDir, STEP_SIZE, s);
        
        col.rgb = lerp(integral.rgb, col.rgb, integral.a);
        
        //Central star
    	float star = 0.0001/dot(nuv, nuv);
        star *= 2.0/dot(rayOrigin, rayOrigin);
    	col.rgb += (1.0-exp(-star)) *  _StarColor;
    }	
    return col;
    //fragColor = col;
}
            ENDCG
        }
    }
}
