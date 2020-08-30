Shader "Avatar_Vis/Vertex_Color"
{
		Properties
		{
		}

		SubShader
		{
				Tags
				{
						"RenderType" = "Opaque"
				}
				CGPROGRAM
				#pragma surface surf Lambert vertex:vert
				struct Input
				{
						float3 customColor;
				};

				void vert(inout appdata_full v, out Input o)
				{
						UNITY_INITIALIZE_OUTPUT(Input, o);
						//o.customColor = abs(v.normal);
						o.customColor = v.color;
				}

				void surf(Input IN, inout SurfaceOutput o)
				{
						o.Albedo = IN.customColor;
				}
				ENDCG
		}

		FallBack "Diffuse"
}