Shader "Outer Wilds/Effects/Reality Shadow" {
	Properties {
	}
	SubShader {
		LOD 100
		Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+401" "RenderType" = "Transparent" }
		GrabPass {
			"_FinalGrabTex"
		}
		Pass {
			LOD 100
			Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+401" "RenderType" = "Transparent" }
			ZWrite Off
			Cull Off
			Offset 1, 1
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float4 texcoord : TEXCOORD0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _FinalGrabTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                tmp0 = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                o.position = tmp0;
                tmp0.y = tmp0.y * _ProjectionParams.x;
                tmp1.xzw = tmp0.xwy * float3(0.5, 0.5, 0.5);
                o.texcoord.zw = tmp0.zw;
                o.texcoord.xy = tmp1.zz + tmp1.xw;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0.xy = inp.texcoord.xy / inp.texcoord.ww;
                tmp0 = tex2D(_FinalGrabTex, tmp0.xy);
                tmp0.xyz = max(tmp0.xyz, float3(0.0, 0.0, 0.0));
                o.sv_target.w = tmp0.w;
                tmp0.xyz = log(tmp0.xyz);
                tmp0.xyz = tmp0.xyz * float3(0.4166667, 0.4166667, 0.4166667);
                tmp0.xyz = exp(tmp0.xyz);
                tmp0.xyz = tmp0.xyz * float3(1.055, 1.055, 1.055) + float3(-0.055, -0.055, -0.055);
                tmp0.xyz = max(tmp0.xyz, float3(0.0, 0.0, 0.0));
                tmp0.xyz = float3(1.0, 1.0, 1.0) - tmp0.xyz;
                tmp1.xyz = tmp0.xyz * float3(0.305306, 0.305306, 0.305306) + float3(0.6821711, 0.6821711, 0.6821711);
                tmp1.xyz = tmp0.xyz * tmp1.xyz + float3(0.0125229, 0.0125229, 0.0125229);
                tmp0.xyz = tmp0.xyz * tmp1.xyz;
                tmp0.xyz = tmp0.xyz * tmp0.xyz;
                tmp0.xyz = tmp0.xyz * tmp0.xyz;
                o.sv_target.xyz = tmp0.xyz * tmp0.xyz;
                return o;
			}
			ENDCG
		}
	}
}