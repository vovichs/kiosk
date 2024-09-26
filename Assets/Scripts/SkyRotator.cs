using UnityEngine;

public class SkyRotator : MonoBehaviour
{
	public Material skyMat;

	private float rotationFloat;

	private void Update()
	{
		skyMat.SetFloat("_Rotation", rotationFloat);
		rotationFloat += Time.deltaTime * 4f;
		if (rotationFloat >= 360f)
		{
			rotationFloat = 0f;
		}
	}
}
