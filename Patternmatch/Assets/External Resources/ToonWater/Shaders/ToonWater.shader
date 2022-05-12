// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Roystan/Toon/Water"
{
    Properties
    {	
		_DetailColor("Detail Color", Color) = (0.325, 0.807, 0.971, 0.725)
		_FoamColor("Foam Color", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1
        
        _FoamNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}	
        
        _SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777
        _SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27
        
        _FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
        _FoamMinDistance("Foam Minimum Distance", Float) = 0.04

        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
		_WaveOffsetParams("Wave Magnitude Parameters", Vector) = (0.0004, 0.1, 10, 10)
    }
    SubShader
    {
        Pass
        {
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define SMOOTHSTEP_AA 0.03
			#define GRADIENT_START 0.35
			#define PI 3.14159265359

            #include "UnityCG.cginc"

			#pragma multi_compile_fog

			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float4 _DetailColor;
			float4 _FoamColor;

			float _DepthMaxDistance;
            float _SurfaceNoiseCutoff;

            float _FoamMaxDistance;
            float _FoamMinDistance;

            float _SurfaceDistortionAmount;

            float2 _SurfaceNoiseScroll;

            sampler2D _FoamNoise;
            float4 _FoamNoise_ST;

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;

            sampler2D _CameraDepthNormalsTexture;

			// Wave params
			float4 _WaveOffsetParams;
			//////////////

			// Global Shader values
			uniform float2 _BendAmount;
			uniform float3 _BendOrigin;
			uniform float _BendFalloff;

			float4 Curve(float4 v)
			{
				//HACK: Considerably reduce amount of Bend
				_BendAmount *= .0001;

				float4 world = mul(unity_ObjectToWorld, v);

				float dist = length(world.xz-_BendOrigin.xz);

				dist = max(0, dist-_BendFalloff);

				// Distance squared
				dist = dist*dist;

				world.xy += dist*_BendAmount;
				return mul(unity_WorldToObject, world);
			}

            float4 alphaBlend(float4 top, float4 bottom)
            {
                float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
                float alpha = top.a + bottom.a * (1 - top.a);

                return float4(color, alpha);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 noiseUV : TEXCOORD0;
                float2 distortUV : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
                float2 foamUV : TEXCOORD3;
                float3 viewNormal : NORMAL;

				UNITY_FOG_COORDS(4)
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
                o.foamUV = TRANSFORM_TEX(v.uv, _FoamNoise);
				
				float3 worldPos = Curve(v.vertex);//mul(unity_ObjectToWorld, v.vertex);
				float2 offset = float2(
					sin(_Time.y) * _WaveOffsetParams.x
					, sin(_Time.y + worldPos.z * _WaveOffsetParams.w + worldPos.x * _WaveOffsetParams.z) * _WaveOffsetParams.y);

				v.vertex.xy += offset;
                o.vertex = UnityObjectToClipPos(worldPos);

				o.screenPos = ComputeScreenPos(o.vertex);
                o.viewNormal = COMPUTE_VIEW_NORMAL;
                
				UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 depthNormal = tex2Dproj(_CameraDepthNormalsTexture, UNITY_PROJ_COORD(i.screenPos));
                
                float3 cameraNorm;
                float cameraDepth;

                DecodeDepthNormal(depthNormal, cameraDepth, cameraNorm);

				float existingDepthLinear = cameraDepth * _ProjectionParams.z;// LinearEyeDepth(cameraDepth);

				float depthDiff = existingDepthLinear - i.screenPos.w;

				float waterDepthDiff01 = saturate(depthDiff / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDiff01);

                float3 normalDot = saturate(dot(cameraNorm, i.viewNormal));
				float upDot = saturate(dot(cameraNorm, float3(0, 1, 0)));

                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);

                float foamDepthDifference01 = saturate(depthDiff / foamDistance);
                float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;
                
                float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;

                float2 noiseUV = float2(i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x + distortSample.x
                                        , i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y + distortSample.y);

				float2 foamUV = float2(i.foamUV.x + _Time.y * _SurfaceNoiseScroll.x + distortSample.x
										, i.foamUV.y + _Time.y * _SurfaceNoiseScroll.y + distortSample.y);

                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).a;
				float foamNoiseSample = tex2D(_FoamNoise, foamUV).g;

                float surfaceNoise = surfaceNoiseSample;//smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);

				float threshold = clamp(foamNoiseSample, 0, 1);
				float foam = step(foamDepthDifference01, threshold)
					* step(upDot, 0.67);

				foam *= GRADIENT_START + 2 * (threshold - foamDepthDifference01);


				waterColor.rgb += float3(_DetailColor.r * surfaceNoise * _DetailColor.a
					, _DetailColor.g * surfaceNoise * _DetailColor.a
					, _DetailColor.b * surfaceNoise * _DetailColor.a);
				
				float4 col = alphaBlend(float4(_FoamColor.r, _FoamColor.g, _FoamColor.b, foam * _FoamColor.a), waterColor);

				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
            }
            ENDCG
        }
    }
}