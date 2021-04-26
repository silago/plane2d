Shader "Unlit/Nebula3"
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

            struct v2f {
                float2 uv : TEXCOORD0;
            };



            sampler2D _MainTex;
            float4 _MainTex_ST;

             v2f vert (
                float4 vertex : POSITION, // vertex position input
                float2 uv : TEXCOORD0, // texture coordinate input
                out float4 outpos : SV_POSITION // clip space position output
                )
            {
                v2f o;
                o.uv = uv;
                outpos = UnityObjectToClipPos(vertex);
                return o;
            }
            // Hacked up version of https://www.shadertoy.com/view/MsVXWW


#define R(p, a) p=cos(a)*p+sin(a)*float2(p.y, -p.x)

//=====================================
// otaviogood's noise from https://www.shadertoy.com/view/ld2SzK
//--------------------------------------------------------------
// This spiral noise works by successively adding and rotating sin waves while increasing frequency.
// It should work the same on all computers since it's not based on a hash function like some other noises.
// It can be much faster than other noise functions if you're ok with some repetition.
static const float nudge = 0.739513;	// size of perpendicular vector
static float normalizer = 1.0 / sqrt(1.0 + nudge*nudge);	// pythagorean theorem on that perpendicular to maintain scale
float SpiralNoiseC(float3 p){
    float n = 0.0;	// noise amount
    float iter = 1.0;
    for (int i = 0; i < 6; i++){
        // add sin and cos scaled inverse with the frequency
        n += -abs(sin(p.y*iter) + cos(p.x*iter)) / iter;	// abs for a ridged look
        // rotate by adding perpendicular and scaling down
        p.xy += float2(p.y, -p.x) * nudge;
        p.xy *= normalizer;
        // rotate on other axis
        p.xz += float2(p.z, -p.x) * nudge;
        p.xz *= normalizer;
        // increase the frequency
        iter *= 1.733733;
    }
    return n;
}

float SpiralNoise3D(float3 p){
    float n = 0.0;
    float iter = 1.0;
    for (int i = 0; i < 5; i++){
        n += (sin(p.y*iter) + cos(p.x*iter)) / iter;
        p.xz += float2(p.z, -p.x) * nudge;
        p.xz *= normalizer;
        iter *= 1.33733;
    }
    return n;
}

float NebulaNoise(float3 p){
   float final = p.y + 4.5;
    final -= SpiralNoiseC(p.xyz); // mid-range noise
    final += SpiralNoiseC(p.zxy*0.5123 + 100.0)*4.0; // large scale features
    final -= SpiralNoise3D(p); // more large scale features, but 3d

    return final;
}

float map(float3 p){
	R(p.xz, _Time.y*0.4);
    
    float r = length(p);
    float star = r + 0.5;
    float noise = 1.0 + pow(abs(NebulaNoise(p/0.5)*0.5), 1.5);
    return clamp(star, noise, smoothstep(0.45, 1.5, r) - smoothstep(2.0, 3.0, r));
}

bool RaySphereIntersect(float3 org, float3 dir, out float near, out float far){
	float b = dot(dir, org);
	float c = dot(org, org) - 8.;
	float delta = b*b - c;
	if(delta < 0.0) return false;
	float deltasqrt = sqrt(delta);
	near = -b - deltasqrt;
	far = -b + deltasqrt;
	return far > 0.0;
}

static const float3 starColor = float3(1.0, 0.5, 0.25);
static const float2 iMouse = 1;
          

fixed4 frag (v2f i , UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {
//void mainImage( out float4 fragColor, in float2 fragCoord){
	// ro: ray origin
	// rd: direction of the ray
    float4 gl_FragCoord = screenPos; 
    float2 iResolution = float2(1,1);                
	float3 rd = normalize(float3((gl_FragCoord.xy-0.5*iResolution.xy)/iResolution.y, 1.0));
	float3 ro = float3(0.0, 0.0, -4.0);
	
    const float rot = 0.01;
    R(rd.yz, -iMouse.y*rot);
    R(rd.xz,  iMouse.x*rot);
    R(ro.yz, -iMouse.y*rot);
    R(ro.xz,  iMouse.x*rot);
	
    int steps = 0;
    const int max_steps = 64;
    const float max_advance = 1.0;
    
    float t = 0.0;
	float4 sum = 0.0;
   
    float min_dist=0.0, max_dist=0.0;
    if(RaySphereIntersect(ro, rd, min_dist, max_dist)){
        float dither = 0.5 - 1.5*tex2D(_MainTex, gl_FragCoord.xy/256.0).r;
        t = min_dist + max_advance*dither;

        for(int i = 0; i < max_steps; i++){
            if(sum.a > 0.95 || t > max_dist) break;
            
            float3 pos = ro + t*rd;
            float dist = map(pos);
			float advance = clamp(0.05*dist, 0.01, max_advance);
            
            float density = max(1.2 - dist, 0.0);
            float3 emit = starColor*(110.0*advance*density/dot(pos, pos));
            float block = 1.0 - pow(0.05, density*advance/0.05);
            sum += (1.0 - sum.a)*float4(emit, block);

            t += advance;
            steps = i;
        }

	}
	
//    fragColor = float4(float3(smoothstep(min_dist, max_dist, t)), 1.0); return;
//    fragColor = float4(float3(sum.a), 1.0); return;
//    fragColor = float4(float3(float(steps)/float(max_steps)), 1.0); return;
    
    sum.rgb = pow(sum.rgb, 2.2);
    sum.rgb = sum.rgb/(1.0 + sum.rgb);
    return float4(sum.xyz,1.0);
    //fragColor = float4(sum.xyz,1.0);
}

            //fixed4 frag (v2f i) : SV_Target
            //{
            //    // sample the texture
            //    fixed4 col = tex2D(_MainTex, i.uv);
            //    // apply fog
            //    UNITY_APPLY_FOG(i.fogCoord, col);
            //    return col;
            //}
            ENDCG
        }
    }
}
