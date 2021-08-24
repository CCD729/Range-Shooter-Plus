using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittingAudioManager : MonoBehaviour
{
    //Hitting sound Clips
    public AudioClip[] hitWall;
    public AudioClip[] hitMetal;
    private bool started = false;

    public AudioSource AS;

    void FixedUpdate()
    {
        if (started && !AS.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    public void Play(bool targetHit)
    {
        int index = Random.Range(0, 8);
        if (targetHit)
        {
            AS.clip = hitMetal[index];
            AS.PlayOneShot(AS.clip, 0.07f);
        }
        else
        {
            AS.clip = hitWall[index];
            AS.PlayOneShot(AS.clip, 0.3f);
        }
        started = true;
    }
}
