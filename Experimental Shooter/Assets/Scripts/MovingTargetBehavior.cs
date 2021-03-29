using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetBehavior : MonoBehaviour
{
    public float height = 5f, radius = 5f, speed = 30f, scale = 0.5f;
    
    private Rigidbody rb;
    private MeshRenderer mr;
    public bool hit = false;
    public Material hitMaterial;

    IEnumerator ChangeMaterial(float time)
    {
        yield return new WaitForSeconds(time);
        mr.material = hitMaterial;
        rb.useGravity = true;
    }

    IEnumerator PhysicsPush(float time, Vector3 hitPos, Vector3 hitDir)
    {
        yield return new WaitForSeconds(time);
        rb.AddForceAtPosition(hitDir, hitPos, ForceMode.Impulse);
    }


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(radius, height, 0f);
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        transform.localScale = new Vector3(scale, 0.1f, scale);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hit)
        {
            transform.RotateAround(new Vector3 (0f,height,0f), Vector3.up, speed * Time.deltaTime);
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
}
