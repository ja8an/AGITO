Shader "TM/mobile/Transparent Specular Reflective Mobile" 
{
	Properties 
	{
		_MainTex ("Diffuse(RGB) Alpha(A)", 2D) = "white" {}
		
		_SpecularMultiplier ("Specular Multiplier", float) = 1.0
		_SpecularColor ("Specular Tint", Color) = (1,1,1,1)
		_SpecularGloss ( "Gloss Multiplier", float) = 128.0
		
		_ReflectColor ("Reflection Tint", Color) = (1,1,1,0.5)
		_ReflectMultiplier ("Reflection Multiplier", float) = 1.0
		_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }	
		LOD 250
		
		Blend SrcAlpha OneMinusSrcAlpha 

		CGPROGRAM
		#pragma surface surf CustomSpec approxview noforwardadd exclude_path:prepass nolightmap 
				
			struct SurfaceOutputCustomSpec 
			{
				fixed3 Albedo;
				fixed3 Normal;
				fixed3 Emission;
				half Specular;
				fixed Gloss;
				fixed Alpha;
			};
			
			struct Input 
			{
				float2 uv_MainTex;
				float3 worldRefl;
				INTERNAL_DATA
			};
			
			sampler2D _MainTex;
			samplerCUBE _Cube;
			fixed4 _ReflectColor, _SpecularColor;
			float _ReflectMultiplier, _SpecularMultiplier, _SpecularGloss;
				
			void surf (Input IN, inout SurfaceOutputCustomSpec o)
			{			 
				fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = albedo.rgb;
				o.Alpha = albedo.a;
			
				o.Gloss = _SpecularGloss;
				o.Specular = _SpecularMultiplier;
								
				float3 worldRefl = WorldReflectionVector (IN, o.Normal);
				fixed4 reflcol = texCUBE (_Cube, worldRefl);
				o.Emission = reflcol.rgb * _ReflectColor.rgb * _ReflectMultiplier;
			}
						
			inline fixed4 LightingCustomSpec (SurfaceOutputCustomSpec s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				half3 h = normalize (lightDir + viewDir);
				half diff = max (0, dot (s.Normal, lightDir));
				float nh = max (0, dot (s.Normal, h));
				
				//float spec = pow (nh, 48.0);
				float spec = pow (nh, s.Specular*128.0) * s.Gloss;
				//spec = saturate(pow(spec, s.Gloss * s.Gloss) * s.Specular ) ;
				
				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * _SpecularColor) * (atten * 2);
				c.a = s.Alpha + spec * _LightColor0.rgb ;
				return c;
			}
		ENDCG
	} 
	FallBack "Transparent/VertexLit"
}
