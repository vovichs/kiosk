using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	public static PlayerController instance;

	public Camera mainCamera;

	public Transform recoilObj;

	public AudioClip walkMetal;

	public AudioClip walkTile;

	public AudioSource stepsAudio;

	private CharacterController characterController;

	public static bool blockGlobal = true;

	public float mouseSensitivity = 30f;

	public float movementSpeed = 10f;

	public float jumpSpeed = 7.5f;

	public float gravity = 9.81f;

	public float zoomSpeed = 20f;

	public GrabObject currentGrabObj;

	public Transform grabHolder;

	[Header("Interactable Values")]
	public LayerMask interactableMask;

	public LayerMask pointMask;

	public float interactableRange = 5f;

	[Header("Crouch Values")]
	public Vector3 crouchCameraPos = new Vector3(0f, 0.2f, 0f);

	public LayerMask crouchLayerMask;

	public Vector3 GizmosScale;

	public float crouchValue = 0.8f;

	[Header("Headbob Values")]
	public float walkingBobbingSpeed = 14f;

	public float bobbingAmount = 0.05f;

	private float defaultPosY;

	private float timer;

	private float mouseX;

	private float mouseY;

	private float duckMoveSpeed = 6f;

	private float playerViewYOffset = 0.6f;

	private float currentFieldOfView = 60f;

	private float recoilReturnSpeed = 8f;

	private float horizontal;

	private float vertical;

	private Vector3 moveDirection = Vector3.zero;

	private Vector3 xRotateVector;

	private Vector3 eulerRotation = Vector3.zero;

	private Vector3 recoil = Vector3.zero;

	private Collider[] hitColliders;

	public UnityEvent<GrabObject> onGrabObjEquip;

	public Transform stepCube;

	public LayerMask playerMask;

	private Interactable lastInteractable;

	private Interactable hoverInteractable;

	private bool canZoomPress;

	private Coroutine focusTargetCor;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
		Application.targetFrameRate = 300;
		QualitySettings.vSyncCount = 0;
		mainCamera.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + playerViewYOffset, base.transform.position.z);
		defaultPosY = recoilObj.localPosition.y;
	}

	public void LockCursor(bool b = false)
	{
		Cursor.visible = b;
		Cursor.lockState = ((!b) ? CursorLockMode.Locked : CursorLockMode.None);
	}

	private void Update()
	{
		if (!blockGlobal)
		{
			HandleHeadBob();
			HandleMovement();
			HandleCameraRotation();
			HandleInteractions();
			HandleAudioSteps();
		}
		else
		{
			stepsAudio.volume = 0f;
		}
	}

	public bool isGroudned()
	{
		if (characterController != null)
		{
			return characterController.isGrounded;
		}
		return false;
	}

	private void HandleAudioSteps()
	{
		stepsAudio.volume = Mathf.Clamp(characterController.velocity.magnitude / 3.2f, 0f, 0.5f);
		Collider[] array = Physics.OverlapBox(stepCube.position, stepCube.localScale / 2f, stepCube.rotation, playerMask);
		if (array.Length != 0 && stepsAudio.clip != walkMetal)
		{
			stepsAudio.clip = walkMetal;
			stepsAudio.Play();
		}
		else if (array.Length == 0 && stepsAudio.clip != walkTile)
		{
			stepsAudio.clip = walkTile;
			stepsAudio.Play();
		}
	}

	private void HandleHeadBob()
	{
		if (Mathf.Abs(characterController.velocity.x) > 0.1f || Mathf.Abs(characterController.velocity.z) > 0.1f)
		{
			timer += Time.deltaTime * walkingBobbingSpeed;
			recoilObj.localPosition = new Vector3(recoilObj.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, recoilObj.localPosition.z);
		}
		else
		{
			timer = 0f;
			recoilObj.localPosition = new Vector3(recoilObj.localPosition.x, Mathf.Lerp(recoilObj.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), recoilObj.localPosition.z);
		}
	}

	public void UpdateHoverName()
	{
		if (hoverInteractable != null)
		{
			UIController.instance.SetInteractableInfoText(hoverInteractable.interactableName);
		}
	}

	private void HandleInteractions()
	{
		if (blockGlobal)
		{
			UIController.instance.SetInteractableInfoTextActive(b: false);
		}
		if (Input.GetMouseButtonDown(0) && hasGrabObj())
		{
			currentGrabObj.Throw();
			SetGrabObj(null);
		}
		else if (Input.GetMouseButtonDown(1) && hasGrabObj())
		{
			currentGrabObj.Throw(addForce: false);
			SetGrabObj(null);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.E) && hasGrabObj())
		{
			currentGrabObj.StartUse();
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.E) && hasGrabObj())
		{
			currentGrabObj.StopUse();
		}
		else if (UnityEngine.Input.GetKey(KeyCode.E) && hasGrabObj())
		{
			currentGrabObj.Use();
		}
		else
		{
			if (hasGrabObj())
			{
				return;
			}
			GameObject gameObject = ShootRay();
			if (gameObject != null && gameObject.GetComponent<Interactable>() != null)
			{
				if (hoverInteractable != null && hoverInteractable.gameObject != gameObject && hoverInteractable.isInteractable() && hoverInteractable.enabled && hoverInteractable.gameObject.layer == 8)
				{
					UIController.instance.SetInteractableInfoTextActive(b: false);
					hoverInteractable = null;
				}
				if (getActiveInteractableObject(gameObject) != null && getActiveInteractableObject(gameObject).isInteractable())
				{
					hoverInteractable = getActiveInteractableObject(gameObject);
				}
			}
			else
			{
				UIController.instance.SetInteractableInfoTextActive(b: false);
			}
			if (lastInteractable != null && lastInteractable.enabled)
			{
				if (Input.GetMouseButtonUp(0))
				{
					lastInteractable.Release();
					lastInteractable = null;
				}
				if (Input.GetMouseButton(0) && lastInteractable != null)
				{
					lastInteractable.Drag();
				}
				if (!Input.GetMouseButton(0) && lastInteractable != null)
				{
					lastInteractable = null;
				}
			}
			else if (gameObject != null && gameObject.GetComponent<Interactable>() != null)
			{
				if (Input.GetMouseButtonDown(0))
				{
					if (hoverInteractable != null && hoverInteractable.isInteractable() && hoverInteractable.enabled)
					{
						UIController.instance.SetInteractableInfoTextActive(b: false);
						hoverInteractable = null;
					}
					if (getActiveInteractableObject(gameObject) != null)
					{
						lastInteractable = getActiveInteractableObject(gameObject);
						if (lastInteractable.enabled)
						{
							lastInteractable.Press();
						}
					}
				}
				if (hoverInteractable != null && hoverInteractable.isInteractable() && hoverInteractable.enabled)
				{
					if (hoverInteractable.gameObject.layer == 6)
					{
						UIController.instance.SetInteractableInfoText(hoverInteractable.interactableName);
						UIController.instance.SetInteractableInfoTextActive(hoverInteractable.interactableName != "");
					}
					else if (hoverInteractable.interactableName == "")
					{
						UIController.instance.SetInteractableInfoTextActive(b: false);
					}
				}
			}
			else if (hoverInteractable != null && hoverInteractable.isInteractable() && hoverInteractable.enabled)
			{
				if (hoverInteractable.gameObject.layer == 8)
				{
					UIController.instance.SetInteractableInfoTextActive(b: false);
					hoverInteractable = null;
				}
			}
			else if (hoverInteractable != null)
			{
				UIController.instance.SetInteractableInfoTextActive(b: false);
				hoverInteractable = null;
			}
			else
			{
				UIController.instance.SetInteractableInfoTextActive(b: false);
			}
		}
	}

	public void DeactivateHoverObj()
	{
		if (hoverInteractable != null)
		{
			hoverInteractable = null;
		}
	}

	private Interactable getActiveInteractableObject(GameObject tempObj)
	{
		Interactable[] components = tempObj.GetComponents<Interactable>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].enabled)
			{
				return components[i];
			}
		}
		return null;
	}

	private void HandleMovement()
	{
		horizontal = UnityEngine.Input.GetAxis("Horizontal");
		vertical = UnityEngine.Input.GetAxis("Vertical");
		if (CanStandUp())
		{
			playerViewYOffset = (UnityEngine.Input.GetKey(KeyCode.LeftControl) ? 0f : 0.6f);
			SetHeight(UnityEngine.Input.GetKey(KeyCode.LeftControl) ? crouchValue : 2f);
		}
		else if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
		{
			playerViewYOffset = 0f;
			SetHeight(crouchValue);
		}
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(base.transform.position.x, base.transform.position.y + playerViewYOffset, base.transform.position.z), duckMoveSpeed * Time.deltaTime);
		Vector3 direction = new Vector3(horizontal, 0f, vertical);
		Vector3 vector = base.transform.TransformDirection(direction) * movementSpeed;
		vector = Vector3.ClampMagnitude(vector, movementSpeed);
		if (!isGroudned() || !Input.GetKeyDown(KeyCode.Space))
		{
			if (isGroudned())
			{
				moveDirection.y -= gravity * Time.deltaTime;
			}
			else
			{
				moveDirection.y -= gravity * Time.deltaTime;
			}
		}
		moveDirection = new Vector3(vector.x, moveDirection.y, vector.z);
		characterController.Move(moveDirection * Time.deltaTime);
	}

	private float ConvertAngle(float angle)
	{
		angle %= 360f;
		if (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	private void HandleCameraRotation(float yClampMin = 85f, float yClampMax = 85f, float xClampMin = 181f, float xClampMax = 181f)
	{
		mouseX = UnityEngine.Input.GetAxis("Mouse X") * mouseSensitivity;
		mouseY = UnityEngine.Input.GetAxis("Mouse Y") * mouseSensitivity;
		float num = ConvertAngle(mainCamera.transform.localEulerAngles.x);
		if (num > 85f)
		{
			num = 85f;
			if (mouseY < 0f)
			{
				mouseY = 0f;
			}
		}
		else if (num < -85f)
		{
			num = -85f;
			if (mouseY > 0f)
			{
				mouseY = 0f;
			}
		}
		ClampXAxisRotationToValue(num);
		recoil.z = horizontal * -2f;
		recoilObj.transform.localRotation = Quaternion.RotateTowards(recoilObj.transform.localRotation, Quaternion.Euler(recoil), recoilReturnSpeed * Time.deltaTime);
		if (Input.GetMouseButtonDown(1))
		{
			canZoomPress = hasGrabObj();
		}
		if (!hasGrabObj() && !canZoomPress)
		{
			currentFieldOfView = (Input.GetMouseButton(1) ? 50f : 60f);
		}
		else
		{
			currentFieldOfView = 60f;
		}
		mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentFieldOfView, Time.deltaTime * zoomSpeed);
		mainCamera.transform.Rotate(Vector3.left * mouseY);
		base.transform.Rotate(Vector3.up * mouseX);
		xRotateVector = base.transform.localRotation.ToEulerAngles() * 57.29578f;
		xRotateVector.y = Mathf.Clamp(xRotateVector.y, 0f - xClampMin, xClampMax);
		base.transform.localEulerAngles = xRotateVector;
	}

	private void ClampXAxisRotationToValue(float value)
	{
		eulerRotation = mainCamera.transform.localEulerAngles;
		eulerRotation.x = value;
		eulerRotation.z = 0f;
		eulerRotation.y = 0f;
		mainCamera.transform.localEulerAngles = eulerRotation;
	}

	private void SetHeight(float index)
	{
		if (characterController.height != index)
		{
			characterController.height = index;
			if (index >= 2f)
			{
				characterController.center = Vector3.zero;
				return;
			}
			Vector3 center = characterController.center;
			center.y = (crouchValue - 2f) / 2f;
			characterController.center = center;
		}
	}

	private bool CanStandUp()
	{
		hitColliders = Physics.OverlapBox(mainCamera.transform.position + crouchCameraPos, GizmosScale / 2f, Quaternion.identity, crouchLayerMask);
		if (hitColliders.Length != 0)
		{
			return false;
		}
		return true;
	}

	public GameObject ShootRay()
	{
		if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo, 2.4f, interactableMask, QueryTriggerInteraction.Ignore))
		{
			return hitInfo.collider.gameObject;
		}
		return null;
	}

	public RaycastHit shootPoint()
	{
		if (Physics.Raycast(mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hitInfo, 2f, pointMask, QueryTriggerInteraction.Ignore))
		{
			return hitInfo;
		}
		return default(RaycastHit);
	}

	public void EquipGrabObj(GrabObject tempGrabObj)
	{
		if (!hasGrabObj())
		{
			SetGrabObj(tempGrabObj);
			tempGrabObj.Equip(grabHolder);
			onGrabObjEquip.Invoke(tempGrabObj);
		}
	}

	public void SetGrabObj(GrabObject tempGrabObj)
	{
		currentGrabObj = tempGrabObj;
		UIController.instance.SetButtonsHint(tempGrabObj != null, tempGrabObj != null && tempGrabObj.GetComponent<SauceBottle>() != null);
	}

	public bool hasGrabObj()
	{
		return currentGrabObj != null;
	}

	public GrabObject getGrabObj()
	{
		return currentGrabObj;
	}

	public bool isCurrentGrabObject(GrabObject thisGrabObj)
	{
		return currentGrabObj == thisGrabObj;
	}

	public float GetMouseX()
	{
		return mouseX;
	}

	public void FocusViewOn(Transform targetFocus)
	{
		if (!(targetFocus == null))
		{
			if (focusTargetCor != null)
			{
				StopCoroutine(focusTargetCor);
			}
			focusTargetCor = StartCoroutine(focusTarget(targetFocus));
		}
	}

	public void FocusAndRelease(Transform targetFocus, float speed = 0.15f)
	{
		if (!(targetFocus == null))
		{
			if (focusTargetCor != null)
			{
				StopCoroutine(focusTargetCor);
			}
			focusTargetCor = StartCoroutine(focusAndReleaseCor(targetFocus, speed));
		}
	}

	public void ForceStopFocus()
	{
		if (focusTargetCor != null)
		{
			StopCoroutine(focusTargetCor);
		}
	}

	private IEnumerator focusTarget(Transform targetFocus)
	{
		float lerpValue = 0f;
		float time = 0.38f;
		Quaternion currentRotation = base.transform.rotation;
		Quaternion endRotation = Quaternion.LookRotation(targetFocus.transform.position - base.transform.position);
		base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
		Vector3 localEulerAngle = mainCamera.transform.localEulerAngles;
		Vector3 zero = Vector3.zero;
		Quaternion currentRotQuat = mainCamera.transform.rotation;
		Quaternion endRotationQuat = Quaternion.LookRotation(targetFocus.transform.position - mainCamera.transform.position);
		float startFieldOfView = currentFieldOfView;
		float endFieldOfView = 50f;
		while (lerpValue <= time)
		{
			base.transform.rotation = Quaternion.Lerp(currentRotation, endRotation, lerpValue / time);
			base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
			mainCamera.transform.rotation = Quaternion.Lerp(currentRotQuat, endRotationQuat, lerpValue / time);
			mainCamera.transform.localEulerAngles = new Vector3(mainCamera.transform.localEulerAngles.x, 0f, 0f);
			mouseY = base.transform.eulerAngles.y;
			mainCamera.fieldOfView = Mathf.Lerp(startFieldOfView, endFieldOfView, lerpValue / time);
			currentFieldOfView = mainCamera.fieldOfView;
			mouseX = mainCamera.transform.eulerAngles.x;
			lerpValue += Time.deltaTime;
			yield return null;
		}
		mainCamera.fieldOfView = endFieldOfView;
		currentFieldOfView = mainCamera.fieldOfView;
		base.transform.rotation = endRotation;
		base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
		mainCamera.transform.rotation = endRotationQuat;
		mainCamera.transform.localEulerAngles = new Vector3(mainCamera.transform.localEulerAngles.x, 0f, 0f);
		mouseY = base.transform.eulerAngles.y;
		mouseX = mainCamera.transform.localEulerAngles.x;
	}

	private IEnumerator focusAndReleaseCor(Transform targetFocus, float speed)
	{
		blockGlobal = true;
		float lerpValue = 0f;
		Quaternion currentRotation = base.transform.rotation;
		Quaternion endRotation = Quaternion.LookRotation(targetFocus.transform.position - base.transform.position);
		base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
		Vector3 localEulerAngle = mainCamera.transform.localEulerAngles;
		Vector3 zero = Vector3.zero;
		Quaternion currentRotQuat = mainCamera.transform.rotation;
		Quaternion endRotationQuat = Quaternion.LookRotation(targetFocus.transform.position - mainCamera.transform.position);
		while (lerpValue <= speed)
		{
			base.transform.rotation = Quaternion.Lerp(currentRotation, endRotation, lerpValue / speed);
			base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
			mainCamera.transform.rotation = Quaternion.Lerp(currentRotQuat, endRotationQuat, lerpValue / speed);
			mainCamera.transform.localEulerAngles = new Vector3(mainCamera.transform.localEulerAngles.x, 0f, 0f);
			mouseY = base.transform.eulerAngles.y;
			currentFieldOfView = mainCamera.fieldOfView;
			mouseX = mainCamera.transform.eulerAngles.x;
			lerpValue += Time.deltaTime;
			yield return null;
		}
		currentFieldOfView = mainCamera.fieldOfView;
		base.transform.rotation = endRotation;
		base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
		mainCamera.transform.rotation = endRotationQuat;
		mainCamera.transform.localEulerAngles = new Vector3(mainCamera.transform.localEulerAngles.x, 0f, 0f);
		mouseY = base.transform.eulerAngles.y;
		mouseX = mainCamera.transform.localEulerAngles.x;
		blockGlobal = false;
	}

	private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
	{
		float x = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
		float y = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
		float z = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
		return new Vector3(x, y, z);
	}

	public void MoveAndRotateTo(Transform newPos)
	{
		base.transform.position = newPos.position;
		base.transform.rotation = newPos.rotation;
		mainCamera.transform.localEulerAngles = Vector3.zero;
	}

	public void SetSensitivity(float value)
	{
		mouseSensitivity = value;
	}
}
