using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private MeshRenderer mr;
    public Vector3 initPosition;
    public Vector3 initRotationEuler;
    private bool recovering;
    //public bool hit = false;
    public Material hitMaterial;
    private Material initMaterial;
    public bool recoverable = false;
    public float recoverTime = 5f;
    public bool damageVariant = false;
    public bool physicsReaction = false;
    public bool kinematicWhenActive = false;
    public bool changeMaterial = false;
    public bool reactionTrialUse = false;
    public bool TimedTrialUse = false;
    public bool DisplayUse = false;
    public bool FreemoveTrialUse = false;
    public bool redBoxhitMaterialChange = false;
    public bool damageTaking = false;
    public bool damageTakingDefault = false;
    public bool targetDown = false;
    public bool damageDisplay = true;
    public bool damageDisplayDefault = true;
    public int hitPoints = 100;
    public int maxHitPoints = 100;
    public int pointWorth = 100;
    public GameObject eventSystem;

    IEnumerator ChangeMaterial(float time)
    {
        yield return new WaitForSeconds(time);
        if(targetDown)
            mr.material = hitMaterial;
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
        if (!FreemoveTrialUse)
        {
            initPosition = transform.localPosition;
            initRotationEuler = transform.localRotation.eulerAngles;
        }
        initMaterial = mr.material;
        if (kinematicWhenActive)
        {
            rb.isKinematic = true;
        }

        if (TimedTrialUse)
        {
            damageTaking = false;
            damageDisplay = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetDown && !recovering)
        {
            if (recoverable)
            {
                recovering = true;
                StartCoroutine(Recover(recoverTime));
            }
        }
    }

    //Should TargetDown Pair with animations? if not all distinguishable

    //TargetDown for weapon damage
    void TargetDown(Vector3 hitPos, Vector3 hitDir)
    {
        if (changeMaterial)
            StartCoroutine(ChangeMaterial(0.1f));
        if (kinematicWhenActive)
            rb.isKinematic = false;
        if (physicsReaction)
            StartCoroutine(PhysicsPush(0.1f, hitPos, hitDir));
        targetDown = true;
        damageTaking = false;
        damageDisplay = false;

        //Temporary
        if (TimedTrialUse)
        {
            transform.parent.GetComponent<MovingTargetContainerBehavior>().FinishDown();
            //Consider integrate reaction trial target behavior here. Lock takingDamage until trial start to prevent false scoring
            eventSystem.GetComponent<TrialScript>().timedTrialScore += pointWorth;
        }
        if (DisplayUse)
        {
            transform.GetComponent<animController>().TargetPopDownAnimation();
        }
        if (FreemoveTrialUse)
        {
            transform.parent.GetComponent<MovingTargetContainerBehavior>().FinishDown();
            if (++eventSystem.GetComponent<TrialScript>().freemoveTrialTargetCounter == eventSystem.GetComponent<TrialScript>().freemoveTrialTargetAmount)
                eventSystem.GetComponent<TrialScript>().FreemoveTrialDataRecord();
        }

    }

    //TargetDown for other damage sources
    void TargetDown()
    {
        if (changeMaterial)
        {
            StartCoroutine(ChangeMaterial(0.1f));
        }
        targetDown = true;
        damageTaking = false;
        damageDisplay = false;


        if (TimedTrialUse)
        {
            transform.parent.GetComponent<MovingTargetContainerBehavior>().FinishDown();
            //Consider integrate reaction trial target behavior here. Lock takingDamage until trial start to prevent false scoring
            eventSystem.GetComponent<TrialScript>().timedTrialScore += pointWorth;
        }
        if (DisplayUse)
        {
            transform.parent.GetComponent<MovingTargetContainerBehavior>().FinishDown();
        }
        if (FreemoveTrialUse)
        {
            transform.parent.GetComponent<MovingTargetContainerBehavior>().FinishDown();
            if (++eventSystem.GetComponent<TrialScript>().freemoveTrialTargetCounter == eventSystem.GetComponent<TrialScript>().freemoveTrialTargetAmount)
                eventSystem.GetComponent<TrialScript>().FreemoveTrialDataRecord();
        }
    }

    IEnumerator Recover(float time)
    {
        yield return new WaitForSeconds(time);
        transform.localPosition = initPosition;
        transform.localRotation = Quaternion.Euler(initRotationEuler);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (changeMaterial)
            StartCoroutine(ChangeMaterial(0f));
        if (kinematicWhenActive)
            rb.isKinematic = true;
        recovering = false;
        targetDown = false;
        damageTaking = damageTakingDefault;
        damageDisplay = damageDisplayDefault;
        hitPoints = maxHitPoints;
    }
    //Damage behavior from weapon
    public void DamageBehavior(bool isCritical, int damage, Vector3 hitPos, Vector3 hitDir)
    {
        int realDamage = isCritical ? damage * 2 : damage; 
        //If the damage should trigger something... like hitPoint lost
        if(reactionTrialUse && eventSystem.GetComponent<ShootingScript>().currentTrial == 1 && eventSystem.GetComponent<TrialScript>().reactionTrialTargetUp)
        {
            //Reaction Trial target hit
            eventSystem.GetComponent<TrialScript>().ReactionTrialDataRecord();
            eventSystem.GetComponent<TrialScript>().StopTrial();
            damageDisplay = false;
        }
        else if (!reactionTrialUse)
        {
            if (damageTaking && !targetDown)
            {
                if (hitPoints <= realDamage)
                {
                    hitPoints = 0;
                    TargetDown(hitPos, hitDir);
                }
                else
                {
                    hitPoints -= realDamage;
                }
            }
            else if (targetDown)
            {
                if (physicsReaction)
                    StartCoroutine(PhysicsPush(0.1f, hitPos, hitDir));
            }
        }
    }
    //Damage behavior from Grenade (explosions)
    public void DamageBehavior(int damage, float force, Vector3 center, float radius, float explosionUpwardModifier)
    {
        //If the damage should trigger something... like hitPoint lost
        if (!reactionTrialUse)
        {
            if (damageTaking && !targetDown)
            {
                if (hitPoints <= damage)
                {
                    hitPoints = 0;
                    TargetDown();
                    if (kinematicWhenActive)
                        rb.isKinematic = false;
                    if (physicsReaction)
                        rb.AddExplosionForce(force, center, radius, explosionUpwardModifier, ForceMode.Impulse);
                }
                else
                {
                    hitPoints -= damage;
                }
            }
            else if (targetDown)
            {
                if (physicsReaction)
                    rb.AddExplosionForce(force, center, radius, explosionUpwardModifier, ForceMode.Impulse);
            }
        }
    }

    //Damage behavior from other sources (No physics or physics are already done)
    public void DamageBehavior(int damage)
    {
        //If the damage should trigger something... like hitPoint lost
        if (!reactionTrialUse)
        {
            if (damageTaking)
            {
                if (hitPoints <= damage)
                {
                    hitPoints = 0;
                    TargetDown();
                    if (kinematicWhenActive)
                        rb.isKinematic = false;
                }
                else
                {
                    hitPoints -= damage;
                }
            }
        }
    }
   public void ResetMaterial()
    {
        mr.material = initMaterial;
    }
}
