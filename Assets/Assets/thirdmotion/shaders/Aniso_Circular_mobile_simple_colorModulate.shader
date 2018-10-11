Shader "TM/mobile/Aniso Circular Mobile ColorMod" 
{
	Properties 
	{
		_MainTex ("Brightness(R), Spec Map(G), SecondaryColor(B), Alpha (A)", 2D) = "white" {}
		_SecondTex ("Root Color(B), Tip Color(R), Punk Color(G)", 2D) = "white" {}
		
		_Color ("Base Hair Color", Color) = (1,1,1,1)
		_ColorSecondary("Secondary Hair Color", Color ) = (1,1,1,1)
		_ColorRoots ("Roots Hair Color", Color) = (1,1,1,1)
		_ColorTips ("Tips Color", Color) = (1,1,1,1)
		_ColorPunk ("Punk Extra Color", Color)= (1,1,1,1)
		
		_SpecularMultiplier ("Specular Multiplier", float) = 1.0
		_SpecularColor ("Specular Color", Color) = (1,1,1,1)
		_AnisoOffset ("Anisotropic Highlight Offset", Range(-1,1)) = 0.0
		_Cutoff ("Alpha Cut-Off Threshold", float) = 0.9
		_Gloss ( "Gloss Multiplier", float) = 128.0
	}
	
	SubShader
	{	
		//changing this to TransparentCutout gave us shadow collecting ( using transparent for better sorting, since we have alpha below that kills collection anyway )
		Tags { "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		
		//MAIN HAIR PASS
		AlphaTest Greater [_Cutoff]
		Blend off
		Cull Back
		ZWrite on
		
		CGPROGRAM
		#pragma surface surf Aniso alphatest:_Cutoff approxview exclude_path:prepass halfasview nolightmap  
		//noforwardadd
			  
			struct Input
			{
				fixed2 uv_MainTex;
			};

			sampler2D _MainTex, _SecondTex;
			fixed _AnisoOffset, _SpecularMultiplier, _Gloss;
			fixed3 _SpecularColor, _Color, _ColorSecondary, _ColorRoots, _ColorTips, _ColorPunk;
			
			struct SurfaceOutputAniso 
			{
			    half3 Albedo;
			    half3 Normal;
			    half3 Emission;
			    half Specular;
			    half Gloss;
			    half Alpha;

				fixed3 BaseTex;
				fixed3 SecondTex;
			};
			
			void surf (Input IN, inout SurfaceOutputAniso o)
			{
				fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex); 
				o.Albedo = _Color.rgb * albedo.r;
				o.Alpha = albedo.a;
				
				o.BaseTex = albedo.rgb;
				
				fixed3 secondary = tex2D(_SecondTex, IN.uv_MainTex); 
				o.SecondTex = secondary;
			}

			inline fixed4 LightingAniso (SurfaceOutputAniso s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				fixed NdotL = dot(s.Normal, lightDir);
				
				fixed aniso = max(0, sin(radians((NdotL + _AnisoOffset) * 180)));
				aniso = saturate(pow( aniso, _Gloss));
				aniso = aniso * _SpecularMultiplier * s.BaseTex.g;
				
				fixed4 c;
				c.rgb = lerp( _Color, _ColorSecondary, s.BaseTex.b ).rgb; 
				c.rgb = lerp( c.rgb, _ColorRoots, s.SecondTex.b ).rgb;
				c.rgb = lerp( c.rgb, _ColorTips, s.SecondTex.r).rgb;
				c.rgb = lerp( c.rgb, _ColorPunk, s.SecondTex.g).rgb;
				c.rgb = c.rgb * s.BaseTex.r;
				
				c.rgb = ((c.rgb * _LightColor0.rgb ) + (_LightColor0.rgb * aniso * _SpecularColor )) * ( NdotL * atten * 2);
				c.a = s.Alpha;
				
				return c;
			}
		ENDCG
				
		//TRANSPARENT SECTION
		AlphaTest LEqual [_Cutoff]
		//Blend SrcAlpha OneMinusSrcAlpha 
		Cull Back
		ZWrite off
		
		CGPROGRAM
		#pragma surface surf Aniso alpha approxview nolightmap exclude_path:prepass halfasview 
		//alpha kills shadow collection, can use clip + blend instead, but clip is not very mobile friendly, so just sticking to no shadow collection
		//noforwardadd
			
			struct Input
			{
				fixed2 uv_MainTex;
			};

			sampler2D _MainTex, _SecondTex;
			fixed _AnisoOffset, _SpecularMultiplier, _Gloss;
			fixed3 _SpecularColor, _Color, _ColorSecondary, _ColorRoots, _ColorTips, _ColorPunk;
			
			struct SurfaceOutputAniso 
			{
			    half3 Albedo;
			    half3 Normal;
			    half3 Emission;
			    half Specular;
			    half Gloss;
			    half Alpha;
				
				fixed3 BaseTex;
				fixed3 SecondTex;
			};
			
			void surf (Input IN, inout SurfaceOutputAniso o)
			{
				fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex); 
				o.Albedo = _Color.rgb * albedo.r;
				o.Alpha = albedo.a;
				
				//clip ( _Cutoff  - o.Alpha );
				
				o.BaseTex = albedo.rgb;
				
				fixed3 secondary = tex2D(_SecondTex, IN.uv_MainTex); 
				o.SecondTex = secondary;
				
				o.Albedo = lerp( o.Albedo.rgb , _ColorSecondary.rgb, albedo.b );
			}

			inline fixed4 LightingAniso (SurfaceOutputAniso s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				fixed NdotL = saturate(dot(s.Normal, lightDir));
				
				fixed aniso = max(0, sin(radians((NdotL + _AnisoOffset) * 180)));
				aniso = saturate(pow( aniso, _Gloss));
				aniso = aniso * _SpecularMultiplier * s.BaseTex.g;
				
				fixed4 c;
				c.rgb = lerp( _Color, _ColorSecondary, s.BaseTex.b ).rgb; 
				c.rgb = lerp( c.rgb, _ColorRoots, s.SecondTex.b ).rgb;
				c.rgb = lerp( c.rgb, _ColorTips, s.SecondTex.r).rgb;
				c.rgb = lerp( c.rgb, _ColorPunk, s.SecondTex.g).rgb;
				c.rgb = c.rgb * s.BaseTex.r;
				
				c.rgb = ((c.rgb * _LightColor0.rgb ) + (_LightColor0.rgb * aniso * _SpecularColor )) * ( NdotL * atten * 2);
				c.a = s.Alpha;
				
				return c;
			}
		ENDCG
	}
	FallBack "Transparent/Cutout/VertexLit"
}