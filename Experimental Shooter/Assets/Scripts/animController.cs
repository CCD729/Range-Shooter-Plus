using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ReloadAnimation()
    {
        animator.Play("Reload", -1, 0f);
    }
}
