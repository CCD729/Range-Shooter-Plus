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

    public void ReloadAnimation()
    {
        animator.Play("Reload", -1, 0f);
    }
    public void SkillPutdownAnimation()
    {
        animator.Play("SkillPutdown", -1, 0f);
    }
    public void ShootAnimation()
    {
        animator.Play("Shoot", -1, 0f);
    }
}
