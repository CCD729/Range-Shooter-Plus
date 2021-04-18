using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private MeshRenderer mr;
    private Vector3 initPosition;
    private bool recovering;
    public bool hit = false;
    public Material hitMaterial;
    private Material initMaterial;
    public bool recoverable = false;

    IEnumerator ChangeMaterial(float time)
    {
        if (!hit)
        {
            yield return new WaitForSeconds(time);
            mr.material = hitMaterial;
        }
        else
            mr.material = initMaterial;
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
        initPosition = transform.localPosition;
        initMaterial = mr.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (hit && !recovering)
        {
            if (recoverable)
            {
                recovering = true;
                StartCoroutine(Recover(5f));
            }
        }
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
    IEnumerator Recover(float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = initPosition;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        StartCoroutine(ChangeMaterial(0f));
        hit = false;
        recovering = false;
    }
}
