using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private MeshRenderer mr;
    public bool hit = false;
    public Material hitMaterial;

    IEnumerator ChangeMaterial(float time)
    {
        yield return new WaitForSeconds(time);
        mr.material = hitMaterial;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(Vector3 hitPos, Vector3 hitDir)
    {
        if (!hit)
        {
            StartCoroutine(ChangeMaterial(0.1f));
            hit = true;
        }
        StartCoroutine(PhysicsPush(0.1f, hitPos, hitDir));
    }
}
