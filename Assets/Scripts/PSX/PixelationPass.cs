using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class PixelationPass : ScriptableRenderPass
	{
		private static readonly string shaderPath = "PostEffect/Pixelation";

		private static readonly string k_RenderTag = "Render Pixelation Effects";

		private static readonly int MainTexId = Shader.PropertyToID("_MainTex");

		private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetPixelation");

		private static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");

		private static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");

		private static readonly int ColorPrecison = Shader.PropertyToID("_ColorPrecision");

		private Pixelation pixelation;

		private Material pixelationMaterial;

		private RenderTargetIdentifier currentTarget;

		public PixelationPass(RenderPassEvent evt)
		{
			base.renderPassEvent = evt;
			Shader shader = Shader.Find(shaderPath);
			if (shader == null)
			{
				UnityEngine.Debug.LogError("Shader not found.");
			}
			else
			{
				pixelationMaterial = CoreUtils.CreateEngineMaterial(shader);
			}
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (pixelationMaterial == null)
			{
				UnityEngine.Debug.LogError("Material not created.");
			}
			else if (renderingData.cameraData.postProcessEnabled)
			{
				VolumeStack stack = VolumeManager.instance.stack;
				pixelation = stack.GetComponent<Pixelation>();
				if (!(pixelation == null) && pixelation.IsActive())
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
			pixelationMaterial.SetFloat(WidthPixelation, pixelation.widthPixelation.value);
			pixelationMaterial.SetFloat(HeightPixelation, pixelation.heightPixelation.value);
			pixelationMaterial.SetFloat(ColorPrecison, pixelation.colorPrecision.value);
			int pass = 0;
			cmd.SetGlobalTexture(MainTexId, renderTargetIdentifier);
			cmd.GetTemporaryRT(tempTargetId, scaledPixelWidth, scaledPixelHeight, 0, FilterMode.Point, RenderTextureFormat.Default);
			cmd.Blit(renderTargetIdentifier, tempTargetId);
			cmd.Blit(tempTargetId, renderTargetIdentifier, pixelationMaterial, pass);
		}
	}
}
