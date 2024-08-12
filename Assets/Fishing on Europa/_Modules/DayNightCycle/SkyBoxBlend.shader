Shader "Skybox/NightDay"
{
    Properties
    {
        _Texture1("Sunrise", 2D) = "white" {}
        _Texture2("Day", 2D) = "white" {}
        _Texture3("Sunset", 2D) = "white" {}
        _Texture4("Night", 2D) = "white" {}
        _Blend("Blend", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;
            sampler2D _Texture4;
            float _Blend;

            v2f vert(appdata v)
            {
                v2f o;
                o.texcoord = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float2 ToRadialCoords(float3 coords)
            {
                float3 normalizedCoords = normalize(coords);
                float latitude = acos(normalizedCoords.y);
                float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                const float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
                return float2(0.5, 1.0) - sphereCoords;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 tc = ToRadialCoords(i.texcoord);
                fixed4 tex1 = tex2D(_Texture1, tc);
                fixed4 tex2 = tex2D(_Texture2, tc);
                fixed4 tex3 = tex2D(_Texture3, tc);
                fixed4 tex4 = tex2D(_Texture4, tc);

                if(_Blend <=0.25) return lerp(tex1, tex2, _Blend);
                else if(_Blend > 0.25 && _Blend <= 0.75) return lerp(tex2, tex3, _Blend);
                else return lerp(tex3, tex4, _Blend);

                
            }
            ENDCG
        }
    }
}