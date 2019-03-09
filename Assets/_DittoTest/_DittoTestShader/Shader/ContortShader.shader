Shader "Ditto/ContortShader"
{
	Properties
	{	
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DistortTexture("DistortTexture",2D)="white"{}
		_DistortColor("DistortColor",Color) = (1,1,1,1)
		_DistortBump ("DistortBump", 2D) = "bump" {}
		_BumpAmt("BumpAmt",Range(0,128))=10

		_OriTex("OriTex", 2D) = "white" {}
		 
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		Pass{
			Name "CONTORTSHADER"
			ZWrite off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct appdata{
				float2 uv : TEXCOORD0;
				float4 vertex :POSITION;
				float3 normal:NORMAL;
			};
			struct v2f{
				float4 vertex :SV_POSITION;
				float2 uvmain : TEXCOORD0;
				float2 uvbump : TEXCOORD1;
				float4 uvgrab:TEXCOORD2;
			};
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _MainTex_ST;
			sampler2D _DistortTexture;
			float4 _DistortTexture_ST;
			sampler2D _DistortBump;
			float4 _DistortBump_ST;
			float4 _DistortColor;
			float _BumpAmt;
			v2f vert (appdata v){
				v2f o ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				 
				o.uvgrab.xy = (float2(o.vertex.x,o.vertex.y)+o.vertex.w)*0.5;
				o.uvgrab.zw = o.vertex.zw;
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uvgrab.y = 1 - o.uvgrab.y;
				#endif

				o.uvmain = TRANSFORM_TEX(v.uv,_DistortTexture);
				o.uvbump = TRANSFORM_TEX(v.uv,_DistortBump);
				return o;
			}

			half4 frag(v2f v):COLOR{
				half2 bump = UnpackNormal(tex2D(_DistortBump, v.uvbump+float2(0 , _Time.y * 0.05))).rg;
				float2 offset = bump * _BumpAmt * _MainTex_TexelSize.xy;
				v.uvgrab.xy = offset * v.uvgrab.z + v.uvgrab.xy;

				half4 col  = tex2Dproj(_MainTex,UNITY_PROJ_COORD(v.uvgrab));
				half4 tint = tex2D(_DistortTexture,v.uvmain)*_DistortColor;
				return col*tint;
			}
			ENDCG
		}
	}
}
