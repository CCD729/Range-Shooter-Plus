using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAudioManager : MonoBehaviour
{
    public AudioSource playerAS;
    public AudioClip trialStart, trialComplete;
    public void TrialStartSound()
    {
        playerAS.PlayOneShot(trialStart, 0.5F);
    }
    public void TrialCompleteSound()
    {
        playerAS.PlayOneShot(trialComplete, 0.5F);
    }
}
