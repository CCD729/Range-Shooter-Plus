using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject impactGrenadeObject;
    // Start is called before the first frame update
    void Start()
    {
        impactGrenadeObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            impactGrenadeObject.GetComponent<Rigidbody>().isKinematic = false;
            //impactGrenadeObject.transform.LookAt(transform.forward * 1000);
            // Modify the throwing angle so the grenade is a bit higher than horizontal
            Vector3 currentRotation = impactGrenadeObject.transform.eulerAngles;
            Vector3 modifiedRotation = currentRotation + new Vector3(-2f, 0f, 0f);
            impactGrenadeObject.transform.localRotation = Quaternion.Euler(modifiedRotation);
            impactGrenadeObject.GetComponent<Rigidbody>().AddForce(impactGrenadeObject.transform.forward * 400f, ForceMode.Impulse);
        }
    }
}
