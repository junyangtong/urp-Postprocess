using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TiltShiftBlurRendererFeature : ScriptableRendererFeature 
{   // 移轴效果shader
    public Shader TiltShiftBlurShader;

    // 声明屏幕缓冲rt
    private RenderTargetHandle m_CameraColorAttachment;
    
    // 拷贝目标rt
    private RenderTargetHandle m_CameraTransparentColorAttachment;
    
    // 屏幕拷贝Pass
    private MyRenderPass m_MyRenderPass;

    // 移轴效果pass
    private TiltShiftBlurPass m_TiltShiftBlurBlockPass;

    // 移轴效果材质球
    private Material m_TiltShiftBlurmaterial;

    //Initializes this feature's resources.
    public override void Create() 
    { 
        // 初始化Pass
        m_MyRenderPass = new MyRenderPass(RenderPassEvent.AfterRenderingTransparents);
        m_TiltShiftBlurBlockPass = new TiltShiftBlurPass(RenderPassEvent.AfterRenderingTransparents);

        // 传入到显存中的rt
        m_CameraColorAttachment.Init("_CameraColorTexture");
        m_CameraTransparentColorAttachment.Init("_CameraTransparentColorTexture");

        // 初始化材质球
        m_TiltShiftBlurmaterial = new Material(TiltShiftBlurShader);
    } 

    //Injects one or multiple ScriptableRenderPass in the renderer.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) 
    { 
        // 添加屏幕copyPass
        m_MyRenderPass.Setup(m_CameraColorAttachment.Identifier(), m_CameraTransparentColorAttachment);
        renderer.EnqueuePass(m_MyRenderPass);   // 将pass加入到待执行队列中

        // 添加移轴效果pass
        m_TiltShiftBlurBlockPass.Setup(m_CameraTransparentColorAttachment.Identifier(), m_CameraColorAttachment.Identifier(), m_TiltShiftBlurmaterial);
        renderer.EnqueuePass(m_TiltShiftBlurBlockPass);
    } 
} 
 