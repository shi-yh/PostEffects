using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessPosetProcessing : ScriptableRendererFeature
{
    [System.Serializable]
    public class Setting
    {
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;

        public Material mat;
    }

    class CustomRenderPass : ScriptableRenderPass
    {
        /// <summary>
        /// 后处理用到的材质
        /// </summary>
        public Material passMat = null;

        /// <summary>
        /// 源图像
        /// </summary>
        private RenderTargetIdentifier passSource { get; set; }

        /// <summary>
        /// 临时计算的图像
        /// </summary>
        private RenderTargetHandle passTempleColorTex;

        /// <summary>
        /// 用于在帧调试器中显示缓冲区的名称
        /// </summary>
        private string passTag;

        public CustomRenderPass(RenderPassEvent passEvent, Material mat, string tag)
        {
            this.renderPassEvent = passEvent;

            this.passMat = mat;

            passTag = tag;
        }


        public void SetUp(RenderTargetIdentifier source)
        {
            this.passSource = source;
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            ///从命令缓冲区池中获取一个带标签的命令缓冲区，该标签名称在帧调试器中可以见到
            CommandBuffer cmd = CommandBufferPool.Get(passTag);

            ///获取目标相机的描述信息，创建一个构造体，里面有render Texture各种信息
            RenderTextureDescriptor opaqueDes = renderingData.cameraData.cameraTargetDescriptor;

            ///设置深度缓冲区为0，不需要深度缓冲区
            opaqueDes.depthBufferBits = 0;
            ///申请临时图像
            cmd.GetTemporaryRT(passTempleColorTex.id, opaqueDes);
            ///将源图像放入材质中计算，然后存储到临时缓冲区中
            cmd.Blit(passSource, passTempleColorTex.Identifier(), passMat);
            ///将临时缓冲区的计算结果存回源图像
            cmd.Blit(passTempleColorTex.Identifier(), passSource);
            ///执行命令缓冲区
            context.ExecuteCommandBuffer(cmd);
            ///释放命令缓存
            CommandBufferPool.Release(cmd);
            ///释放临时render Texture
            cmd.ReleaseTemporaryRT(passTempleColorTex.id);
        }
    }


    public Setting setting = new Setting();

    private CustomRenderPass pass;


    public override void Create()
    {
        pass = new CustomRenderPass(setting.passEvent, setting.mat, name);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var src = renderer.cameraColorTarget;
        pass.SetUp(src);
        renderer.EnqueuePass(pass);
    }
}