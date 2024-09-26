using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class DitheringPass : ScriptableRenderPass
	{
		private static readonly string shaderPath = "PostEffect/Dithering";

		private static readonly string k_RenderTag = "Render Dithering Effects";

		private static readonly int MainTexId = Shader.PropertyToID("_MainTex");

		private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetDithering");

		private static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");

		private static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");

		private static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");

		private static readonly int DitherScale = Shader.PropertyToID("_DitherScale");

		private Dithering dithering;

		private Material ditheringMaterial;

		private RenderTargetIdentifier currentTarget;

		public DitheringPass(RenderPassEvent evt)
		{
			base.renderPassEvent = evt;
			Shader shader = Shader.Find(shaderPath);
			if (shader == null)
			{
				UnityEngine.Debug.LogError("Shader not found.");
			}
			else
			{
				ditheringMaterial = CoreUtils.CreateEngineMaterial(shader);
			}
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (ditheringMaterial == null)
			{
				UnityEngine.Debug.LogError("Material not created.");
			}
			else if (renderingData.cameraData.postProcessEnabled)
			{
				VolumeStack stack = VolumeManager.instance.stack;
				dithering = stack.GetComponent<Dithering>();
				if (!(dithering == null) && dithering.IsActive())
				{
					CommandBuffer commandBuffer = CommandBufferPool.Get(k_RenderTag);
					Render(commandBuffer, ref renderingData);
					context.ExecuteCommandBuffer(commandBuffer);
					CommandBufferPool.Release(commandBuffer);
				}
			}
		}

		public void Setup(in RenderTargetIdentifier currentTarget)
		{
			this.currentTarget = currentTarget;
		}

		private void Render(CommandBuffer cmd, ref RenderingData renderingData)
		{
			ref CameraData cameraData = ref renderingData.cameraData;
			RenderTargetIdentifier renderTargetIdentifier = currentTarget;
			int tempTargetId = TempTargetId;
			int scaledPixelWidth = cameraData.camera.scaledPixelWidth;
			int scaledPixelHeight = cameraData.camera.scaledPixelHeight;
			cameraData.camera.depthTextureMode = (cameraData.camera.depthTextureMode | DepthTextureMode.Depth);
			ditheringMaterial.SetInt(PatternIndex, dithering.patternIndex.value);
			ditheringMaterial.SetFloat(DitherThreshold, dithering.ditherThreshold.value);
			ditheringMaterial.SetFloat(DitherStrength, dithering.ditherStrength.value);
			ditheringMaterial.SetFloat(DitherScale, dithering.ditherScale.value);
			int pass = 0;
			cmd.SetGlobalTexture(MainTexId, renderTargetIdentifier);
			cmd.GetTemporaryRT(tempTargetId, scaledPixelWidth, scaledPixelHeight, 0, FilterMode.Point, RenderTextureFormat.Default);
			cmd.Blit(renderTargetIdentifier, tempTargetId);
			cmd.Blit(tempTargetId, renderTargetIdentifier, ditheringMaterial, pass);
		}
	}
}
