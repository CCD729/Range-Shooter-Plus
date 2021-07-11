using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInfo : MonoBehaviour
{
    public bool trialButton = false;
    public bool ammoBox = false;
    public bool oneTimeButton = false;
    public bool teleportButton = false;
    public Vector3 TeleportPosition = Vector3.zero;
    public float coolDown = 1f; 
    //For Trials
    public int typeIdentifier;
    public string[] GUIDisplayText;
}
