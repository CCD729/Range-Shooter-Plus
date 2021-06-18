using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public AudioSource weaponManipulationSound, gunShootSound, etcSound;
    public AudioClip emptyReloadClip, tacticalReloadClip, shootClip, pickupClip, putdownClip, dryShootClip;

    public void tacticalReloadSound()
    {
        weaponManipulationSound.PlayOneShot(tacticalReloadClip, 0.5F);
    }
    public void emptyReloadSound()
    {
        weaponManipulationSound.PlayOneShot(emptyReloadClip, 0.5F);
    }
    public void PickupSound()
    {
        weaponManipulationSound.PlayOneShot(pickupClip, 0.5F);
    }
    public void PutdownSound()
    {
        weaponManipulationSound.PlayOneShot(putdownClip, 0.5F);
    }
    public void ShootSound()
    {
        gunShootSound.PlayOneShot(shootClip, 0.5F);
    }
    public void dryShootSound()
    {
        gunShootSound.PlayOneShot(dryShootClip, 0.5F);
    }
    //TODO: shell droping sound with shells
}
