using UnityEngine;
using UnityEngine.Rendering;

namespace PSX
{
	[ExecuteInEditMode]
	public class PixelationController : MonoBehaviour
	{
		[SerializeField]
		protected VolumeProfile volumeProfile;

		[SerializeField]
		protected bool isEnabled = true;

		protected Pixelation pixelation;

		[SerializeField]
		protected float widthPixelation = 512f;

		[SerializeField]
		protected float heightPixelation = 256f;

		[SerializeField]
		protected float colorPrecision = 16f;

		protected void Update()
		{
			SetParams();
		}

		protected void SetParams()
		{
			if (isEnabled && !(volumeProfile == null))
			{
				if (pixelation == null)
				{
					volumeProfile.TryGet(out pixelation);
				}
				if (!(pixelation == null))
				{
					pixelation.widthPixelation.value = widthPixelation;
					pixelation.heightPixelation.value = heightPixelation;
					pixelation.colorPrecision.value = colorPrecision;
				}
			}
		}
	}
}
