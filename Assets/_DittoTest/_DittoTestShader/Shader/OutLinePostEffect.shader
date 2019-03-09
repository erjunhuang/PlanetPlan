// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutLinePostEffect" {
 
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BlurTex("Blur", 2D) = "white"{}

		_DistortTexture("DistortTexture",2D)="white"{}
		_DistortColor("DistortColor",Color) = (1,1,1,1)
		_DistortBump ("DistortBump", 2D) = "bump" {}
		_BumpAmt("BumpAmt",Range(0,128))=10
	}
 
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	sampler2D _BlurTex;
	float4 _BlurTex_ST;
	float4 _BlurTex_TexelSize;
	float4 _offsets;

	sampler2D _DistortTexture;
	float4 _DistortTexture_ST;
	sampler2D _DistortBump;
	float4 _DistortBump_ST;
	float4 _DistortColor;
	float _BumpAmt;


	//0:高斯模糊 vert shader（之前的文章有详细注释，此处也可以用BoxBlur，更省一点）

		//用于模糊
	struct v2f_blur
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
	};

	v2f_blur vert_blur(appdata_img v)
	{
		v2f_blur o;
		_offsets *= _MainTex_TexelSize.xyxy;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
 
		o.uv01 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1);
		o.uv23 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
		o.uv45 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 3.0;
 
		return o;
	}
 
	//高斯模糊 pixel shader
	fixed4 frag_blur(v2f_blur i) : SV_Target
	{
		fixed4 color = fixed4(0,0,0,0);
		color += 0.40 * tex2D(_MainTex, i.uv);
		color += 0.15 * tex2D(_MainTex, i.uv01.xy);
		color += 0.15 * tex2D(_MainTex, i.uv01.zw);
		color += 0.10 * tex2D(_MainTex, i.uv23.xy);
		color += 0.10 * tex2D(_MainTex, i.uv23.zw);
		color += 0.05 * tex2D(_MainTex, i.uv45.xy);
		color += 0.05 * tex2D(_MainTex, i.uv45.zw);
		return color;
	}
	
	//1:Blur图和原图进行相减获得轮廓
	
	//用于剔除中心留下轮廓
	struct v2f_cull
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f_cull vert_cull(appdata_img v)
	{
		v2f_cull o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		//dx中纹理从左上角为初始坐标，需要反向
		#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv.y = 1 - o.uv.y;
		#endif	
		return o;
	}
 
	fixed4 frag_cull(v2f_cull i) : SV_Target
	{
		fixed4 colorMain = tex2D(_MainTex, i.uv);
		fixed4 colorBlur = tex2D(_BlurTex, i.uv);
		//最后的颜色是_BlurTex - _MainTex，周围0-0=0，黑色；边框部分为描边颜色-0=描边颜色；中间部分为描边颜色-描边颜色=0。最终输出只有边框
		//return fixed4((colorBlur - colorMain).rgb, 1);
		return colorBlur - colorMain;
	}
	 
	//2:叠加
	struct v2f_add
	{
		float4 vertex : SV_POSITION;
		float2 uv  : TEXCOORD0;
	};
	 
	v2f_add vert_add (appdata_img v){
		v2f_add o ;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		return o;
	}

	half4 frag_add(v2f_add v):COLOR{
		fixed4 scene = tex2D(_MainTex, v.uv);
		fixed4 blur = tex2D(_BlurTex, v.uv);
		//fixed4 distort = tex2D(_DistortTexture, v.uv);
		return scene+blur;
	}

	//3:扭曲
	struct appdata{
		float2 uv : TEXCOORD0;
		float4 vertex :POSITION;
	};
	struct v2f{
		float4 vertex :SV_POSITION;
		//float2 uvmain : TEXCOORD0;
		float2 uvbump : TEXCOORD1;
		float4 uvgrab:TEXCOORD2;
	};
	 
	v2f vert_Distort (appdata v){
		v2f o ;
		o.vertex = UnityObjectToClipPos(v.vertex);
				 
		o.uvgrab.xy = (float2(o.vertex.x,o.vertex.y)+o.vertex.w)*0.5;
		o.uvgrab.zw = o.vertex.zw;
		#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				o.uvgrab.y = 1 - o.uvgrab.y;
		#endif

		//o.uvmain = TRANSFORM_TEX(v.uv,_DistortTexture);
		o.uvbump = TRANSFORM_TEX(v.uv,_DistortBump);
		return o;
	}

	half4 frag_Distort(v2f v):COLOR{
		half2 bump = UnpackNormal(tex2D(_DistortBump, v.uvbump+float2(0 , _Time.y * 0.05))).rg;
		float2 offset = bump * _BumpAmt * _MainTex_TexelSize.xy;
		v.uvgrab.xy = offset * v.uvgrab.z + v.uvgrab.xy;

		half4 col  = tex2Dproj(_MainTex,UNITY_PROJ_COORD(v.uvgrab));
		//half4 tint = tex2D(_DistortTexture,v.uvmain)*_DistortColor;

		return col;
	}
 
	ENDCG
 
	SubShader
	{
		//pass 0: 高斯模糊
		Pass
		{
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }
 
			CGPROGRAM
			#pragma vertex vert_blur
			#pragma fragment frag_blur
			ENDCG
		}

		
		//pass 1: 剔除中心部分 
		Pass
		{
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }
 
			CGPROGRAM
			#pragma vertex vert_cull
			#pragma fragment frag_cull
			ENDCG
		}

 
		//pass 2: 最终叠加
		Pass
		{
 
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }
 
			CGPROGRAM
			#pragma vertex vert_add
			#pragma fragment frag_add
			ENDCG
		}

		//pass 3: 扭曲
		Pass
		{
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }
 
			CGPROGRAM
			#pragma vertex vert_Distort
			#pragma fragment frag_Distort
			ENDCG
		}
 
	}
}