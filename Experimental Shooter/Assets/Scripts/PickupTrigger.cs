using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    public Transform player;
    public float pickUpRange;
    public PickupHandler pickupHandler;
    private void Update()
    {
        //THIS SCRIPT IS DECREPATED FOR THE MOMENT

        /*//pickup when in range (TODO: weapon data UI, multiple data priority according to viewpoint, etc.)
        Vector3 distance = player.position - transform.position;
        if (distance.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E)) 
            pickupHandler.Pickup(gameObject.GetComponent<WeaponInfo>(), this.gameObject);*/
    }
}
