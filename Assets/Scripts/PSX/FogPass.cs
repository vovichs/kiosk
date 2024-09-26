using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class FogPass : ScriptableRenderPass
	{
		private static readonly string shaderPath = "PostEffect/Fog";

		private static readonly string k_RenderTag = "Render Fog Effects";

		private static readonly int MainTexId = Shader.PropertyToID("_MainTex");

		private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetFog");

		private static readonly int FogDensity = Shader.PropertyToID("_FogDensity");

		private static readonly int FogDistance = Shader.PropertyToID("_FogDistance");

		private static readonly int FogColor = Shader.PropertyToID("_FogColor");

		private static readonly int FogNear = Shader.PropertyToID("_FogNear");

		private static readonly int FogFar = Shader.PropertyToID("_FogFar");

		private static readonly int FogAltScale = Shader.PropertyToID("_FogAltScale");

		private static readonly int FogThinning = Shader.PropertyToID("_FogThinning");

		private static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");

		private static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");

		private Fog fog;

		private Material fogMaterial;

		private RenderTargetIdentifier currentTarget;

		public FogPass(RenderPassEvent evt)
		{
			base.renderPassEvent = evt;
			Shader shader = Shader.Find(shaderPath);
			if (shader == null)
			{
				UnityEngine.Debug.LogError("Shader not found.");
			}
			else
			{
				fogMaterial = CoreUtils.CreateEngineMaterial(shader);
			}
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (fogMaterial == null)
			{
				UnityEngine.Debug.LogError("Material not created.");
			}
			else if (renderingData.cameraData.postProcessEnabled)
			{
				VolumeStack stack = VolumeManager.instance.stack;
				fog = stack.GetComponent<Fog>();
				if (!(fog == null) && fog.IsActive())
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
			fogMaterial.SetFloat(FogDensity, fog.fogDensity.value);
			fogMaterial.SetFloat(FogDistance, fog.fogDistance.value);
			fogMaterial.SetColor(FogColor, fog.fogColor.value);
			fogMaterial.SetFloat(FogNear, fog.fogNear.value);
			fogMaterial.SetFloat(FogFar, fog.fogFar.value);
			fogMaterial.SetFloat(FogAltScale, fog.fogAltScale.value);
			fogMaterial.SetFloat(FogThinning, fog.fogThinning.value);
			fogMaterial.SetFloat(NoiseScale, fog.noiseScale.value);
			fogMaterial.SetFloat(NoiseStrength, fog.noiseStrength.value);
			int pass = 0;
			cmd.SetGlobalTexture(MainTexId, renderTargetIdentifier);
			cmd.GetTemporaryRT(tempTargetId, scaledPixelWidth, scaledPixelHeight, 0, FilterMode.Point, RenderTextureFormat.Default);
			cmd.Blit(renderTargetIdentifier, tempTargetId);
			cmd.Blit(tempTargetId, renderTargetIdentifier, fogMaterial, pass);
		}
	}
}
