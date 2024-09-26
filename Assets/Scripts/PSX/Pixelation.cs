using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
	public class Pixelation : VolumeComponent, IPostProcessComponent
	{
		public FloatParameter widthPixelation = new FloatParameter(512f);

		public FloatParameter heightPixelation = new FloatParameter(512f);

		public FloatParameter colorPrecision = new FloatParameter(32f);

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
