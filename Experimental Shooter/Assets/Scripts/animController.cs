using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animController : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

   /* public void ReloadAnimation()
    {
        animator.Play("Reload", -1, 0f);
    }*/
    public void PutdownAnimation()
    {
        animator.Play("Putdown", -1, 0f);
    }
    public void PutdownEmptyMagAnimation()
    {
        animator.Play("PutdownEmptyMag", -1, 0f);
    }
    public void PickupAnimation()
    {
        animator.Play("Pickup", -1, 0f);
    }
    public void PickupEmptyMagAnimation()
    {
        animator.Play("PickupEmptyMag", -1, 0f);
    }
    public void PickupPullActionAnimation()
    {
        animator.Play("PickupPullAction", -1, 0f);
    }
    public void ShootAnimation()
    {
        animator.Play("Shoot", -1, 0f);
    }
    public void ShootLastRoundAnimation()
    {
        animator.Play("ShootLastRound", -1, 0f);
    }
    public void TacticalReloadAnimation()
    {
        animator.Play("TacticalReload", -1, 0f);
    }
    public void EmptyReloadAnimation()
    {
        animator.Play("EmptyReload", -1, 0f);
    }
}
