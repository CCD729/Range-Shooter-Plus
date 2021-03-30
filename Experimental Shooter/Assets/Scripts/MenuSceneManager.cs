using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour
{
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
        //TO DO: SettingsMenu
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
