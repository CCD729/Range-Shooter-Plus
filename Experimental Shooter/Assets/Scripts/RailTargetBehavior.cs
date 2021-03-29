using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTargetBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private MeshRenderer mr;
    private Animator animator;
    public bool hit = false;
    public Material hitMaterial;

    IEnumerator ChangeMaterial(float time)
    {
        yield return new WaitForSeconds(time);
        mr.material = hitMaterial;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    IEnumerator PhysicsPush(float time, Vector3 hitPos, Vector3 hitDir)
    {
        yield return new WaitForSeconds(time);
        rb.AddForceAtPosition(hitDir, hitPos, ForceMode.Impulse);
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void Hit(Vector3 hitPos, Vector3 hitDir)
    {
        if (!hit)
        {
            StartCoroutine(ChangeMaterial(0.1f));
            hit = true;
            Destroy(animator);
        }
        StartCoroutine(PhysicsPush(0.1f, hitPos, hitDir));
    }
}
