Shader "TiltShiftBlur"
{
    SubShader
   {
      Cull Off 
      ZWrite Off 
      
      Pass
      {
         HLSLPROGRAM
         
         #pragma vertex Vert
         #pragma fragment Frag

         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
   

         half4 _TiltShiftBlurPassParams;


         #define _Offset _TiltShiftBlurPassParams.x
         #define _Area _TiltShiftBlurPassParams.y
         #define _Spread _TiltShiftBlurPassParams.z
         #define _BlurInt _TiltShiftBlurPassParams.w
         
        Texture2D _MainTex;
        SamplerState sampler_MainTex;
        
        float TiltShiftMask(float2 uv)
        {
            float centerY = uv.y * 2.0 - 1.0 + _Offset; // [0,1] -> [-1,1]
            return pow(abs(centerY * _Area), _Spread);
        }
        
        float4 GaussianSample (float2 uv)
            {
                // 1 / 16
                float offset = _BlurInt * 0.0625f;

                // 左上
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x - offset,uv.y - offset)) * 0.0947416f;
                // 上
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,float2(uv.x,uv.y - offset)) * 0.118318f;
                // 右上
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,float2(uv.x + offset,uv.y + offset)) * 0.0947416f;
                // 左
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,float2(uv.x - offset,uv.y)) * 0.118318f;
                // 中
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,float2(uv.x,uv.y)) * 0.147761f;
                // 右
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,float2(uv.x + offset,uv.y)) * 0.118318f;
                // 左下
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x - offset,uv.y + offset)) * 0.0947416f;
                // 下
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,float2(uv.x,uv.y + offset)) * 0.118318f;
                // 右下
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,float2(uv.x + offset,uv.y - offset)) * 0.0947416f;

                return color;
            }

        float4 Frag(Varyings i): SV_Target
        {
            float2 uv = i.uv;
            float mask = TiltShiftMask(uv);
            float3 src = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb; 
            float3 dest = GaussianSample(uv); 
            float3 col = lerp(src,dest,mask);

            return float4(col,1.0);
        }

        ENDHLSL
         
      }
   }
}
 