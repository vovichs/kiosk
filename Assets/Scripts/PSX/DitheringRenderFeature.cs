using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class DitheringRenderFeature : ScriptableRendererFeature
	{
		private DitheringPass ditheringPass;

		public override void Create()
		{
			ditheringPass = new DitheringPass(RenderPassEvent.BeforeRenderingPostProcessing);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			DitheringPass obj = ditheringPass;
			RenderTargetIdentifier currentTarget = renderer.cameraColorTarget;
			renderer.EnqueuePass(ditheringPass);
		}
	}
}
