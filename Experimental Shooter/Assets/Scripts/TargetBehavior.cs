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
    public bool damageVariant = false;
    public bool physicsReaction = false;
    public bool changeMaterial = false;
    public bool reactionTrialUse = false;
    public bool damageTaking = false;
    public int hitPoints = 100;

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
        rb.AddForceAtPosition(hitDir*2, hitPos, ForceMode.Impulse);
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
        if (!hit && changeMaterial)
        {
            StartCoroutine(ChangeMaterial(0.1f));
            hit = true;
        }
        if(physicsReaction)
            StartCoroutine(PhysicsPush(0.1f, hitPos, hitDir));
    }

    public void HitByProjectile()
    {
        if (!hit)
        {
            StartCoroutine(ChangeMaterial(0.1f));
            hit = true;
        }
    }

    IEnumerator Recover(float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = initPosition;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        if(changeMaterial)
            StartCoroutine(ChangeMaterial(0f));
        hit = false;
        recovering = false;
    }
    public void DamageBehavior(bool isCritical, int damage)
    {
        //If the damage should trigger something... like hitPoint lost
    }
}
