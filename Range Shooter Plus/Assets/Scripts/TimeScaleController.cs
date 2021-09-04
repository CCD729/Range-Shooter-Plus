using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    public bool slow = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (!slow)
            {
                slow = true;
                Time.timeScale = 0.3f;
            }
            else
            {
                slow = false;
                Time.timeScale = 1f;
            }
        }

    }
}
