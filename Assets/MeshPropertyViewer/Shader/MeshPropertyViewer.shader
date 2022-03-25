Shader "Hidden/Debug/MeshPropertyViewer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_LineWidth("Line Width", Float) = 1.0
		_NormalColor("Normal Color", Color) = (1.0, 1.0, 0.0, 1)
		_TangentColor("Tangent Color", Color) = (1.0, 0.0, 1.0, 1)
    }
    SubShader
    {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+99" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

        Pass
        {
			Name "Color"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile __ _VERTEX_NORMAL _VERTEX_TANGENT _VERTEX_TEXTURE _VERTEX_UV0 _VERTEX_UV1 _VERTEX_UV2 _VERTEX_UV3 _VERTEX_UV4 _VERTEX_UV5 _VERTEX_UV6 _VERTEX_UV7
			#pragma multi_compile __ _TEXTURE_UV1 _TEXTURE_UV2 _TEXTURE_UV3 _TEXTURE_UV4 _TEXTURE_UV5 _TEXTURE_UV6 _TEXTURE_UV7
			#pragma multi_compile __ _COLOR_R _COLOR_G _COLOR_B _COLOR_A _COLOR_RGB

            #include "UnityCG.cginc"

            struct appdata
            {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 color : COLOR;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 uv3 : TEXCOORD3;
				float4 uv4 : TEXCOORD4;
				float4 uv5 : TEXCOORD5;
				float4 uv6 : TEXCOORD6;
				float4 uv7 : TEXCOORD7;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 color : COLOR0;
            };

            sampler2D _MainTex;
			float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.vertex = UnityObjectToClipPos(v.vertex);

#if _VERTEX_NORMAL
				o.color = v.normal * 0.5 + 0.5;
#elif _VERTEX_TANGENT
				o.color = v.tangent * 0.5 + 0.5;
#elif _VERTEX_TEXTURE
				float2 uv;
	#if _TEXTURE_UV1
				uv = v.uv1;
	#elif _TEXTURE_UV2
				uv = v.uv2;
	#elif _TEXTURE_UV3
				uv = v.uv3;
	#elif _TEXTURE_UV4
				uv = v.uv4;
	#elif _TEXTURE_UV5
				uv = v.uv5;
	#elif _TEXTURE_UV6
				uv = v.uv6;
	#elif _TEXTURE_UV7
				uv = v.uv7;
	#else
				uv = v.uv0;
	#endif
				o.uv.xy = TRANSFORM_TEX(uv, _MainTex);
#elif _VERTEX_UV0
				o.color = v.uv0;
#elif _VERTEX_UV1
				o.color = v.uv1;
#elif _VERTEX_UV2
				o.color = v.uv2;
#elif _VERTEX_UV3
				o.color = v.uv3;
#elif _VERTEX_UV4
				o.color = v.uv4;
#elif _VERTEX_UV5
				o.color = v.uv5;
#elif _VERTEX_UV6
				o.color = v.uv6;
#elif _VERTEX_UV7
				o.color = v.uv7;
#else
				o.color = v.color;
#endif
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
#if _VERTEX_TEXTURE
				fixed4 col = tex2D(_MainTex, i.uv);
#else
				fixed4 col = i.color;
#endif

#if _COLOR_R
				col = fixed4(col.rrr, 1);
#elif _COLOR_G
				col = fixed4(col.ggg, 1);
#elif _COLOR_B
				col = fixed4(col.bbb, 1);
#elif _COLOR_A
				col = fixed4(col.aaa, 1);
#elif _COLOR_RGB
				col = fixed4(col.rgb, 1);
#endif
                return col;
            }
            ENDCG
        }

		Pass
		{
			Name "Normal"

			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct v2g
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
			};

			float _LineWidth;
			float4 _NormalColor;

			v2g vert(appdata v)
			{
				v2g o;
				UNITY_INITIALIZE_OUTPUT(v2g, o);
				o.vertex = v.vertex;
				o.normal = v.normal;

				return o;
			}

			[maxvertexcount(6)]
			void geom(triangle v2g input[3], inout LineStream<g2f> outStream)
			{
				g2f o;

				for (int i = 0; i < 3; i++)
				{
					o.vertex = UnityObjectToClipPos(input[i].vertex);
					outStream.Append(o);

					float4 worldPosition = mul(unity_ObjectToWorld, input[i].vertex);
					float3 normalDir = UnityObjectToWorldNormal(input[i].normal);
					o.vertex = mul(UNITY_MATRIX_VP, float4(worldPosition.xyz + normalDir * _LineWidth, 1.0));
					outStream.Append(o);

					outStream.RestartStrip();
				}
			}

			fixed4 frag(g2f i) : SV_Target
			{
				return _NormalColor;
			}

			ENDCG
		}

		Pass
		{
			Name "Tangent"

			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
			};

			struct v2g
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
			};

			float _LineWidth;
			float4 _TangentColor;

			v2g vert(appdata v)
			{
				v2g o;
				UNITY_INITIALIZE_OUTPUT(v2g, o);
				o.vertex = v.vertex;
				o.tangent = v.tangent;

				return o;
			}

			[maxvertexcount(6)]
			void geom(triangle v2g input[3], inout LineStream<g2f> outStream)
			{
				g2f o;

				for (int i = 0; i < 3; i++)
				{
					o.vertex = UnityObjectToClipPos(input[i].vertex);
					outStream.Append(o);

					float4 worldPosition = mul(unity_ObjectToWorld, input[i].vertex);
					float3 tangentDir = UnityObjectToWorldNormal(input[i].tangent);
					o.vertex = mul(UNITY_MATRIX_VP, float4(worldPosition.xyz + tangentDir * _LineWidth, 1.0));
					outStream.Append(o);

					outStream.RestartStrip();
				}
			}

			fixed4 frag(g2f i) : SV_Target
			{
				return _TangentColor;
			}

			ENDCG
		}
    }
}
