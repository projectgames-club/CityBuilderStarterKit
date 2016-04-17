Shader "Custom/DesaturationAndBrightnessWithColor" {


Properties{
	_Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = ""
}


Subshader {

	Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

    ZWrite Off

    Blend SrcAlpha OneMinusSrcAlpha

    Pass {

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
			
        CGPROGRAM

        #pragma vertex vert

        #pragma fragment frag

        struct v2f {

            float4 position : SV_POSITION;

            float2 uv_mainTex : TEXCOORD;
            
            float4 color : COLOR ;

        }; 


        uniform float4 _MainTex_ST;
		
        v2f vert(float4 position : POSITION, float2 uv : TEXCOORD0, float4 color: COLOR) {

            v2f o;

            o.position = mul(UNITY_MATRIX_MVP, position);

            o.uv_mainTex = uv * _MainTex_ST.xy + _MainTex_ST.zw;
            
            o.color = color;

            return o;

        }

        

        uniform sampler2D _MainTex;
        uniform fixed4 _Color;


 		fixed4 frag(v2f input) : COLOR {
            fixed4 mainTex = tex2D(_MainTex, input.uv_mainTex);
            fixed4 mainColor = _Color;
            fixed4 fragColor;
            fixed4 tempColor;
            tempColor.rgb = dot(mainTex.rgb, fixed3(.222, .707, .071));
			fragColor = ((mainColor.r * mainTex) + ((1 - mainColor.r) * tempColor))/ ( 2 - mainColor.g); 
            fragColor.a = mainTex.a;
            return fragColor;
        }

        ENDCG

    }

}

 

}