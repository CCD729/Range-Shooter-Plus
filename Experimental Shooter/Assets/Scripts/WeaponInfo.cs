using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    SubMachinegun,
    AssaultRifle,
    SniperRifle,
    TacticalRifle,
    Shotgun,
    Handgun,
    Melee
}
public enum FireSelect
{
    auto,
    burst,
    semi
}
public class WeaponInfo : MonoBehaviour
{
    //Weapon properties
    new public string name;
    public int weaponID;
    public float damage;
    public WeaponType weaponType;

    [Tooltip("Return true if the weapon is a melee weapon")]
    public bool melee = false;
    //melee properties
    public float meleeRange = 1f;
    public float attackDelay = 0.5f;
    public float attackBackSwing = 0.5f;

    //firearm properties
    public float firingRate = 10f;
    public float burstRate = 10f;
    public int magSize = 30;
    public int currentMagAmmo = 30;
    public int backupAmmo = 120;
    public int maxAmmo = 240;
    public float tacticalReloadTime = 1.5f;
    public float emptyReloadTime = 1.5f;
    public float ammoFillTime = 1.5f;
    public bool requireActionPull = true;
    public float maxHorizontalRecoil = 0.5f;
    public float minHorizontalRecoil = 0f;
    public float verticalRecoil = 0.5f;
    public FireSelect fireMode = FireSelect.auto;
    public FireSelect[] FireModes;
    public GameObject bulletObj;
}
