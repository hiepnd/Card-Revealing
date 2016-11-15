Shader "Screw/Texture Flat" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_BackTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry"}
		LOD 200
		
		Pass {
			Cull Back
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct VertexInput {
				half4 vertex : POSITION;
				half2 texcoord : TEXCOORD;
			}; 
			
			struct VertexOutput {
				half4 pos : SV_POSITION;
				half2 uv : TEXCOORD;
				
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Color;
			
			VertexOutput vert (VertexInput i) {
				VertexOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);
				return o;
			}
			
			half _GrayScaleFactor;
			half4 frag (VertexOutput i) : COLOR {
				half4 color = tex2D(_MainTex, i.uv);
				return color * _Color;
			}					
			
			ENDCG
		}

		Pass {
			Cull Front
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct VertexInput {
				half4 vertex : POSITION;
				half2 texcoord : TEXCOORD;
			}; 
			
			struct VertexOutput {
				half4 pos : SV_POSITION;
				half2 uv : TEXCOORD;
				
			};
			
			sampler2D _BackTex;
			float4 _BackTex_ST;
			half4 _Color;
			
			VertexOutput vert (VertexInput i) {
				VertexOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = TRANSFORM_TEX(i.texcoord, _BackTex);
				return o;
			}
			
			half _GrayScaleFactor;
			half4 frag (VertexOutput i) : COLOR {
//				i.uv.x = 1 - i.uv.x;
				half4 color = tex2D(_BackTex, i.uv);
				return color * _Color;
			}					
			
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}