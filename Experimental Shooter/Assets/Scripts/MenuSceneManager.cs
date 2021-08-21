using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour
{
    //0 = menu, 1 = settings
    public int menuState = 0;
    public float menuUITargetAlpha = 1f;
    public float settingsUITargetAlpha = 0f;
    public float menuUIFadeSpeed = 1f;
    public float settingsUIFadeSpeed = 1f;
    public float backgroundUICurrentHeight;
    public float backgroundUIHeightTarget = 700f;
    public float backgroundUITweenSpeed = 1f;
    public CanvasGroup menuUI;
    public CanvasGroup settingsUI;
    public GameObject backgroundUIImage;

    public GameObject[] popingDisplayContainers;
    public int currentTarget = 0;
    public int maxTarget = 6;
    public Slider sensSlider, fovSlider;
    [SerializeField] private float mouseSensitivity = 20f;
    [SerializeField] private float fieldOfView = 90f;


    void Start()
    {
        menuState = 0;
        menuUI.alpha = 1f;
        settingsUI.alpha = 0f;
        backgroundUIImage.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundUIImage.GetComponent<RectTransform>().sizeDelta.x, 700f);
        backgroundUIHeightTarget = 700f;
        backgroundUICurrentHeight = 700f;
        Time.timeScale = 1f;
        sensSlider.value = mouseSensitivity;
        fovSlider.value = fieldOfView;
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 20f);
        fieldOfView = PlayerPrefs.GetFloat("FOV", 90f);
        foreach (GameObject targetContainer in popingDisplayContainers)
        {
            currentTarget++;
            targetContainer.GetComponent<MovingTargetContainerBehavior>().StartUp();
        }
    }

    void Update()
    {
        menuUI.alpha = Mathf.Lerp(menuUI.alpha, menuUITargetAlpha, menuUIFadeSpeed * Time.deltaTime);
        settingsUI.alpha = Mathf.Lerp(settingsUI.alpha, settingsUITargetAlpha, settingsUIFadeSpeed * Time.deltaTime);
        backgroundUICurrentHeight = Mathf.Lerp(backgroundUICurrentHeight, backgroundUIHeightTarget, backgroundUITweenSpeed * Time.deltaTime);
        backgroundUIImage.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundUIImage.GetComponent<RectTransform>().sizeDelta.x, backgroundUICurrentHeight);
    }

    public void StartTimeTrial()
    {
        SceneManager.LoadScene("TimeTrial");
    }
    public void StartShootingRange()
    {
        SceneManager.LoadScene("ShootingRange");
    }
    public void OpenSettings()
    {
        menuUI.interactable = false;
        menuUI.blocksRaycasts = false;
        menuUITargetAlpha = 0f;
        settingsUITargetAlpha = 1f;
        backgroundUIHeightTarget = 630f;
        StartCoroutine(EnableSettingsInteraction(0.3f));
    }
    public void BackToMenu()
    {
        settingsUI.interactable = false;
        settingsUI.blocksRaycasts = false;
        menuUITargetAlpha = 1f;
        settingsUITargetAlpha = 0f;
        backgroundUIHeightTarget = 700f;
        StartCoroutine(EnableMenuInteraction(0.3f));
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void SetMouseSensitivitySlider(float sens)
    {
        PlayerPrefs.SetFloat("mouseSensitivity", sens);
    }
    public void SetFOVSlider(float fov)
    {
        PlayerPrefs.SetFloat("FOV", fov);
    }
    IEnumerator EnableMenuInteraction(float time)
    {
        yield return new WaitForSeconds(time);
        menuUI.interactable = true;
        menuUI.blocksRaycasts = true;
    }
    IEnumerator EnableSettingsInteraction(float time)
    {
        yield return new WaitForSeconds(time);
        settingsUI.interactable = true;
        settingsUI.blocksRaycasts = true;
    }
}
