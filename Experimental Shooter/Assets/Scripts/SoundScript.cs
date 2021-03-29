using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public AudioSource reloadSound, shootSound;
    public AudioClip reloadclip, shootclip;

    public void ReloadSound()
    {
        reloadSound.PlayOneShot(reloadclip, 0.5F);
    }
    public void ShootSound()
    {
        shootSound.PlayOneShot(shootclip, 0.5F);
    }
}
