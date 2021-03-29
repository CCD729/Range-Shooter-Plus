using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public Text scoreText, ammoText, timeText, EndScoreText;
    public GameObject ContinueButton, RestartButton, EndButton, SensSlider, gun;
    public Image crossHair, Ammo, Ring;
    public Camera cam;
    List<string> endList;
    private bool reloading = false;

    void Start()
    {
        endList = new List<string> { "Bravo! ", "WTF? ", "Wow! ", "Awesome! " };
        ContinueButton.SetActive(false);
        RestartButton.SetActive(false);
        EndButton.SetActive(false);
        EndScoreText.enabled = false;
		SensSlider.SetActive(false);
        Ammo.enabled = false;
        Ring.enabled = false;
    }
    void Update()
    {
    }

    public void Pause(bool reloadingStatus)
    {
        if (reloadingStatus)
            gun.GetComponent<SoundScript>().reloadSound.Pause();
        reloading = reloadingStatus;
        scoreText.enabled = false;
        ammoText.enabled = false;
        timeText.enabled = false;
        crossHair.enabled = false;
        Ammo.enabled = false;
        Ring.enabled = false;
        ContinueButton.SetActive(true);
        RestartButton.SetActive(true);
        EndButton.SetActive(true);
		SensSlider.SetActive(true);
        cam.GetComponent<CameraController>().StopCam();
        Ring.GetComponent<ReloadRingAnim>().Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Resume()
    {
        scoreText.enabled = true;
        ammoText.enabled = true;
        timeText.enabled = true;
        ContinueButton.SetActive(false);
        RestartButton.SetActive(false);
		SensSlider.SetActive(false);
        EndButton.SetActive(false);
        if (reloading)
        {
            Ammo.enabled = true;
            Ring.enabled = true;
            reloading = true;
            gun.GetComponent<SoundScript>().reloadSound.UnPause();
        }
        else
        {
            crossHair.enabled = true;
        }
        Ring.GetComponent<ReloadRingAnim>().Resume();
        cam.GetComponent<CameraController>().ResumeCam();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void End(float score)
    {
        if (gun.GetComponent<SoundScript>().reloadSound.isPlaying)
            gun.GetComponent<SoundScript>().reloadSound.Pause();
        scoreText.enabled = false;
        ammoText.enabled = false;
        timeText.enabled = false;
        crossHair.enabled = false;
        Ammo.enabled = false;
        Ring.enabled = false;
        var random = new Random();
        int index = Random.Range(0,3);
        EndScoreText.text = endList[index] + "You got: " + score.ToString();
        EndScoreText.enabled = true;
        EndButton.SetActive(true);
        cam.GetComponent<CameraController>().StopCam();
        Ring.GetComponent<ReloadRingAnim>().Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Perfect()
    {
        if (gun.GetComponent<SoundScript>().reloadSound.isPlaying)
            gun.GetComponent<SoundScript>().reloadSound.Pause();
        scoreText.enabled = false;
        ammoText.enabled = false;
        timeText.enabled = false;
        crossHair.enabled = false;
        Ammo.enabled = false;
        Ring.enabled = false;
        EndScoreText.text = "Oh Shit! You're a BADASS!";
        EndScoreText.enabled = true;
        EndButton.SetActive(true);
        cam.GetComponent<CameraController>().StopCam();
        Ring.GetComponent<ReloadRingAnim>().Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Restart()
    {
        SceneManager.LoadScene("Level");
    }
}
