Shader "Ditto/Outline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)

		_OutLineWidth("OutlineWidth",Range(0,10))=1.1

		_DistortTexture("DistortTexture",2D)="white"{}
		_DistortColor("DistortColor",Color) = (1,1,1,1)
		_DistortBump ("DistortBump", 2D) = "bump" {}
		_BumpAmt("BumpAmt",Range(0,128))=10

		_BlurRadius("_BlurRadius",Range(0,10))=1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }

		GrabPass{}
		Pass{
			Name "OUTLINEDISTORT"
			ZWrite off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog

			#pragma multi_compile_instancing
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

			float _OutLineWidth;
			sampler2D _DistortTexture;
			float4 _DistortTexture_ST;
			sampler2D _DistortBump;
			float4 _DistortBump_ST;
			float4 _DistortColor;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _BumpAmt;
			v2f vert (appdata v){
				//v.vertex.xyz*=_OutLineWidth;
				v.vertex.xyz += normalize(v.normal) * _OutLineWidth;
				v2f o ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				 
				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale =1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x,o.vertex.y*scale)+o.vertex.w)*0.5;
				o.uvgrab.zw = o.vertex.zw;

				o.uvmain = TRANSFORM_TEX(v.uv,_DistortTexture);
				o.uvbump = TRANSFORM_TEX(v.uv,_DistortBump);
				return o;
			}

			half4 frag(v2f v):COLOR{

				half2 bump = UnpackNormal(tex2D(_DistortBump, v.uvbump)).rg;
				float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
				v.uvgrab.xy = offset * v.uvgrab.z + v.uvgrab.xy;

				half4 col  = tex2Dproj(_GrabTexture,UNITY_PROJ_COORD(v.uvgrab));
				half4 tint = tex2D(_DistortTexture,v.uvmain)*_DistortColor;
				return col*tint;
			}
			ENDCG
		}

		GrabPass{}
		Pass{
			Name "OUTLINEHORIZONTALBLUR"
			ZWrite off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog

			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			struct appdata{
				float2 uv : TEXCOORD0;
				float4 vertex :POSITION;
				float3 normal:NORMAL;
			};
			struct v2f{
				float4 vertex :SV_POSITION;
				float4 uvgrab:TEXCOORD0;
			};

			float _OutLineWidth;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _BumpAmt;
			float _BlurRadius;
			v2f vert (appdata v){
				//v.vertex.xyz*=+_OutLineWidth;
				v.vertex.xyz += normalize(v.normal) * _OutLineWidth;
				v2f o ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				 
				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale =1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x,o.vertex.y*scale)+o.vertex.w)*0.5;
				o.uvgrab.zw = o.vertex.zw;

				return o;
			}
			half4 GRABPIXEL(float kernelx,float weight , float4 uvgrab){
				half4 col =  tex2Dproj(_GrabTexture,UNITY_PROJ_COORD(float4(uvgrab.x+_GrabTexture_TexelSize.x*kernelx*_BlurRadius,uvgrab.y,uvgrab.z,uvgrab.w)))*weight;
				return col;
			}
			half4 frag(v2f v):COLOR{

				half4 col  = GRABPIXEL(0,1,v.uvgrab);

				half4 texsum = half4(0,0,0,0);
				texsum +=GRABPIXEL(-4,0.05,v.uvgrab);
				texsum +=GRABPIXEL(-3,0.09,v.uvgrab);
				texsum +=GRABPIXEL(-2,0.12,v.uvgrab);
				texsum +=GRABPIXEL(-1,0.15,v.uvgrab);
				texsum +=GRABPIXEL(0,0.18,v.uvgrab);
				texsum +=GRABPIXEL(1,0.15,v.uvgrab);
				texsum +=GRABPIXEL(2,0.12,v.uvgrab);
				texsum +=GRABPIXEL(3,0.09,v.uvgrab);
				texsum +=GRABPIXEL(4,0.05,v.uvgrab);

				col = lerp(col,texsum,1);
				return col;
			}
			ENDCG
		}

		GrabPass{}
		Pass{
			Name "OUTLINEVERTICALBLUR"
			ZWrite off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog

			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			struct appdata{
				float2 uv : TEXCOORD0;
				float4 vertex :POSITION;
				float3 normal:NORMAL;
			};
			struct v2f{
				float4 vertex :SV_POSITION;
				float4 uvgrab:TEXCOORD0;
			};

			float _OutLineWidth;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _BumpAmt;
			float _BlurRadius;
			v2f vert (appdata v){
				//v.vertex.xyz*=+_OutLineWidth;
				v.vertex.xyz += normalize(v.normal) * _OutLineWidth;
				v2f o ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				 
				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale =1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x,o.vertex.y*scale)+o.vertex.w)*0.5;
				o.uvgrab.zw = o.vertex.zw;

				return o;
			}
			half4 GRABPIXEL(float kernely,float weight , float4 uvgrab){
				half4 col =  tex2Dproj(_GrabTexture,UNITY_PROJ_COORD(float4(uvgrab.x,uvgrab.y+_GrabTexture_TexelSize.y*kernely*_BlurRadius,uvgrab.z,uvgrab.w)))*weight;
				return col;
			}
			half4 frag(v2f v):COLOR{

				half4 col  = GRABPIXEL(0,1,v.uvgrab);

				half4 texsum = half4(0,0,0,0);
				texsum +=GRABPIXEL(-4,0.05,v.uvgrab);
				texsum +=GRABPIXEL(-3,0.09,v.uvgrab);
				texsum +=GRABPIXEL(-2,0.12,v.uvgrab);
				texsum +=GRABPIXEL(-1,0.15,v.uvgrab);
				texsum +=GRABPIXEL(0,0.18,v.uvgrab);
				texsum +=GRABPIXEL(1,0.15,v.uvgrab);
				texsum +=GRABPIXEL(2,0.12,v.uvgrab);
				texsum +=GRABPIXEL(3,0.09,v.uvgrab);
				texsum +=GRABPIXEL(4,0.05,v.uvgrab);

				col = lerp(col,texsum,1);
				return col;
			}
			ENDCG
		}

		Pass
		{	
			Name "OBJECT"

			ZWrite On 

			Material{
				Diffuse[_Color]
				Ambient[_Color]
			}
			Lighting On

			SetTexture[_MainTex]{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]{
				Combine previous * primary DOUBLE
			}
		}
	}
}
