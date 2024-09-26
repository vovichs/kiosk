using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class Fog : VolumeComponent, IPostProcessComponent
	{
		[Range(0f, 10f)]
		public FloatParameter fogDensity = new FloatParameter(1f);

		[Range(0f, 100f)]
		public FloatParameter fogDistance = new FloatParameter(10f);

		public ColorParameter fogColor = new ColorParameter(Color.white);

		[Range(0f, 100f)]
		public FloatParameter fogNear = new FloatParameter(1f);

		[Range(0f, 100f)]
		public FloatParameter fogFar = new FloatParameter(100f);

		[Range(0f, 100f)]
		public FloatParameter fogAltScale = new FloatParameter(10f);

		[Range(0f, 1000f)]
		public FloatParameter fogThinning = new FloatParameter(100f);

		[Range(0f, 1000f)]
		public FloatParameter noiseScale = new FloatParameter(100f);

		[Range(0f, 1f)]
		public FloatParameter noiseStrength = new FloatParameter(0.05f);

		public bool IsActive()
		{
			return true;
		}

		public bool IsTileCompatible()
		{
			return false;
		}
	}
}
