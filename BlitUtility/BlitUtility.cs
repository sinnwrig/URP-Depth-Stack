using UnityEngine;
using UnityEngine.Rendering;


public static class BlitUtility 
{
    static readonly int currentBlitTarget = Shader.PropertyToID("_BlitTarget");
    
    
    static readonly int blitTargetA = Shader.PropertyToID("_BlitA");
    static readonly int blitTargetB = Shader.PropertyToID("_BlitB");


    static RenderTargetIdentifier destinationA = new RenderTargetIdentifier(blitTargetA);
    static RenderTargetIdentifier destinationB = new RenderTargetIdentifier(blitTargetB);


    static RenderTargetIdentifier latestDest;
    static CommandBuffer blitCommandBuffer;


    public static void SetupBlitTargets(CommandBuffer blitCommandBuffer, RenderTextureDescriptor blitSourceDescriptor) 
    {
        if (blitCommandBuffer == null) 
        {
            Debug.LogError("Blit Command Buffer is null, cannot set up blit targets.");
        }

        RenderTextureDescriptor descriptor = blitSourceDescriptor;
        descriptor.depthBufferBits = 0;

        blitCommandBuffer.GetTemporaryRT(blitTargetA, descriptor, FilterMode.Bilinear);
        blitCommandBuffer.GetTemporaryRT(blitTargetB, descriptor, FilterMode.Bilinear);
        BlitUtility.blitCommandBuffer = blitCommandBuffer;
    }


    public static void BeginBlit(RenderTargetIdentifier source) 
    {
        latestDest = source;
    }


    public static void BlitNext(Material material, int pass = 0) 
    {
        var first = latestDest;
        var last = first == destinationA ? destinationB : destinationA;

        blitCommandBuffer.SetGlobalTexture(currentBlitTarget, first);
            
        blitCommandBuffer.Blit(first, last, material, pass);
        latestDest = last;
    }


    public static void EndBlit(RenderTargetIdentifier destination) 
    {
        blitCommandBuffer.Blit(latestDest, destination);
    }


    public static void ReleaseBlitTargets() 
    {
        blitCommandBuffer.ReleaseTemporaryRT(blitTargetA);
        blitCommandBuffer.ReleaseTemporaryRT(blitTargetB);
    }
}