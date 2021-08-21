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
    public Text ammoBackupText;
    public Text ammoCurrentMagText;
    public Text weaponText;
    public Text timeText;
    public Text skillText;
    public Text EndScoreText;
    public GameObject crossHairReticles;
    public Image img_bulletsIcon;
    public Image img_reloadRing;
    public GameObject[] TrialUI = new GameObject[5];
    public bool[] TrialUIEnableStatus = new bool[5];

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
        ammoBackupText.enabled = false;
        ammoCurrentMagText.enabled = false;
        weaponText.enabled = false;
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
        ammoBackupText.enabled = true;
        ammoCurrentMagText.enabled = true;
        weaponText.enabled = true;
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        ammoBackupText.enabled = false;
        ammoCurrentMagText.enabled = false;
        weaponText.enabled = false;
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
        ammoBackupText.enabled = false;
        ammoCurrentMagText.enabled = false;
        weaponText.enabled = false;
        timeText.enabled = false;
        skillText.enabled = false;
        crossHairReticles.SetActive(true);
        img_bulletsIcon.enabled = false;
        img_reloadRing.enabled = false;
        EndScoreText.text = "Oh Shit! You're a BADASS!";
        EndScoreText.enabled = true;
        EndButton.SetActive(true);
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
    public void SetMouseSensitivitySlider(float sens)
    {
        PlayerPrefs.SetFloat("mouseSensitivity", sens);
    }
    public void SetFOVSlider(float fov)
    {
        PlayerPrefs.SetFloat("FOV", fov);
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
}
