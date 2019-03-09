Shader "Ditto/DissolveEffect"
{
	Properties
	{
		_Diffuse("Diffuse", Color) = (1,1,1,1)
		_MainTex("Base 2D", 2D) = "white"{}
		_DissolveMap("DissolveMap", 2D) = "white"{}
		_DissolveThreshold("DissolveThreshold", Range(0,1)) = 0
		_ColorFactor("ColorFactor", Range(0,1)) = 0.7
		_DissolveEdge("DissolveEdge", Range(0,1)) = 0.8
		_DissolveColor("DissolveColor", Color) = (0,0,0,0)
		_DissolveEdgeColor("DissolveEdgeColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct v2f
			{
				float4 vertex : POSITION;
				float3 worldNormal : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			uniform fixed4 _Diffuse;
			uniform sampler2D _MainTex;
			uniform sampler2D _DissolveMap;
			uniform float4 _MainTex_ST;
			uniform float _DissolveThreshold;
			uniform float _ColorFactor;
			uniform float _DissolveEdge;
			uniform fixed4 _DissolveColor;
			uniform fixed4 _DissolveEdgeColor;

			
			v2f vert (appdata_base v)
			{		
				v.vertex.xyz += v.normal * _DissolveThreshold * 0.5;
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 dissolveValue = tex2D(_DissolveMap, i.uv);
				if(dissolveValue.r<_DissolveThreshold){
					discard;
				}
				//Diffuse + Ambient光照计算
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 lambert = saturate(dot(worldNormal, worldLightDir))*0.5+0.5;
				fixed3 albedo = lambert * _Diffuse.xyz * _LightColor0.xyz + UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 color = tex2D(_MainTex, i.uv).rgb * albedo;

				//优化版本，尽量不在shader中用分支判断的版本,但是代码很难理解啊....
				float percentage = _DissolveThreshold / dissolveValue.r;
				//如果当前百分比 - 颜色权重 - 边缘颜色
				float lerpEdge = sign(percentage - _ColorFactor - _DissolveEdge);
				//貌似sign返回的值还得saturate一下，否则是一个很奇怪的值
				fixed3 edgeColor = lerp(_DissolveEdgeColor.rgb, _DissolveColor.rgb, saturate(lerpEdge));
				//最终输出颜色的lerp值
				float lerpOut = sign(percentage - _ColorFactor);
				//最终颜色在原颜色和上一步计算的颜色之间差值（其实经过saturate（sign（..））的lerpOut应该只能是0或1）
				fixed3 colorOut = lerp(color, edgeColor, saturate(lerpOut));
				return fixed4(colorOut, 1);
			}
			ENDCG
		}
	}
}
