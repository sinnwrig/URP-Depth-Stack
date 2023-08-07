using UnityEngine;
using UnityEngine.Rendering.Universal;

// Add this Render Feature to the active URP Renderer and assign the CopyDepth material in DepthStack/Shader to use encoded depth in a shader

public class DepthStackRenderFeature : ScriptableRendererFeature
{
    public Material copyDepth;

    DepthStackRenderPass cameraRenderPass;
    

    public override void Create() 
    {
        if (copyDepth == null) 
        {
            Debug.LogError("CopyDepth material is not assigned!", this);
        }

        cameraRenderPass = new DepthStackRenderPass(copyDepth);
        cameraRenderPass.renderPassEvent = RenderPassEvent.AfterRendering;
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) 
    {
        if (!renderingData.cameraData.isPreviewCamera) 
        {
            renderer.EnqueuePass(cameraRenderPass);
        }
    }
}
