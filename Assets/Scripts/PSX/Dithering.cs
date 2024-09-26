using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class Dithering : VolumeComponent, IPostProcessComponent
	{
		public IntParameter patternIndex = new IntParameter(0);

		public FloatParameter ditherThreshold = new FloatParameter(512f);

		public FloatParameter ditherStrength = new FloatParameter(1f);

		public FloatParameter ditherScale = new FloatParameter(2f);

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
