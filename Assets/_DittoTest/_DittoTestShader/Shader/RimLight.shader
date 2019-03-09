// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Ditto/RimLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Diffuse("Diffuse", Color) = (1,1,1,1)
		_RimColor("RimColor", Color) = (1,1,1,1)
		_RimPower("RimPower", Range(0.000001, 3.0)) = 0.1
		_RimMask ("RimMask", 2D) = "white" {}
		_RimSpeed("RimSpeed", Range(-10, 10)) = 1.0
		_Gloss("Gloss",Range(8,200)) = 10

		_OutlineCol("OutlineCol", Color) = (1,0,0,1)
		_OutlineFactor("OutlineFactor", Range(1,10)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent"  }
		LOD 100

		Pass
		{	
			//剔除正面，只渲染背面，对于大多数模型适用，不过如果需要背面的，就有问题了
			Cull Front
			//控制深度偏移，描边pass远离相机一些，防止与正常pass穿插
			Offset 1,1

			//关闭深度写入
			//ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			fixed4 _OutlineCol;
			float _OutlineFactor;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			
			v2f vert(appdata_full v)
			{	
				v.vertex*=_OutlineFactor;
				v2f o;
				//在vertex阶段，每个顶点按照法线的方向偏移一部分，不过这种会造成近大远小的透视问题
				//v.vertex.xyz += v.normal * _OutlineFactor;
				o.pos = UnityObjectToClipPos(v.vertex);
				//将法线方向转换到视空间
				float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				//将视空间法线xy坐标转化到投影空间，只有xy需要，z深度不需要了
				float2 offset = TransformViewToProjection(vnormal.xy);
				//在最终投影阶段输出进行偏移操作
				//o.pos.xy += offset * _OutlineFactor;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				//这个Pass直接输出描边颜色
				return _OutlineCol;
			}
			
			//使用vert函数和frag函数
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
		Pass
		{	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"


			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;

				float3 worldNormal : TEXCOORD1;
				float3 worldVireDir : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Diffuse;
			fixed4 _RimColor;
			float _RimPower;
			sampler2D _RimMask;
			float _RimSpeed;
			half _Gloss;
			
			v2f vert (appdata_base  v)
			{	
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				float3 worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
				o.worldVireDir = _WorldSpaceCameraPos.xyz-worldPos;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{	
				fixed3 ambient  = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 worldVireDir = normalize(i.worldVireDir);
                fixed3 halfDir = normalize(worldVireDir + worldLightDir);

				fixed3 lambert = max(dot(worldNormal,worldLightDir)*0.5+0.5,0);
				 
				fixed3 reflectDir = normalize(reflect(-worldLightDir,worldNormal));
				//fixed3 specular = pow(max(dot(reflectDir, worldVireDir), 0), _Gloss);
				fixed3 specular = pow(max(dot(worldNormal, halfDir), 0), _Gloss);

				fixed3 Diffuse = (lambert*_LightColor0.rgb*_Diffuse.rgb+ _LightColor0.rgb*specular+ambient);
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				 

				float rim = 1-max(0,dot(worldVireDir,worldNormal));
				fixed3 rimColor = _RimColor * pow(rim, 1 / _RimPower);

				fixed4 rimMask  = tex2D(_RimMask,i.uv+float2(0 , _Time.y * _RimSpeed));
				col.rgb = col.rgb*Diffuse+rimColor*(1-rimMask.r);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
