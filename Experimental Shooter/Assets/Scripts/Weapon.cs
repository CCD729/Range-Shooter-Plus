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
public class Weapon
{
    //Weapon properties
    public string name;
    public float damage;

    public bool melee = false;
    //melee properties
    public float meleeRange = 1f;
    public float attackDelay = 0.5f;
    public float attackBackSwing = 0.5f;

    //firearm properties
    public float firingRate = 10f;
    public int magSize = 30;
    public float reloadTime = 1.5f;
    //Weapon fireSelector
    public FireSelect defaultFireMode = FireSelect.auto;
    public FireSelect[] FireModes;

    //Melee weapon constructor
    public Weapon(bool isMelee, string newName, float newDamage, float newMeleeRange, float newAttackDelay, float newAttackBackSwing)
    {
        melee = isMelee; //melee = true;
        name = newName;
        damage = newDamage;
        meleeRange = newMeleeRange;
        attackDelay = newAttackDelay;
        attackBackSwing = newAttackBackSwing;
    }
    //Firearm weapon constructor
    public Weapon(bool isMelee, string newName, float newDamage, float newFiringRate, int newMagSize, float newReloadTime, FireSelect newDefaultFireMode, FireSelect[] newFireModes)
    {
        melee = isMelee; //melee = false;
        name = newName;
        damage = newDamage;
        firingRate = newFiringRate;
        magSize = newMagSize;
        reloadTime = newReloadTime;
        defaultFireMode = newDefaultFireMode;
        FireModes = newFireModes;
    }
}
