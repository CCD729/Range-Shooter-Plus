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
    public float backgroundUICurrentHeight;
    public float backgroundUITargetHeight = 700f;
    public float gunTargetY = 0f;
    public float gunRotateSpeed = 10f;
    public float menuUIFadeSpeed = 1f;
    public float settingsUIFadeSpeed = 1f;
    public float backgroundUITweenSpeed = 1f;
    public float gunTweenSpeed = 1f;

    public CanvasGroup menuUI;
    public CanvasGroup settingsUI;
    public GameObject backgroundUIImage;
    public GameObject gunDisplayCenter;  //Warning: Center pivit didn't work somehow, unpacked prefab and adjusted pivot to be same. Weird behavior

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
        backgroundUITargetHeight = 700f;
        backgroundUICurrentHeight = 700f;
        Time.timeScale = 1f;
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 20f);
        fieldOfView = PlayerPrefs.GetFloat("FOV", 90f);
        sensSlider.value = mouseSensitivity;
        fovSlider.value = fieldOfView;
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
        backgroundUICurrentHeight = Mathf.Lerp(backgroundUICurrentHeight, backgroundUITargetHeight, backgroundUITweenSpeed * Time.deltaTime);
        backgroundUIImage.GetComponent<RectTransform>().sizeDelta = new Vector2(backgroundUIImage.GetComponent<RectTransform>().sizeDelta.x, backgroundUICurrentHeight);
        gunDisplayCenter.transform.localPosition = new Vector3(gunDisplayCenter.transform.position.x, Mathf.Lerp(gunDisplayCenter.transform.position.y, gunTargetY, gunTweenSpeed * Time.deltaTime), gunDisplayCenter.transform.position.z);
        gunDisplayCenter.transform.localRotation = Quaternion.Euler(new Vector3(gunDisplayCenter.transform.rotation.eulerAngles.x, gunDisplayCenter.transform.rotation.eulerAngles.y+gunRotateSpeed*Time.deltaTime, gunDisplayCenter.transform.rotation.eulerAngles.z));
        
        //Alternate keyboard control for menu/settings
        if(menuState == 1 && Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

   /* public void StartTimeTrial()
    {
        SceneManager.LoadScene("TimeTrial");
    }*/
    public void StartShootingRange()
    {
        StartCoroutine(DelayEnterLevel(0.4f));
    }
    public void OpenSettings()
    {
        menuState = 1;
        menuUI.interactable = false;
        menuUI.blocksRaycasts = false;
        menuUITargetAlpha = 0f;
        settingsUITargetAlpha = 1f;
        backgroundUITargetHeight = 630f;
        gunTargetY = -2f;
        StartCoroutine(EnableSettingsInteraction(0.3f));
    }
    public void BackToMenu()
    {
        menuState = 0;
        settingsUI.interactable = false;
        settingsUI.blocksRaycasts = false;
        menuUITargetAlpha = 1f;
        settingsUITargetAlpha = 0f;
        backgroundUITargetHeight = 700f;
        gunTargetY = 0f;
        StartCoroutine(EnableMenuInteraction(0.3f));
    }
    public void ExitGame()
    {
        StartCoroutine(DelayExit(0.4f));
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

    IEnumerator DelayEnterLevel(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("ShootingRange");
    }
    IEnumerator DelayExit(float time)
    {
        yield return new WaitForSeconds(time);
        Application.Quit();
    }
}
