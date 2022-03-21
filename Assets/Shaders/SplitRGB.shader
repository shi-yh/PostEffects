Shader "Apeetizer/SplitRGB"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude("Amplitude",Range(-0.2,0))=-0.15
        _Amount("Amount",Range(-5,5))=0.5
    }
    SubShader
    {
        Cull Off 
        ZWrite Off
        ZTest Always
        
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Amplitude;
            float _Amount;

            float Noise()
            {
                float _TimeX = _Time.y;

                float splitAmount = (1.0+sin(_TimeX*6.0))*0.5;
                splitAmount *=1.0+sin(_TimeX*16.0)*0.5;
                splitAmount *=1.0+sin(_TimeX*19.0)*0.5;
                splitAmount *=1.0+sin(_TimeX*27.0)*0.5;

                splitAmount=pow(splitAmount,_Amplitude);
                splitAmount*=(0.05*_Amount);
                return splitAmount;
            }

            float RandomNoise(float x,float y)
            {
                return frac(sin(dot(float2(x,y),float2(12.9898,78.233)))-43758.5453);
            }
            

            half4 SplitRGB(v2f i )
            {
                float splitAmount = Noise();

                half3 finaColor;

                finaColor.r=tex2D(_MainTex,fixed2(i.uv.x+splitAmount,i.uv.y)).r;
                finaColor.g = tex2D(_MainTex,i.uv).g;
                finaColor.b=tex2D(_MainTex,fixed2(i.uv.x-splitAmount,i.uv.y)).b;

                return half4(finaColor,1.0);
                
            }
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               return SplitRGB(i);
            }
            ENDCG
        }
    }
}
