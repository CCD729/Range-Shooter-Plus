using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadRingAnim : MonoBehaviour
{
    public float reloadTime = 1.5f;
    public Image Ring;

    private float t = 0f;
    private bool play = false;

    private bool gamePlaying = true;

    void Update()
    {
        if (gamePlaying)
        {
            if (t >= 1f)
            {
                play = false;
                t = 0f;
                Ring.enabled = false;
            }
            if (play)
            {
                t += Time.deltaTime / reloadTime;
                Ring.fillAmount = Mathf.Lerp(0f, 1f, t);
            }
        }
        
    }

    public void Play()
    {
        t = 0f;
        Ring.fillAmount = 0;
        play = true;
    }
    public void Pause()
    {
        gamePlaying = false;
    }
    public void Resume()
    {
        gamePlaying = true;
    }
    public void Complete()
    {
        Ring.fillAmount = 0;
    }
}
