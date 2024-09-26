using UnityEngine;
using UnityEngine.Rendering;

namespace PSX
{
	[ExecuteInEditMode]
	public class DitheringController : MonoBehaviour
	{
		[SerializeField]
		protected VolumeProfile volumeProfile;

		[SerializeField]
		protected bool isEnabled = true;

		protected Dithering dithering;

		[SerializeField]
		protected int patternIndex;

		[SerializeField]
		protected float ditherThreshold = 1f;

		[SerializeField]
		protected float ditherStrength = 1f;

		[SerializeField]
		protected float ditherScale = 2f;

		protected void Update()
		{
			SetParams();
		}

		protected void SetParams()
		{
			if (isEnabled && !(volumeProfile == null))
			{
				if (dithering == null)
				{
					volumeProfile.TryGet(out dithering);
				}
				if (!(dithering == null))
				{
					dithering.patternIndex.value = patternIndex;
					dithering.ditherThreshold.value = ditherThreshold;
					dithering.ditherStrength.value = ditherStrength;
					dithering.ditherScale.value = ditherScale;
				}
			}
		}
	}
}
