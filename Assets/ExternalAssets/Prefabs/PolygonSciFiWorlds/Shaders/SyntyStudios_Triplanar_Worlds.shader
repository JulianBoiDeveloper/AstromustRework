Shader "Universal Render Pipeline/SyntyStudios/TriplanarGrime"
{
    Properties
    {
        [HDR] _Overlay("Overlay", 2D) = "white" {}
        [Range(0, 5.01)] _FallOff("FallOff", Float) = 1
        [Range(0, 6.24)] _Tiling("Tiling", Float) = 1
        _Emission("Emission", 2D) = "white" {}
        _Texture("Texture", 2D) = "white" {}
        [ColorUsage(false, true)] _EmissionColor("EmissionColor", Color) = (0, 0, 0, 0)
        [Range(0.5, 1.2)] _DirtAmount("DirtAmount", Float) = 1
        [Range(0, 1)] _SmoothnessSpec("Smoothness(Spec)", Float) = 0.2
        [Range(0, 1)] _Metallic("Metallic", Float) = 0
        _NormalMap("NormalMap", 2D) = "bump" {}
        [HideInInspector] _texcoord("", 2D) = "white" {}
        [HideInInspector] __dirty("", Int) = 1
        
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        Cull Back
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
                // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members uv_texcoord,worldPos,worldNormal)
                #pragma exclude_renderers d3d11
                #pragma shader_feature __ TRIPLANAR_GRIME

                #include "UnityCG.cginc"
                #include "Lighting.cginc"
                #include "UnityStandardBRDF.cginc"

                #define UNITY_PBS_USE_BRDF1

                sampler2D _Overlay;
                float _FallOff;
                float _Tiling;
                sampler2D _Emission;
                sampler2D _Texture;
                float4 _EmissionColor;
                float _DirtAmount;
                float _SmoothnessSpec;
                float _Metallic;
                sampler2D _NormalMap;

                struct SurfaceOutputStandard
                {
                    fixed3 Albedo;      // Base color.
                    fixed3 Normal;      // Normal direction.
                    fixed3 Emission;    // Emission color.
                    fixed Metallic;     // Metallic value.
                    fixed Smoothness;   // Smoothness value.
                    fixed Alpha;        // Transparency value.
                };

                struct v2f
                {
                    float2 uv_texcoord;
                    float3 worldPos;
                    float3 worldNormal;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };
                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord : TEXCOORD0;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.uv_texcoord = v.texcoord;
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    return o;
                }

                fixed4 TriplanarSampling1(sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index)
                {
                    float3 projNormal = pow(abs(worldNormal), falloff);
                    projNormal /= (projNormal.x + projNormal.y + projNormal.z) + 0.00001;
                    float3 nsign = sign(worldNormal);
                    float negProjNormalY = max(0, projNormal.y * -nsign.y);
                    projNormal.y = max(0, projNormal.y * nsign.y);
                    half4 xNorm;
                    half4 yNorm;
                    half4 yNormN;
                    half4 zNorm;
                    xNorm = tex2D(midTexMap, tiling * worldPos.zy * float2(nsign.x, 1.0));
                    yNorm = tex2D(topTexMap, tiling * worldPos.xz * float2(nsign.y, 1.0));
                    yNormN = tex2D(botTexMap, tiling * worldPos.xz * float2(nsign.y, 1.0));
                    zNorm = tex2D(midTexMap, tiling * worldPos.xy * float2(-nsign.z, 1.0));
                    return fixed4(0, 0, 0, 0);
                }

                void surf(v2f i, inout SurfaceOutputStandard o)
                {
                    float2 uv_NormalMap = i.uv_texcoord * UNITY_MATRIX_TEXTURE0[0].xy + UNITY_MATRIX_TEXTURE0[0].zw;
                    o.Normal = UnpackNormal(tex2D(_NormalMap, uv_NormalMap));
                    float2 uv_Texture = i.uv_texcoord;
                    float3 ase_worldPos = i.worldPos;
                    float3 ase_worldNormal = normalize(i.worldNormal);
                    fixed4 triplanar1 = TriplanarSampling1(_Overlay, _Overlay, _Overlay, ase_worldPos, ase_worldNormal, _FallOff, _Tiling, float3(1, 1, 1), float3(0, 0, 0));
                    fixed4 temp_cast_0 = float4(_DirtAmount, _DirtAmount, _DirtAmount, _DirtAmount);
                    fixed4 clampResult17 = clamp(triplanar1, temp_cast_0, float4(1, 1, 1, 0));
                    o.Albedo = tex2D(_Texture, uv_Texture) * clampResult17.rgb;
                    float2 uv_Emission = i.uv_texcoord;
                    o.Emission = tex2D(_Emission, uv_Emission) * _EmissionColor.rgb;
                    o.Metallic = _Metallic;
                    o.Smoothness = _SmoothnessSpec;
                    o.Alpha = 1;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    SurfaceOutputStandard o;
                    surf(IN, o);
                    fixed4 c = LightingStandard(IN.worldPos, o.Normal, o.Albedo, o.Specular, o.Smoothness, o.Alpha);
                    c.rgb += o.Emission;
                    return c;
                }
            ENDCG
        }
    }
}