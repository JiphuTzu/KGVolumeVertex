// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Ke/VolumeVertex_v2" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal", 2D) = "bump" {}
		_OcclusionMap ("Occlusion", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}

	CGINCLUDE

	#pragma surface surf Standard nolightmap addshadow vertex:vert
	#pragma multi_compile _ KEY_DEBUG
	#pragma target 4.0
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _OcclusionMap;
	half _Glossiness;
	half _Metallic;
	fixed4 _Color;

	fixed4 _DebugColor;
	float4 _TargetPos[100];
	float4 _TargetColor[100];
	int _TargetCount;
	half _AffectRange;
	float _Pow;

	struct Input {
		float2 uv_MainTex;
		float4 _cp;
		float4 _tp;
		float4 _cc;
	};

	float Get2PointDistance(float4 _p1,float4 _p2){
		return distance(_p1.xyz,_p2.xyz);
	}

	float GetPointVolumeIntensity(float _2PointDis,float _powIntensity){
		float _intensity = _2PointDis/_AffectRange;
		_intensity = 1-(clamp(pow(_intensity,_powIntensity),0,1));
		return _intensity;
	}


	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM


		void vert(inout appdata_full v,out Input o){

			UNITY_INITIALIZE_OUTPUT(Input,o);

			float4 _CurrentPos = mul(unity_ObjectToWorld, v.vertex);
			float4 _VolumeWorldPos = float4(1000, 1000, 1000, 1000);
			float4 _color = float4(1, 1, 1, 1);

			float _tempDis = 100000;
			for (int i = 0;i<_TargetCount;i++)
			{
				float _temp = Get2PointDistance(_CurrentPos, _TargetPos[i]);
				if (_temp < _tempDis)
				{
					_tempDis = _temp;
					_VolumeWorldPos = _TargetPos[i];
					_color = _TargetColor[i];
				}
			}

			o._cp = _CurrentPos;
			o._tp = _VolumeWorldPos;
			o._cc = _color;

			float _dis = Get2PointDistance(_CurrentPos,_VolumeWorldPos);
			float _intensity = GetPointVolumeIntensity(_dis, _Pow);

			float3 _newPos = lerp(v.vertex.xyz,mul(unity_WorldToObject,float4(_VolumeWorldPos.xyz,1)),_intensity);
			o.uv_MainTex = v.texcoord.xy;
			v.vertex = float4(_newPos,1);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float _dis = Get2PointDistance(IN._cp,IN._tp);
			float _intensity = GetPointVolumeIntensity(_dis, _Pow);
			if(_dis<_AffectRange)
				o.Emission = lerp(float4(0,0,0,0), IN._cc,_intensity);

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex));
			o.Occlusion  = tex2D(_OcclusionMap, IN.uv_MainTex).r;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
