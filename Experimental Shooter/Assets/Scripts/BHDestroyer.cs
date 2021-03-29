using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHDestroyer : MonoBehaviour
{
    [SerializeField]
    private float timeoutLifetime = 3f;

    private float destroyTime;

    void Start()
    {
        destroyTime = Time.time + timeoutLifetime;
    }

    void Update()
    {
        if (destroyTime <= Time.time)
            Destroy(gameObject);
    }
}
