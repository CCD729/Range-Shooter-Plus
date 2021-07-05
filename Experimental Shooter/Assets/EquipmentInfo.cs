using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfo : MonoBehaviour
{
    public enum EquipmentType
    {
        ImpactGrenade
    }
    //Equipment properties (currently just identifier)
    new public string name;
    public EquipmentType equipmentType;
}
