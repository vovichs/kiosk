using UnityEngine;
using UnityEngine.Rendering;

namespace PSX
{
	[ExecuteInEditMode]
	public class FogController : MonoBehaviour
	{
		[SerializeField]
		protected VolumeProfile volumeProfile;

		[SerializeField]
		protected bool isEnabled = true;

		protected Fog fog;

		[Range(0f, 10f)]
		[SerializeField]
		protected float fogDensity = 1f;

		[Range(0f, 100f)]
		[SerializeField]
		protected float fogDistance = 10f;

		[Range(0f, 100f)]
		[SerializeField]
		protected float fogNear = 1f;

		[Range(0f, 100f)]
		[SerializeField]
		protected float fogFar = 100f;

		[Range(0f, 100f)]
		[SerializeField]
		protected float fogAltScale = 10f;

		[Range(0f, 1000f)]
		[SerializeField]
		protected float fogThinning = 100f;

		[Range(0f, 1000f)]
		[SerializeField]
		protected float noiseScale = 100f;

		[Range(0f, 1f)]
		[SerializeField]
		protected float noiseStrength = 0.05f;

		[SerializeField]
		protected Color fogColor;

		protected void Update()
		{
			SetParams();
		}

		protected void SetParams()
		{
			if (isEnabled && !(volumeProfile == null))
			{
				if (fog == null)
				{
					volumeProfile.TryGet(out fog);
				}
				if (!(fog == null))
				{
					fog.fogDensity.value = fogDensity;
					fog.fogDistance.value = fogDistance;
					fog.fogNear.value = fogNear;
					fog.fogFar.value = fogFar;
					fog.fogAltScale.value = fogAltScale;
					fog.fogThinning.value = fogThinning;
					fog.noiseScale.value = noiseScale;
					fog.noiseStrength.value = noiseStrength;
					fog.fogColor.value = fogColor;
				}
			}
		}
	}
}
