using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    public GameObject[] pickupablesWeapon;
    public GameObject[] pickupablesWeaponPOV;

    public ShootingScript shootingScript;

    private bool matchfound = false;
    private bool matchfoundPOV = false;

    public void Pickup(WeaponInfo weaponInfo, PickupTrigger pickupTrigger)
    {
        //TODO NEXT: port customized weaponInfo in the future
        int weaponid = weaponInfo.weaponID;
        GameObject targetWeapon;
        GameObject targetWeaponPOV;
        foreach (GameObject weapon in pickupablesWeapon){
            if(weapon.GetComponent<WeaponInfo>().weaponID == weaponid)
            {
                matchfound = true;
                targetWeapon = weapon;
                foreach (GameObject weaponPOV in pickupablesWeaponPOV)
                {
                    if (weaponPOV.GetComponent<WeaponInfo>().weaponID == weaponid)
                    {
                        matchfoundPOV = true;
                        targetWeaponPOV = weaponPOV;
                        Destroy(pickupTrigger.gameObject);
                        GameObject instanceWeapon = Instantiate(targetWeapon);
                        GameObject instanceWeaponPOV = Instantiate(targetWeaponPOV);
                        if (shootingScript.weaponEquipped)
                        {
                            if (!shootingScript.weaponFull)
                            {
                                instanceWeapon.transform.SetParent(shootingScript.weaponContainer.transform);
                                instanceWeapon.transform.localPosition = Vector3.zero;
                                instanceWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
                                instanceWeapon.transform.localScale = Vector3.one;
                                instanceWeaponPOV.transform.SetParent(shootingScript.weaponContainerPOV.transform);
                                instanceWeaponPOV.transform.localPosition = Vector3.zero;
                                instanceWeaponPOV.transform.localRotation = Quaternion.Euler(Vector3.zero);
                                instanceWeaponPOV.transform.localScale = Vector3.one;
                                shootingScript.weaponFull = true;
                                shootingScript.weaponSlot = 1;
                                shootingScript.secondaryWeapon = instanceWeapon.gameObject;
                                shootingScript.secondaryWeaponPOV = instanceWeaponPOV.gameObject;
                                shootingScript.currentWeapon = shootingScript.secondaryWeapon;
                                shootingScript.currentWeaponPOV = shootingScript.secondaryWeaponPOV;
                                shootingScript.firePoint = instanceWeapon.transform.Find("FirePoint");
                                shootingScript.UpdateWeaponInfo();
                            }
                            else
                            {
                                //TODO: Switch weapon and drop equipped one out
                            }
                        }
                        else
                        {
                            shootingScript.weaponEquipped = true;
                            instanceWeapon.transform.SetParent(shootingScript.weaponContainer.transform);
                            instanceWeapon.transform.localPosition = Vector3.zero;
                            instanceWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
                            instanceWeapon.transform.localScale = Vector3.one;
                            instanceWeaponPOV.transform.SetParent(shootingScript.weaponContainerPOV.transform);
                            instanceWeaponPOV.transform.localPosition = Vector3.zero;
                            instanceWeaponPOV.transform.localRotation = Quaternion.Euler(Vector3.zero);
                            instanceWeaponPOV.transform.localScale = Vector3.one;
                            shootingScript.primaryWeapon = instanceWeapon.gameObject;
                            shootingScript.primaryWeaponPOV = instanceWeaponPOV.gameObject;
                            shootingScript.currentWeapon = shootingScript.primaryWeapon;
                            shootingScript.currentWeaponPOV = shootingScript.primaryWeaponPOV;
                            shootingScript.firePoint = instanceWeapon.transform.Find("FirePoint");
                            shootingScript.UpdateWeaponInfo();
                        }
                    }
                }
            }
        }
        if (!matchfound || !matchfoundPOV)
        {
            Debug.Log("Error: No match found with pickup object");
        }
    }
}
