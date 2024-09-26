using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class FogRenderFeature : ScriptableRendererFeature
	{
		private FogPass fogPass;

		public override void Create()
		{
			fogPass = new FogPass(RenderPassEvent.BeforeRenderingPostProcessing);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			FogPass obj = fogPass;
			RenderTargetIdentifier currentTarget = renderer.cameraColorTarget;
			renderer.EnqueuePass(fogPass);
		}
	}
}
