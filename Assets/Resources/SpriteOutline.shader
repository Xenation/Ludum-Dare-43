Shader "Custom/SpriteOutline" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor ("OutlineColor", Color) = (1, 0, 0, 1)
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjectors"="True"
		}
		LOD 100

		Pass {
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;
			uniform float4 _OutlineColor;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				// sample the texture
				float alpha = tex2D(_MainTex, i.uv).a;
				if (alpha < 0.01) {
					float2 down = float2(i.uv.x, i.uv.y - _MainTex_TexelSize.y);
					float2 up = float2(i.uv.x, i.uv.y + _MainTex_TexelSize.y);
					float2 right = float2(i.uv.x + _MainTex_TexelSize.x, i.uv.y);
					float2 left = float2(i.uv.x - _MainTex_TexelSize.x, i.uv.y);
					float alphaSum = tex2D(_MainTex, down).a + tex2D(_MainTex, up).a + tex2D(_MainTex, right).a + tex2D(_MainTex, left).a;
					if (alphaSum != 0) {
						return _OutlineColor;
					}
				}
				return fixed4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
}
