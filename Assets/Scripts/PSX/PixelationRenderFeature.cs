using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class PixelationRenderFeature : ScriptableRendererFeature
	{
		private PixelationPass pixelationPass;

		public override void Create()
		{
			pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			PixelationPass obj = pixelationPass;
			RenderTargetIdentifier currentTarget = renderer.cameraColorTarget;
			renderer.EnqueuePass(pixelationPass);
		}
	}
}
