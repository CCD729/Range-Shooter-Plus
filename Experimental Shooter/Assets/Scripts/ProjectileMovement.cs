using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField]
    private float timeoutLifetime = 5f;
    [SerializeField]
    private bool destroySelfAfterTimeout = true;
    [SerializeField]
    private float speed = 8f;

    public Vector3 spawnPoint;
    private Rigidbody rb;
    private float destroyTime;

    void Start()
    {   //Set timer 
        destroyTime = Time.time + timeoutLifetime;
        spawnPoint = transform.position;

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
        //TODO: Particle/SFX

        this.Destroy();
    }
    public void Destroy()
    {
        //TODO: SFX and PARTICLES
        Destroy(gameObject);
    }
}
