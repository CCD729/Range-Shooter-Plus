using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField]
    private float timeoutLifetime = 2f;
    [SerializeField]
    private bool destroySelfAfterTimeout = true, destroySelfOnCollision = true;
    [SerializeField]
    private float speed = 80f;

    public bool hit = false;
    public Vector3 hitPoint;

    //Counter
    private Vector3 startPoint;
    private float destroyTime;
    //private float t = 0f;

    void Start()
    {   //Set timer and memorize starting point for lerp
        destroyTime = Time.time + timeoutLifetime;
        startPoint = transform.position;
    }
    void FixedUpdate()
    {   
        if (!hit)
        {
            //Make the bullet fly
            transform.Translate(Vector3.up * Time.fixedDeltaTime * speed);
        }
        else
        {
            //lerp the bullet to target pos
            //t += Time.deltaTime * speed/6f;
            //transform.position = Vector3.Lerp(startPoint, hitPoint, t);
            transform.position = Vector3.MoveTowards(transform.position, hitPoint, Time.deltaTime * speed);
        }
        /* if (t >= 1f)
         {
             this.Destroy();
         }*/
        if (Vector3.Distance(transform.position, hitPoint) < 0.1f)
        {
            //It is within ~0.1f range
            this.Destroy();
        }

        //Destroy the bullet when time out
        if (destroySelfAfterTimeout && destroyTime <= Time.time)
            this.Destroy();
    }
    void OnCollisionEnter(Collision collision)
    {
        //Destroy the bullet on collision
        if (destroySelfOnCollision)
        {
            //TODO: Particle/SFX
            this.Destroy();
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
    public void Destroy()
    {
        //TODO: SFX and PARTICLES
        Destroy(gameObject);
    }
}
