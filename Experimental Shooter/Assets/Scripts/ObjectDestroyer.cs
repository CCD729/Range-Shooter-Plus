using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField]
    private float destroyTime = 1f;

    public void Destroy()
    {
        StartCoroutine(Destroyer(destroyTime));
    }
    IEnumerator Destroyer(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
