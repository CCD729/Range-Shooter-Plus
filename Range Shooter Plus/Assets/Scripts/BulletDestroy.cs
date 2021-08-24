using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroy : MonoBehaviour
{
    [SerializeField]
    private bool destroySelfOnCollision = true;

    void OnCollisionEnter(Collision collision)
    {
        //Destroy the bullet on collision
        if (destroySelfOnCollision)
        {
            if (!collision.gameObject.CompareTag("Player"))
                //TODO: Particle/SFX
                Destroy(this.transform.parent.gameObject);
        }
    }
}
