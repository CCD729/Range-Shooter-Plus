using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedboxHitter : MonoBehaviour
{
    public GameObject redboxDisplay;
    public GameObject machineGunPickup;
    public PickupHandler pickupHandler;
    public TrialScript trialScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            pickupHandler.PickupWeapon(machineGunPickup.GetComponent<WeaponInfo>(), machineGunPickup);
            /*            redboxDisplay.GetComponentInChildren<Rigidbody>().isKinematic = false;
                        redboxDisplay.GetComponent<TargetBehavior>().targetDown = true;
                        redboxDisplay.GetComponent<TargetBehavior>().damageTaking = false;
                        redboxDisplay.GetComponent<TargetBehavior>().damageDisplay = false;

                        redboxDisplay.GetComponentInChildren<Rigidbody>().AddForceAtPosition(hitDir * 2, hitPos, ForceMode.Impulse);
                        transform.parent.GetComponent<MovingTargetContainerBehavior>().FinishDown();*/
            trialScript.BeginTrial(2);
        }
    }
}
