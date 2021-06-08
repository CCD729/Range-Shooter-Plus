using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    [Header("Gameplay UI Elements")]
    public Text scoreText;
    public Text ammoText;
    public Text timeText;
    public Text skillText;
    public Text EndScoreText;
    public Image img_crossHair;
    public Image img_bulletsIcon;
    public Image img_reloadRing;

    [Header("Pause Menu UI Elements")]
    public GameObject ContinueButton;
    public GameObject RestartButton;
    public GameObject EndButton;
    public GameObject SensSlider;
    public GameObject fovSlider;
    
    [Header("Other References")]
    public GameObject currentWeapon;
    public Camera cam;
    public ShootingScript gameRules;
    
    
    List<string> endList;
    private bool reloading = false;
    private string currentScene;

    void Start()
    {
        endList = new List<string> { "Bravo! ", "WTF? ", "Wow! ", "Awesome! " };
        ContinueButton.SetActive(false);
        RestartButton.SetActive(false);
        EndButton.SetActive(false);
        EndScoreText.enabled = false;
		SensSlider.SetActive(false);
        fovSlider.SetActive(false);
        img_bulletsIcon.enabled = false;
        img_reloadRing.enabled = false;
        currentScene = SceneManager.GetActiveScene().name;
    }
    void Update()
    {
    }

    public void Pause(bool reloadingStatus)
    {
        if (reloadingStatus)
            currentWeapon.GetComponent<SoundScript>().reloadSound.Pause();
        reloading = reloadingStatus;
        scoreText.enabled = false;
        ammoText.enabled = false;
        timeText.enabled = false;
        skillText.enabled = false;
        img_crossHair.enabled = false;
        img_bulletsIcon.enabled = false;
        img_reloadRing.enabled = false;
        ContinueButton.SetActive(true);
        RestartButton.SetActive(true);
        EndButton.SetActive(true);
		SensSlider.SetActive(true);
        fovSlider.SetActive(true);
        cam.GetComponent<CameraController>().StopCam();
        img_reloadRing.GetComponent<ReloadRingAnim>().Pause();
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
        ammoText.enabled = true;
        skillText.enabled = true;
        ContinueButton.SetActive(false);
        RestartButton.SetActive(false);
		SensSlider.SetActive(false);
        fovSlider.SetActive(false);
        EndButton.SetActive(false);
        if (reloading)
        {
            img_bulletsIcon.enabled = true;
            img_reloadRing.enabled = true;
            reloading = true;
            currentWeapon.GetComponent<SoundScript>().reloadSound.UnPause();
        }
        else
        {
            img_crossHair.enabled = true;
        }
        img_reloadRing.GetComponent<ReloadRingAnim>().Resume();
        cam.GetComponent<CameraController>().ResumeCam();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void End(float score)
    {
        if (currentWeapon.GetComponent<SoundScript>().reloadSound.isPlaying)
            currentWeapon.GetComponent<SoundScript>().reloadSound.Pause();
        scoreText.enabled = false;
        ammoText.enabled = false;
        timeText.enabled = false;
        skillText.enabled = false;
        img_crossHair.enabled = false;
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
        if (currentWeapon.GetComponent<SoundScript>().reloadSound.isPlaying)
            currentWeapon.GetComponent<SoundScript>().reloadSound.Pause();
        scoreText.enabled = false;
        ammoText.enabled = false;
        timeText.enabled = false;
        skillText.enabled = false;
        img_crossHair.enabled = false;
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
        SceneManager.LoadScene("MainMenu");
    }
    public void Restart()
    {
        SceneManager.LoadScene(currentScene);
    }
    public void UpdateWeaponInfo()
    {
        currentWeapon = gameRules.currentWeapon;
    }
}
