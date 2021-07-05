using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInfo : MonoBehaviour
{
    public bool trialButton = false;
    public bool resourceButton = false;
    public bool oneTimeButton = false;
    public float coolDown = 1f; 
    public int typeIdentifier;
    public string[] GUIDisplayText;
}
