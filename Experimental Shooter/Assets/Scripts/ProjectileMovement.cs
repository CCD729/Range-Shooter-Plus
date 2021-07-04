using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            explosionRadius = 6f;
            explosionForce = 6f;
            explosionDamage = 50;
            explosionUpwardModifier = 1f;
            explosionVisualTime = 1.5f;
            maxExplosionDamageRange = 3f;
            minExplosionDamage = 10;
        }

        //Let the Projectile fly
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse); //(force, spawnPoint, ForceMode.Impulse);
    }
    void FixedUpdate()
    {   
        //Destroy the bullet when time out
        if (destroySelfAfterTimeout && destroyTime <= Time.time)
            this.Destroy();
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
                    //TODO: DAMAGE CALCULATION
                    calculatedDamage = Vector3.Distance(hitCollider.transform.position, center) <= maxExplosionDamageRange ? damage : (int)(minExplosionDamage + (explosionDamage - minExplosionDamage) * (radius - Mathf.Min(Vector3.Distance(hitCollider.transform.position, center), radius)) / (Mathf.Max(Vector3.Distance(hitCollider.transform.position, center), radius) - maxExplosionDamageRange));
                    hitCollider.gameObject.GetComponent<TargetBehavior>().DamageBehavior(false, calculatedDamage);
                    var damageDisplay = Instantiate(EventSystem.GetComponent<ShootingScript>().regularDamageDisplayObj, hitCollider.transform.position, Quaternion.Euler(0f, 0f, 0f));
                    //damageDisplay.transform.SetParent(canvas1stCamera.transform);
                    damageDisplay.transform.SetParent(EventSystem.GetComponent<ShootingScript>().canvasHUD.transform);
                    damageDisplay.GetComponent<DamageDisplay>().hitTarget = hitCollider.gameObject;
                    damageDisplay.GetComponent<DamageDisplay>().damageDisplayText.text = calculatedDamage.ToString();
                    if (hitCollider.gameObject.GetComponent<TargetBehavior>().physicsReaction)
                        hitCollider.gameObject.GetComponent<TargetBehavior>().HitByProjectile();
                    finishedObj.Add(hitCollider.gameObject);
                }
            }
            //Require further change to adapt non-target situation or composed colliders
            if (hitCollider.gameObject.GetComponent<Rigidbody>() != null)
                hitCollider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(force, center, radius, explosionUpwardModifier, ForceMode.Impulse);
            if (hitCollider.gameObject.CompareTag("Target") || hitCollider.gameObject.CompareTag("MovingTarget") || hitCollider.gameObject.CompareTag("RailTarget"))
            {
                EventSystem.GetComponent<ShootingScript>().hitByProjectile(hitCollider.gameObject);
            }
        }
    }
    void ExplosionVisual(Vector3 center, float radius, float time)
    {
        var visualEffectObj = Instantiate(explosionVisualObj, center, Quaternion.Euler(0,0,0));
        visualEffectObj.GetComponent<ObjectDestroyer>().Destroy();
    }
}
