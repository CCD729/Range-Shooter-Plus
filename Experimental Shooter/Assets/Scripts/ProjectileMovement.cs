using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileMovement : MonoBehaviour
{
    enum ProjectileType
    {
        impactGrenade
    }
    private GameObject EventSystem;
    [SerializeField]
    private float timeoutLifetime = 5f;
    [SerializeField]
    private bool destroySelfAfterTimeout = true;
    [SerializeField]
    private float speed = 8f;
    [SerializeField]
    private ProjectileType projectileType;

    [SerializeField]
    private bool Is_Explosive = false;
    [SerializeField]
    private GameObject explosionVisualObj;
    [SerializeField]
    private float explosionRadius = 1f;
    [SerializeField]
    private float explosionForce = 1f;
    [SerializeField]
    private int explosionDamage = 1;
    [SerializeField]
    private float explosionUpwardModifier = 1f;
    [SerializeField]
    private float explosionVisualTime = 1f;
    [SerializeField]
    private float maxExplosionDamageRange = 2f;
    [SerializeField]
    private int minExplosionDamage = 10;

    public Vector3 spawnPoint;
    private Rigidbody rb;
    private float destroyTime;

    public GameObject audioExplosion;

    void Start()
    {
        EventSystem = GameObject.Find("CustomEventSystem");
        //Set timer 
        destroyTime = Time.time + timeoutLifetime;
        spawnPoint = transform.position;

        //type identifier and some extra info
        if (projectileType == ProjectileType.impactGrenade)
        {
            Is_Explosive = true;
            explosionRadius = 8f;
            explosionForce = 7f;
            explosionDamage = 150;
            explosionUpwardModifier = 1.5f;
            explosionVisualTime = 1.5f;
            maxExplosionDamageRange = 3f;
            minExplosionDamage = 30;
        }

        //Let the Projectile fly
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse); //(force, spawnPoint, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        //Destroy the bullet when time out
        if (destroySelfAfterTimeout && destroyTime <= Time.time)
        {
            if (!EventSystem.GetComponent<ShootingScript>().trialScript.grenadeBeforeGone)
                EventSystem.GetComponent<ShootingScript>().trialScript.grenadeBeforeGone = true;
            this.Destroy();
        }
            
    }
    void OnCollisionEnter(Collision collision)
    {
        //Deploy explosion effect on collision
        if (!collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log(collision.gameObject);
            this.Impact();
        }
    }

    /*
    void OnTriggerStay(Collider other)
    {   //Destroy the bullet on collision
        if (destroySelfOnCollision)
            //TODO: Particle/SFX
            this.Destroy();
    }
    */

    //Explosion effects
    public void Impact()
    {
        //TODO: Explosion Behavior
        //TODO: Particle/SFX
        if (Is_Explosive)
        {
            if (projectileType == ProjectileType.impactGrenade)
            {
                ExplosionPhysicsDamage(transform.position, explosionRadius, explosionForce, explosionDamage);
                ExplosionVisual(transform.position, explosionRadius, explosionVisualTime);
                Instantiate(audioExplosion, transform.position, Quaternion.Euler(0, 0, 0)); 
                if (EventSystem.GetComponent<ShootingScript>().currentTrial == 0)
                {
                    if(EventSystem.GetComponent<ShootingScript>().trialScript.grenadeBeforeGone)
                    {
                        EventSystem.GetComponent<ShootingScript>().currentTrial = -1;
                        EventSystem.GetComponent<ShootingScript>().trialScript.text_TrialDataRight.SetActive(false);
                        EventSystem.GetComponent<ShootingScript>().trialScript.text_TrialDataRight.GetComponent<Text>().text = "TrialDataRightTextExample";
                        EventSystem.GetComponent<ShootingScript>().trialScript.GrenadeTrialDataRecord(transform.position.x);
                        EventSystem.GetComponent<ShootingScript>().trialScript.StopTrial();
                    }
                }
                EventSystem.GetComponent<ShootingScript>().trialScript.grenadeBeforeGone = true;
            }
            this.Destroy();
        }
    }
    public void Destroy()
    {
        //TODO: SFX and PARTICLES
        Destroy(gameObject);
    }
    void ExplosionPhysicsDamage(Vector3 center, float radius, float force, int damage)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        List<GameObject> finishedObj = new List<GameObject>();
        int calculatedDamage = 0;
        foreach (var hitCollider in hitColliders)
        {
            //TODO: calculate damage
            //hitCollider.SendMessage("AddDamage");
            if(hitCollider.gameObject.CompareTag("Targets") && LayerMask.LayerToName(hitCollider.gameObject.layer) != "HitablesDamageVariant" && LayerMask.LayerToName(hitCollider.gameObject.layer) != "HitablesDamageCriticalVariant")
            {
                bool repeatedTarget = false;
                foreach (GameObject finishedObject in finishedObj)
                {
                    if (GameObject.ReferenceEquals(finishedObject, hitCollider.gameObject))
                        repeatedTarget = true;
                }
                if (!repeatedTarget)
                {
                    //Test for obstacles
                    int layerMask = 1 << 21; //Layer 21 is HitableWalls
                    if (!Physics.Linecast(hitCollider.transform.position, transform.position, layerMask))
                    {
                        //DAMAGE CALCULATION
                        calculatedDamage = Vector3.Distance(hitCollider.transform.position, center) <= maxExplosionDamageRange ? damage : (int)(minExplosionDamage + (explosionDamage - minExplosionDamage) * (radius - Mathf.Min(Vector3.Distance(hitCollider.transform.position, center), radius)) / (Mathf.Max(Vector3.Distance(hitCollider.transform.position, center), radius) - maxExplosionDamageRange));
                        {
                            if (hitCollider.gameObject.GetComponent<TargetBehavior>().damageDisplay)
                            {
                                var damageDisplay = Instantiate(EventSystem.GetComponent<ShootingScript>().regularDamageDisplayObj, hitCollider.transform.position, Quaternion.Euler(0f, 0f, 0f));
                                //damageDisplay.transform.SetParent(canvas1stCamera.transform);
                                damageDisplay.transform.SetParent(EventSystem.GetComponent<ShootingScript>().canvasHUD.transform);
                                damageDisplay.GetComponent<DamageDisplay>().hitTarget = hitCollider.gameObject;
                                damageDisplay.GetComponent<DamageDisplay>().damageDisplayText.text = calculatedDamage.ToString();
                            }
                        }
                        hitCollider.gameObject.GetComponent<TargetBehavior>().DamageBehavior(calculatedDamage, force, center, radius, explosionUpwardModifier);

                    }
                    finishedObj.Add(hitCollider.gameObject);
                }
            }
            //Require further change to adapt non-target situation or composed colliders
            //if (hitCollider.gameObject.GetComponent<Rigidbody>() != null)
            //    hitCollider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, center, radius, explosionUpwardModifier, ForceMode.Impulse);
            /*if (hitCollider.gameObject.CompareTag("Target") || hitCollider.gameObject.CompareTag("MovingTarget") || hitCollider.gameObject.CompareTag("RailTarget"))
            {
                EventSystem.GetComponent<ShootingScript>().hitByProjectile(hitCollider.gameObject);
            }*/
        }
    }
    void ExplosionVisual(Vector3 center, float radius, float time)
    {
        var visualEffectObj = Instantiate(explosionVisualObj, center, Quaternion.Euler(0,0,0));
        visualEffectObj.GetComponent<ObjectDestroyer>().Destroy();
    }
}
