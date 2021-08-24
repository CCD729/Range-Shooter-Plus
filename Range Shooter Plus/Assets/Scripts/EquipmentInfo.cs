using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfo : MonoBehaviour
{
    //Equipment properties (currently just identifier)
    new public string name;
    public bool primary = false;
    public ShootingScript.Equipment equipmentType;
    public float equipmentCoolDown = 0f;
    public float equipmentTime = 0f;
}
