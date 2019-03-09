struct appdata{
	float4 vertex :POSITION;
};
struct v2f{
	float4 vertex :SV_POSITION;
};
v2f vert (appdata v)
{
	v.vertex.xyz *= OUTLINEWIDTH;
	v2f o ;
	o.vertex = UnityObjectToClipPos(v.vertex);
	return o;
}
half4 frag(v2f v):COLOR{
	return float4(1,1,0,0);
}