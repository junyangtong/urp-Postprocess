using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TiltShiftBlurPass : ScriptableRenderPass
{
     // 用于Shader中使用的Uniform变量
    static class ShaderIDs
    {
        internal static readonly int TiltShiftBlurPassParams = Shader.PropertyToID("_TiltShiftBlurPassParams");
        internal static readonly int TiltShiftBlurPassParams2 = Shader.PropertyToID("_TiltShiftBlurPassParams2");
    }
    // 用于FrameDebugger或其他Profiler中显示的名字
    private const string m_ProfilerTag = "Tilt Shift Blur Pass";

    // 后处理配置类
    //private RenderTargetIdentifier m_Source;
    //private RenderTargetIdentifier m_Destination;
    private TiltShiftBlur m_TiltShiftBlur;

    //移轴效果材质
    //private Material m_Material;


    public TiltShiftBlurPass(RenderPassEvent evt) 
    { 
        // 设置Pass的执行顺序 
        renderPassEvent = evt; 
    } 

    /*public void Setup(RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material)
    {
        m_Source = source;
        m_Destination = destination;
        m_Material = material;
    }

    /*public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        
    }*/

    // 重写Execute 在此执行渲染逻辑
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) 
    {
        // 获取后处理配置
        var stack = VolumeManager.instance.stack;// Gets the current state of the VolumeComponent of the specified type in the stack.
        m_TiltShiftBlur = stack.GetComponent<TiltShiftBlur>();

        CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
        
        // 当效果激活时才执行逻辑
        bool active = m_TiltShiftBlur.IsActive();
        if (active)
        {
            //将参数设置到uniform变量
            //cmd.SetGlobalTexture("_MainTex", m_Source);
            cmd.SetGlobalVector(ShaderIDs.TiltShiftBlurPassParams, new Vector2(m_TiltShiftBlur.Offset.value, m_TiltShiftBlur.Area.value));
            cmd.SetGlobalVector(ShaderIDs.TiltShiftBlurPassParams2, new Vector2( m_TiltShiftBlur.Spread.value,m_TiltShiftBlur.BlurInt.value));
            // 开启故障宏
            cmd.EnableShaderKeyword("_TILTSHIFTBLUR");}
        else
        {
            // 关闭故障宏
            cmd.DisableShaderKeyword("_TILTSHIFTBLUR");
        }
            /*//执行逻辑
                // 计算模糊
                for (int i = 0; i < m_TiltShiftBlur.BlurStep.value; i++)
                {
                    cmd.Blit(m_Destination,m_Source);
                    cmd.Blit(m_Source, m_Destination, m_Material);
                } */
            //cmd.Blit(m_Source, m_Destination, m_Material); //.Identifier():实例化rt Identifies a RenderTexture for a CommandBuffer.
            context.ExecuteCommandBuffer(cmd);  // Schedules the execution of a custom graphics Command Buffer.
            CommandBufferPool.Release(cmd);
            
    }

}