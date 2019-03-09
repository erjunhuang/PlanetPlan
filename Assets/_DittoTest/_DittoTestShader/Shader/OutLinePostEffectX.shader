// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//后处理描边Shader
//by：puppet_master
//2017.1.12
 
Shader "Custom/OutLinePostEffectX" {
 
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BlurTex("Blur", 2D) = "white"{}
		_OriTex("Ori", 2D) = "white"{}
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	
 
	//用于模糊
	struct v2f_blur
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
	};
 
	//用于最后叠加
	struct v2f_add
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
	};
 
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _BlurTex;
	float4 _BlurTex_TexelSize;
	sampler2D _OriTex;
	float4 _OriTex_TexelSize;
	float4 _offsets;
	fixed4 _OutlineColor;
 
	//高斯模糊 vert shader（之前的文章有详细注释，此处也可以用BoxBlur，更省一点）
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
 
	//最终叠加 vertex shader
	v2f_add vert_add(appdata_img v)
	{
		v2f_add o;
		//mvp矩阵变换
		o.pos = UnityObjectToClipPos(v.vertex);
		//uv坐标传递
		o.uv.xy = v.texcoord.xy;
		o.uv1.xy = o.uv.xy;
		o.uv2.xy = o.uv.xy;
#if UNITY_UV_STARTS_AT_TOP
		if  (_OriTex_TexelSize.y < 0){
		o.uv.y = 1 - o.uv.y;
			o.uv2.y = 1 - o.uv2.y;
		}
#endif	

		return o;
	}
 
	fixed4 frag_add(v2f_add i) : SV_Target
	{
		//取原始场景纹理进行采样
		fixed4 scene = tex2D(_MainTex, i.uv1);
		//return scene;
		//对blur后的纹理进行采样
		fixed4 blur = tex2D(_BlurTex, i.uv);
		//对blur之前的rt进行采样
		fixed4 ori = tex2D(_OriTex, i.uv);
		//轮廓是_BlurTex - _OriTex，周围0-0=0，黑色；边框部分为描边颜色-0=描边颜色；中间部分为描边颜色-描边颜色=0。最终输出只有边框
		fixed4 outline = blur - ori;
		
		//输出：blur部分为0的地方返回原始图像，否则为0，然后叠加描边
		fixed4 final = scene * (1 - all(outline.rgb)) + _OutlineColor * any(outline.rgb);//0.01,1,1
		return final;
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
 
		//pass 1: 剔除中心部分以及最后和场景图叠加
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
 
	}
}
