using UnityEngine; 
using UnityEngine.Rendering; 
using UnityEngine.Rendering.Universal; 

public class GlitchImageBlockPass : ScriptableRenderPass
{   
    // 用于Shader中使用的Uniform变量
    static class ShaderIDs
    {
        internal static readonly int Params = Shader.PropertyToID("_Params");
        internal static readonly int Params2 = Shader.PropertyToID("_Params2");
        internal static readonly int Params3 = Shader.PropertyToID("_Params3");
    }

    // 用于FrameDebugger或其他Profiler中显示的名字
    private const string m_ProfilerTag = "Glitch Image Block";

    // 后处理配置类
    private RenderTargetIdentifier m_Source;
    private RenderTargetIdentifier m_Destination;
    private Glitch m_Glitch;

    //故障效果材质
    private Material m_Material;

    private float TimeX = 1.0f;

    public GlitchImageBlockPass(RenderPassEvent evt) 
    { 
        // 设置Pass的执行顺序 
        renderPassEvent = evt; 
    } 

    public void Setup(RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material)
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
        m_Glitch = stack.GetComponent<Glitch>();

        // 当效果激活时才执行逻辑
        bool active = m_Glitch.IsActive();
        if (active)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            
            //将参数设置到uniform变量
            cmd.SetGlobalTexture("_MainTex", m_Source);
            cmd.SetGlobalVector(ShaderIDs.Params, new Vector3(TimeX * m_Glitch.Speed.value, m_Glitch.Amount.value, m_Glitch.Fade.value));
            cmd.SetGlobalVector(ShaderIDs.Params2, new Vector4(m_Glitch.BlockLayer1_U.value, m_Glitch.BlockLayer1_V.value, m_Glitch.BlockLayer2_U.value, m_Glitch.BlockLayer2_V.value));
            cmd.SetGlobalVector(ShaderIDs.Params3, new Vector3(m_Glitch.RGBSplitIndensity.value, m_Glitch.BlockLayer1_Indensity.value, m_Glitch.BlockLayer2_Indensity.value));
            
            //执行逻辑
            cmd.Blit(m_Source, m_Destination, m_Material); //.Identifier():实例化rt Identifies a RenderTexture for a CommandBuffer.
            context.ExecuteCommandBuffer(cmd);  // Schedules the execution of a custom graphics Command Buffer.
            CommandBufferPool.Release(cmd);
        }    
    }

}