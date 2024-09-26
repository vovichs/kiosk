using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public static UIController instance;

	public Text interactableInfoText;

	public GameObject crosshairFill;

	public GameObject crosshair;

	public CanvasGroup fadeScreen;

	public Text fadeScreenTextNormal;

	public GameObject mainMenuScreen;

	public GameObject settingsScreen;

	public GameObject pauseMenu;

	public GameObject cookBook;

	public GameObject continueBtn;

	public GameObject buttonsHint;

	public GameObject useButton;

	private bool nextOpenCookBook;

	private float halfSound = -10f;

	private Coroutine fadeToMainmenuCor;

	private Coroutine fadeNextDayScreenCor;

	private bool cameFromPauseMenu;

	public Slider musicSlider;

	public Slider sensitivitySlider;

	public Text sensitivityValueText;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		CheckContinueBtn();
		musicSlider.value = PlayerPrefs.GetFloat("volumePref", 1f);
		sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivityValue", 1.4f);
		SetVolume(musicSlider.value);
		SetSensitivity(sensitivitySlider.value);
		StartCoroutine(startFadeout());
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (nextOpenCookBook)
			{
				nextOpenCookBook = false;
			}
			else
			{
				CloseCookBook();
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && fadeToMainmenuCor == null && fadeNextDayScreenCor == null)
		{
			if (cookBook.activeSelf)
			{
				CloseCookBook();
			}
			else if (!PlayerController.blockGlobal && !pauseMenu.activeSelf)
			{
				PauseGame();
			}
			else if (settingsScreen.activeSelf)
			{
				CloseSettings();
			}
			else if (pauseMenu.activeSelf)
			{
				ResumeGame();
			}
		}
	}

	public void OpenCookBook()
	{
		cookBook.SetActive(value: true);
		PlayerController.blockGlobal = true;
		SetCrosshair(b: false);
		nextOpenCookBook = true;
	}

	public void CloseCookBook()
	{
		if (cookBook.activeSelf)
		{
			cookBook.SetActive(value: false);
			PlayerController.blockGlobal = false;
			SetCrosshair(b: true);
		}
	}

	public void SetInteractableInfoText(string text)
	{
		interactableInfoText.text = text;
	}

	public void SetInteractableInfoTextActive(bool b)
	{
		if ((b || crosshairFill.activeInHierarchy) && (!b || !crosshairFill.activeInHierarchy))
		{
			interactableInfoText.gameObject.SetActive(b);
			if (!b)
			{
				SetInteractableInfoText("");
			}
			SetCrosshairFill(b);
		}
	}

	public void ForceDisableInteractText()
	{
		interactableInfoText.gameObject.SetActive(value: false);
		SetInteractableInfoText("");
		SetCrosshairFill(b: false);
	}

	public void SetCrosshairFill(bool b)
	{
		crosshairFill.SetActive(b);
	}

	public void SetCrosshair(bool b)
	{
		crosshair.SetActive(b);
	}

	public void NextDayScreen(bool skipFadeIn = false)
	{
		fadeNextDayScreenCor = StartCoroutine(fadeNextDayScreen("Day " + (GameManager.instance.currentDay + 1).ToString(), skipFadeIn));
	}

	public void EndCredit()
	{
		crosshair.SetActive(value: false);
		PlayerController.blockGlobal = true;
		fadeScreen.blocksRaycasts = true;
		fadeScreenTextNormal.text = "";
		fadeScreen.alpha = 1f;
		StartCoroutine(startCredits());
	}

	private IEnumerator startCredits()
	{
		crosshair.SetActive(value: false);
		PlayerController.blockGlobal = true;
		fadeScreen.blocksRaycasts = true;
		fadeScreenTextNormal.text = "";
		fadeScreen.alpha = 1f;
		yield return new WaitForSeconds(6f);
		string originalText2 = "";
		string endCreditsText2 = "Game made by: Vivi";
		char[] wordToShow2 = endCreditsText2.ToCharArray();
		for (int k = 0; k < wordToShow2.Length; k++)
		{
			originalText2 += wordToShow2[k].ToString();
			AudioController.instance.SpawnDialogLetterTick();
			fadeScreenTextNormal.text = originalText2;
			yield return new WaitForSecondsRealtime(DialogController.instance.letterShowSpeed * 2f);
		}
		fadeScreenTextNormal.text = endCreditsText2;
		yield return new WaitForSeconds(2.4f);
		for (int k = 0; k < wordToShow2.Length; k++)
		{
			AudioController.instance.SpawnDialogLetterTick();
			fadeScreenTextNormal.text = fadeScreenTextNormal.text.Remove(fadeScreenTextNormal.text.Length - 1);
			yield return new WaitForSecondsRealtime(DialogController.instance.letterShowSpeed * 2f);
		}
		yield return new WaitForSeconds(1f);
		originalText2 = "";
		endCreditsText2 = "Thanks for playing! \n If you want to see more games \n like this follow me on X or Itch.io";
		wordToShow2 = endCreditsText2.ToCharArray();
		for (int k = 0; k < wordToShow2.Length; k++)
		{
			AudioController.instance.SpawnDialogLetterTick();
			originalText2 += wordToShow2[k].ToString();
			fadeScreenTextNormal.text = originalText2;
			yield return new WaitForSecondsRealtime(DialogController.instance.letterShowSpeed * 2f);
		}
		yield return new WaitForSeconds(4f);
		fadeScreenTextNormal.text = "Some of the assets used from:\n DINER , Paquete de modelos psx 3 by Elbolilloduro  \n Low Poly Human Mesh by Comp-3 Interactive \n Sounds used from freesound.org";
		yield return new WaitForSeconds(7f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private IEnumerator startFadeout()
	{
		float time = 0.4f;
		PlayerController.instance.MoveAndRotateTo(GameManager.instance.mainMenuPos);
		PlayerController.instance.LockCursor(b: true);
		PlayerController.blockGlobal = true;
		crosshair.SetActive(value: false);
		fadeScreen.blocksRaycasts = true;
		fadeScreenTextNormal.text = "";
		mainMenuScreen.SetActive(value: true);
		AudioController.instance.FadeMasterTo(halfSound);
		float lerpValue = 0f;
		while (lerpValue <= time)
		{
			fadeScreen.alpha = Mathf.Lerp(1f, 0f, lerpValue / time);
			lerpValue += Time.deltaTime;
			yield return null;
		}
		fadeScreen.alpha = 0f;
		fadeScreen.blocksRaycasts = false;
	}

	private IEnumerator fadeNextDayScreen(string textToShow, bool skipFadeIn = false)
	{
		PlayerController.blockGlobal = true;
		float lerpValue2 = 0f;
		float time = 0.4f;
		fadeScreen.blocksRaycasts = true;
		fadeScreenTextNormal.text = "";
		AudioController.instance.FadeMasterTo(-80f);
		if (!skipFadeIn)
		{
			while (lerpValue2 <= time)
			{
				fadeScreen.alpha = Mathf.Lerp(0f, 1f, lerpValue2 / time);
				lerpValue2 += Time.deltaTime;
				yield return null;
			}
		}
		ReactionController.instance.InvokeReaction("TurnOffMusic");
		fadeScreen.alpha = 1f;
		mainMenuScreen.gameObject.SetActive(value: false);
		PlayerController.instance.MoveAndRotateTo(GameManager.instance.startingPos);
		PlayerController.instance.LockCursor();
		crosshair.SetActive(value: true);
		yield return new WaitForSeconds(1.2f);
		string originalText = "";
		char[] wordToShow = textToShow.ToCharArray();
		for (int j = 0; j < wordToShow.Length; j++)
		{
			originalText += wordToShow[j].ToString();
			fadeScreenTextNormal.text = originalText;
			AudioController.instance.SpawnDialogLetterTick();
			yield return new WaitForSecondsRealtime(DialogController.instance.letterShowSpeed * 2f);
		}
		yield return new WaitForSeconds(2.4f);
		for (int j = 0; j < wordToShow.Length; j++)
		{
			AudioController.instance.SpawnDialogLetterTick();
			fadeScreenTextNormal.text = fadeScreenTextNormal.text.Remove(fadeScreenTextNormal.text.Length - 1);
			yield return new WaitForSecondsRealtime(DialogController.instance.letterShowSpeed * 2f);
		}
		AudioController.instance.FadeMasterTo(0f, 1.3f);
		yield return new WaitForSeconds(1.2f);
		PlayerController.blockGlobal = false;
		lerpValue2 = 0f;
		while (lerpValue2 <= time)
		{
			fadeScreen.alpha = Mathf.Lerp(1f, 0f, lerpValue2 / time);
			lerpValue2 += Time.deltaTime;
			yield return null;
		}
		fadeScreen.alpha = 0f;
		fadeScreen.blocksRaycasts = false;
		yield return new WaitForSeconds(3f);
		if (GameManager.instance.currentDay != 0)
		{
			CustomersController.instance.SpawnCustomer();
		}
		else
		{
			ReactionController.instance.InvokeReaction("PhoneCallStart");
		}
		fadeNextDayScreenCor = null;
	}

	private IEnumerator fadeToMainMenu()
	{
		PlayerController.instance.LockCursor(b: true);
		PlayerController.blockGlobal = true;
		crosshair.SetActive(value: false);
		fadeScreen.blocksRaycasts = true;
		fadeScreenTextNormal.text = "";
		float lerpValue = 0f;
		float time = 0.4f;
		AudioController.instance.FadeMasterTo(-80f);
		while (lerpValue <= time)
		{
			fadeScreen.alpha = Mathf.Lerp(0f, 1f, lerpValue / time);
			lerpValue += Time.deltaTime;
			yield return null;
		}
		fadeScreen.alpha = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void StartTheGame()
	{
		PlayerPrefs.SetInt("currentDay", 0);
		GameManager.instance.currentDay = 0;
		GameManager.instance.currentCustomer = 0;
		NextDayScreen();
	}

	public void ContinueGame()
	{
		GameManager.instance.currentDay = PlayerPrefs.GetInt("currentDay", 0);
		GameManager.instance.currentCustomer = 0;
		NextDayScreen();
	}

	public void BackToMainMenu()
	{
		fadeToMainmenuCor = StartCoroutine(fadeToMainMenu());
	}

	public void SetCameFromPauseMenu(bool b)
	{
		cameFromPauseMenu = b;
	}

	public void CloseSettings()
	{
		settingsScreen.SetActive(value: false);
		if (cameFromPauseMenu)
		{
			pauseMenu.SetActive(value: true);
		}
		else
		{
			mainMenuScreen.SetActive(value: true);
		}
	}

	public void PauseGame()
	{
		PlayerController.blockGlobal = true;
		PlayerController.instance.LockCursor(b: true);
		pauseMenu.SetActive(value: true);
		SetCrosshair(b: false);
	}

	public void ResumeGame()
	{
		PlayerController.blockGlobal = false;
		PlayerController.instance.LockCursor();
		pauseMenu.SetActive(value: false);
		SetCrosshair(b: true);
	}

	private void CheckContinueBtn()
	{
		continueBtn.gameObject.SetActive(PlayerPrefs.HasKey("currentDay"));
	}

	public void SetVolume(float value)
	{
		float master = Mathf.Log10(value) * 20f;
		AudioController.instance.SetMaster(master);
		PlayerPrefs.SetFloat("volumePref", value);
	}

	public void SetSensitivity(float value)
	{
		PlayerController.instance.SetSensitivity(value);
		PlayerPrefs.SetFloat("sensitivityValue", value);
		sensitivityValueText.text = value.ToString("F2");
	}

	public void SetButtonsHint(bool b, bool secondB = false)
	{
		buttonsHint.SetActive(b);
		useButton.SetActive(secondB);
	}

	public void OpenX()
	{
		Application.OpenURL("https://x.com/vivigamedev");
	}

	public void OpenItchIo()
	{
		Application.OpenURL("https://thevivi.itch.io/");
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
