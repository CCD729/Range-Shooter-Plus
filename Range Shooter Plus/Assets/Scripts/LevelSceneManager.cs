using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class LevelSceneManager : MonoBehaviour
{
    [Header("Gameplay UI Elements")]
    public Text scoreText;
    public Text timeText;
    public Text skillText;
    public Text EndScoreText;
    public GameObject crossHairReticles;
    public Image img_bulletsIcon;
    public Image img_reloadRing;
    public GameObject[] TrialUI = new GameObject[5];
    public bool[] TrialUIEnableStatus = new bool[5];
    public GameObject[] weaponHUD = new GameObject[7];

    [Header("Pause Menu UI Elements")]
    public GameObject ContinueButton;
    public GameObject RestartButton;
    public GameObject EndButton;
    public GameObject SensSlider;
    public GameObject fovSlider;
    public GameObject PauseBackgroundImage;

    [Header("Post Processing Values")]
    public PostProcessVolume volume;
    public Bloom bloom;
    public DepthOfField dof;

    [Header("Other References")]
    public GameObject currentWeapon;
    public Camera cam;
    public ShootingScript gameRules;
    public float lerpSpeed = 100f;
    public float targetDOFValue = 5f;
    public bool achievementPoped = false;
    public bool achievementPop = false;
    public bool achievementBack = false;
    public float achievementMoveSpeed = 1f;
    public GameObject achievementBlock;


    List<string> endList;
    private bool reloading = false;
    private string currentScene;

    void Start()
    {
        endList = new List<string> { "Bravo! ", "WTF? ", "Wow! ", "Awesome! " };
        ContinueButton.SetActive(false);
        RestartButton.SetActive(false);
        EndButton.SetActive(false);
        PauseBackgroundImage.SetActive(false);
        EndScoreText.enabled = false;
		SensSlider.SetActive(false);
        fovSlider.SetActive(false);
        img_bulletsIcon.enabled = false;
        img_reloadRing.enabled = false;
        currentScene = SceneManager.GetActiveScene().name;

        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out dof);
    }

    void Update()
    {
        dof.focusDistance.value = Mathf.Lerp(dof.focusDistance, targetDOFValue, Time.unscaledDeltaTime * lerpSpeed);

        if (achievementPop)
        {
            if (achievementBlock.GetComponent<RectTransform>().anchoredPosition.y >= 25f)
                achievementBlock.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, achievementBlock.GetComponent<RectTransform>().anchoredPosition.y - achievementMoveSpeed);
            else
            {
                achievementBlock.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, 25f);
                achievementPop = false;
            }
        }
        if (achievementBack)
        {
            if (achievementBlock.GetComponent<RectTransform>().anchoredPosition.y <= 250f)
                achievementBlock.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, achievementBlock.GetComponent<RectTransform>().anchoredPosition.y + achievementMoveSpeed);
            else
            {
                achievementBlock.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10f, 250f);
                achievementBack = false;
            }
        }
    }

    public void Pause(bool reloadingStatus)
    {
        if (gameRules.weaponEquipped)
        {
            currentWeapon.GetComponent<SoundScript>().weaponManipulationSound.Pause();
            currentWeapon.GetComponent<SoundScript>().gunShootSound.Pause();
            currentWeapon.GetComponent<SoundScript>().etcSound.Pause();
        }

        //Post processing
        targetDOFValue = 0.1f;

        reloading = reloadingStatus;
        scoreText.enabled = false;
        timeText.enabled = false;
        skillText.enabled = false;
        crossHairReticles.SetActive(false);
        img_bulletsIcon.enabled = false;
        img_reloadRing.enabled = false;
        ContinueButton.SetActive(true);
        RestartButton.SetActive(true);
        EndButton.SetActive(true);
		SensSlider.SetActive(true);
        fovSlider.SetActive(true);
        PauseBackgroundImage.SetActive(true);
        cam.GetComponent<CameraController>().StopCam();
        img_reloadRing.GetComponent<ReloadRingAnim>().Pause();
        for(int i=0; i< TrialUI.Length; i++)
        {
            TrialUIEnableStatus[i] = TrialUI[i].activeInHierarchy;
            TrialUI[i].SetActive(false);
        }
        foreach (GameObject element in weaponHUD)
        {
            element.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Resume()
    {
        if (gameRules.mode_Scored)
        {
            scoreText.enabled = true;
        }
        if (gameRules.mode_Timed)
        {
            timeText.enabled = true;
        }

        targetDOFValue = 3f;
        skillText.enabled = true;
        ContinueButton.SetActive(false);
        RestartButton.SetActive(false);
		SensSlider.SetActive(false);
        fovSlider.SetActive(false);
        EndButton.SetActive(false);
        PauseBackgroundImage.SetActive(false);
        if (reloading)
        {
            img_bulletsIcon.enabled = true;
            img_reloadRing.enabled = true;
            reloading = true;
        }
        else
        {
            crossHairReticles.SetActive(true);
        }
        if (gameRules.weaponEquipped)
        {
            currentWeapon.GetComponent<SoundScript>().weaponManipulationSound.UnPause();
            currentWeapon.GetComponent<SoundScript>().gunShootSound.UnPause();
            currentWeapon.GetComponent<SoundScript>().etcSound.UnPause();
        }
        img_reloadRing.GetComponent<ReloadRingAnim>().Resume();
        cam.GetComponent<CameraController>().ResumeCam();
        for (int i = 0; i < TrialUI.Length; i++)
        {
            TrialUI[i].SetActive(TrialUIEnableStatus[i]);
        }
        foreach (GameObject element in weaponHUD)
        {
            element.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EasterEggEvent()
    {
        if (!achievementPoped)
        {
            achievementPoped = true;
            StartCoroutine(AchievementPopSequence(6f));
        }
    }

    //LEGACY (NO LONGER IN USE)
    public void End(float score)
    {
        if (gameRules.weaponEquipped)
        {
            currentWeapon.GetComponent<SoundScript>().weaponManipulationSound.Pause();
            currentWeapon.GetComponent<SoundScript>().gunShootSound.Pause();
            currentWeapon.GetComponent<SoundScript>().etcSound.Pause();
        }
        targetDOFValue = 0.1f;
        scoreText.enabled = false;
        timeText.enabled = false;
        skillText.enabled = false;
        crossHairReticles.SetActive(false);
        img_bulletsIcon.enabled = false;
        img_reloadRing.enabled = false;
        //var random = new Random();
        int index = Random.Range(0,3);
        EndScoreText.text = endList[index] + "You got: " + score.ToString();
        EndScoreText.enabled = true;
        EndButton.SetActive(true);
        foreach (GameObject element in weaponHUD)
        {
            element.SetActive(false);
        }
        cam.GetComponent<CameraController>().StopCam();
        img_reloadRing.GetComponent<ReloadRingAnim>().Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    public void Perfect()
    {
        if (gameRules.weaponEquipped)
        {
            currentWeapon.GetComponent<SoundScript>().weaponManipulationSound.Pause();
            currentWeapon.GetComponent<SoundScript>().gunShootSound.Pause();
            currentWeapon.GetComponent<SoundScript>().etcSound.Pause();
        }
        targetDOFValue = 0.1f;
        scoreText.enabled = false;
        timeText.enabled = false;
        skillText.enabled = false;
        crossHairReticles.SetActive(true);
        img_bulletsIcon.enabled = false;
        img_reloadRing.enabled = false;
        EndScoreText.text = "Oh Shit! You're a BADASS!";
        EndScoreText.enabled = true;
        EndButton.SetActive(true);
        foreach (GameObject element in weaponHUD)
        {
            element.SetActive(false);
        }
        cam.GetComponent<CameraController>().StopCam();
        img_reloadRing.GetComponent<ReloadRingAnim>().Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void BackToMenu()
    {
        StartCoroutine(BackToMainMenu(0.4f));
    }
    public void Restart()
    {
        StartCoroutine(RestartLevel(0.4f));
    }
    public void UpdateWeaponInfo()
    {
        if(gameRules.currentWeapon != null)
            currentWeapon = gameRules.currentWeapon;
        else
            currentWeapon = null;
    }
    IEnumerator RestartLevel(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        SceneManager.LoadScene(currentScene);
    }
    IEnumerator BackToMainMenu(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        SceneManager.LoadScene("MainMenu");
    }
    IEnumerator AchievementPopSequence(float time)
    {
        achievementPop = true;
        achievementBlock.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSecondsRealtime(time);
        achievementBack = true;
        yield return new WaitForSecondsRealtime(2f);
        achievementBlock.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
