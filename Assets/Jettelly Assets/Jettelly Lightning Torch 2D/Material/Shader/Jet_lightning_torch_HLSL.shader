
Shader "Jettelly/BIRP/Lightning Torch/Torch"
{
    Properties
    {       
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _fScale ("Flame Scale", Range(1., 10.)) = 4.
        [HideInInspector] _fThreshold ("Noise Threshold", Range(0.1, 1.0)) = 0.3
        [HideInInspector] _fSpeed ("Noise Speed", Range(0.0, 5.0)) = 1.5
        [HideInInspector] _fVolume ("Volume", Range(0.0, 0.5)) = 0.5

        [HideInInspector] _cBottom ("Color Bottom", Color) = (1, 1, 1, 1)
        [HideInInspector] _cMiddle ("Color Middle", Color) = (1, 1, 1, 1)
        [HideInInspector] _cTop ("Color Top", Color) = (1, 1, 1, 1)

        [HideInInspector] _vOffset("V Offset", Range(0.0, 0.5)) = 0.15
        [HideInInspector] _uOffset ("U Offset", Range(0.0, 0.5)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/JettellyCommon/CGinc/JettellyCommon.cginc"
            #include "Assets/JettellyCommon/CGinc/ShaderGraphNodes.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _fScale;
            float _fThreshold;
            float _fSpeed;
            float _vOffset;
            float _fVolume;

            float4 _cBottom;
            float4 _cMiddle;
            float4 _cTop;

            float _uOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }          

            float capsule (float2 p, float r1, float r2, float h)
            {          
                p.y = p.y - 0.5;
                p.x = abs(p.x);
                float b = (r1 - r2) / h;
                float a = sqrt(1.0 - b * b);
                float k = dot(p, float2(-b, a));
                if(k < 0.0) return length(p) - r1;
                if(k > a * h) return length(p - float2(0.0, h)) - r2;

                return dot(p, float2(a, b)) - r1;
            }          

            half4 frag(v2f i) : SV_Target
            {
                i.uv.x -= _uOffset;
                half uvy = i.uv.y - (_Time.y * _fSpeed);
                half n = Unity_GradientNoise(float2(i.uv.x, uvy), _fScale);
                n *= 1 - i.uv.y;    
                
                i.uv.y = i.uv.y + _vOffset;
                half c1 = capsule(i.uv, _fVolume * 0.35, _fVolume * 0.15, _vOffset * 2);
                half c2 = smoothstep(c1 - 0.05, c1 + 0.05, 0.1);
                n *= c2;    
                
                half s = smoothstep(n + 0.05, n - 0.05, _fThreshold);
                half g = (1 - i.uv.y) * s;
                s += g;
                
                half4 render = 0;
                render = Jettelly_ThreeColorGradient(_cTop, _cMiddle, _cBottom, i.uv.y);
                render *= s;
                render *= i.color;
                render = saturate(render);
                
                return render;
            }
            ENDCG
        }
    }
}
