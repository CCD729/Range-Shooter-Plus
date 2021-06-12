using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] private float dropForce = 3f;
    [SerializeField] private float dropForceUpward = 3f;
    public GameObject[] pickupablesWeapon;
    public GameObject[] pickupablesWeaponPOV;

    public GameObject player;
    public ShootingScript shootingScript;
    public LevelSceneManager levelSceneManager;

    private bool matchfound = false;
    private bool matchfoundPOV = false;

    public void Pickup(WeaponInfo weaponInfo, GameObject pickupObj)
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
                        //Destroy(pickupObj);
                        pickupObj.GetComponent<Rigidbody>().isKinematic = true;
                        pickupObj.GetComponent<BoxCollider>().isTrigger = true;
                        pickupObj.transform.SetParent(shootingScript.weaponBackDisplayContainer.transform);
                        pickupObj.transform.localPosition = Vector3.zero;
                        pickupObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        pickupObj.transform.localScale = Vector3.one;
                        pickupObj.transform.SetLayerAllChildren(12);
                        GameObject instanceWeapon = Instantiate(targetWeapon);
                        GameObject instanceWeaponPOV = Instantiate(targetWeaponPOV);
                        instanceWeapon.transform.SetParent(shootingScript.weaponContainer.transform);
                        instanceWeapon.transform.localPosition = Vector3.zero;
                        instanceWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        instanceWeapon.transform.localScale = Vector3.one;
                        instanceWeaponPOV.transform.SetParent(shootingScript.weaponContainerPOV.transform);
                        instanceWeaponPOV.transform.localPosition = Vector3.zero;
                        instanceWeaponPOV.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        instanceWeaponPOV.transform.localScale = Vector3.one;
                        if (shootingScript.weaponEquipped)
                        {
                            if (!shootingScript.weaponFull)
                            {
                                shootingScript.weaponFull = true;
                                shootingScript.currentWeaponSlot = 1;
                                shootingScript.secondaryWeapon = instanceWeapon.gameObject;
                                shootingScript.secondaryWeaponPOV = instanceWeaponPOV.gameObject;
                                shootingScript.secondaryBullet = weaponInfo.bulletObj;
                                shootingScript.currentWeapon = shootingScript.secondaryWeapon;
                                shootingScript.currentWeaponPOV = shootingScript.secondaryWeaponPOV;
                                shootingScript.currentBullet = shootingScript.secondaryBullet;
                                shootingScript.secondaryWeaponBackDisplay = pickupObj;
                                shootingScript.secondaryWeaponBackDisplay.SetActive(false);
                                shootingScript.primaryWeaponBackDisplay.SetActive(true);
                                shootingScript.primaryWeapon.SetActive(false);
                                shootingScript.primaryWeaponPOV.SetActive(false);
                            }
                            else
                            {
                                //TODO: Switch weapon and drop equipped one out
                                //Delete old
                                Destroy(shootingScript.currentWeapon);
                                Destroy(shootingScript.currentWeaponPOV);

                                if (shootingScript.currentWeaponSlot == 0)
                                {
                                    shootingScript.primaryWeaponBackDisplay.transform.SetParent(null);
                                    shootingScript.primaryWeaponBackDisplay.transform.SetLayerAllChildren(13);
                                    //shootingScript.primaryWeaponBackDisplay = null;       // is this the correct way to do this when dropping for nothing?
                                    DropPhysics(shootingScript.primaryWeaponBackDisplay);
                                    //Add new
                                    shootingScript.primaryWeapon = instanceWeapon.gameObject;
                                    shootingScript.primaryWeaponPOV = instanceWeaponPOV.gameObject;
                                    shootingScript.primaryBullet = weaponInfo.bulletObj;
                                    shootingScript.currentWeapon = shootingScript.primaryWeapon;
                                    shootingScript.currentWeaponPOV = shootingScript.primaryWeaponPOV;
                                    shootingScript.currentBullet = shootingScript.primaryBullet;
                                    shootingScript.primaryWeaponBackDisplay = pickupObj;
                                }
                                else
                                {
                                    shootingScript.secondaryWeaponBackDisplay.transform.SetParent(null);
                                    shootingScript.secondaryWeaponBackDisplay.transform.SetLayerAllChildren(13);
                                    DropPhysics(shootingScript.secondaryWeaponBackDisplay);
                                    //Add new
                                    shootingScript.secondaryWeapon = instanceWeapon.gameObject;
                                    shootingScript.secondaryWeaponPOV = instanceWeaponPOV.gameObject;
                                    shootingScript.secondaryBullet = weaponInfo.bulletObj;
                                    shootingScript.currentWeapon = shootingScript.secondaryWeapon;
                                    shootingScript.currentWeaponPOV = shootingScript.secondaryWeaponPOV;
                                    shootingScript.currentBullet = shootingScript.secondaryBullet;
                                    shootingScript.secondaryWeaponBackDisplay = pickupObj;
                                }
                            }
                        }
                        else
                        {
                            shootingScript.weaponEquipped = true;
                            shootingScript.primaryWeapon = instanceWeapon.gameObject;
                            shootingScript.primaryWeaponPOV = instanceWeaponPOV.gameObject;
                            shootingScript.primaryBullet = weaponInfo.bulletObj;
                            shootingScript.currentWeapon = shootingScript.primaryWeapon;
                            shootingScript.currentWeaponPOV = shootingScript.primaryWeaponPOV;
                            shootingScript.currentBullet = shootingScript.primaryBullet;
                            shootingScript.primaryWeaponBackDisplay = pickupObj;
                            shootingScript.primaryWeaponBackDisplay.SetActive(false);
                        }
                        shootingScript.firePoint = instanceWeapon.transform.GetChild(0).Find("FirePoint");
                        shootingScript.UpdateWeaponInfo();
                        levelSceneManager.UpdateWeaponInfo();
                    }
                }
            }
        }
        if (!matchfound || !matchfoundPOV)
        {
            Debug.Log("Error: No match found with pickup object");
        }
    }

    /// <summary>
    /// Drops a item at hand (physics only)
    /// </summary>
    /// <param name="dropObj"></param>
    public void DropPhysics(GameObject dropObj)
    {
        dropObj.GetComponent<Rigidbody>().isKinematic = false;
        dropObj.GetComponent<BoxCollider>().isTrigger = false;
        dropObj.GetComponent<Rigidbody>().velocity = player.GetComponent<CharacterController>().velocity;
        dropObj.GetComponent<Rigidbody>().AddForce(shootingScript.playerCam.transform.forward * dropForce, ForceMode.Impulse);
        dropObj.GetComponent<Rigidbody>().AddForce(shootingScript.playerCam.transform.up * dropForceUpward, ForceMode.Impulse);
    }
       
}

public static class Tools
{
    //Inspired by "Change GameObject layer at run time won't apply to child" at Unity Answers 
    public static void SetLayerAllChildren(this Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        for (int i = 0, count = obj.childCount; i < count; i++)
        {
            obj.GetChild(i).SetLayerAllChildren(layer);
        }
    }
}