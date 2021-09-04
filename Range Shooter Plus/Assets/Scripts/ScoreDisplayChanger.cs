using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplayChanger : MonoBehaviour
{

    public GameObject displaynewScoreTMP;
    public GameObject displayHiScoreTMP;
    public bool changed = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!changed)
            {
                changed = true;
                displaynewScoreTMP.GetComponent<TMPro.TextMeshPro>().text = "270";
                displayHiScoreTMP.GetComponent<TMPro.TextMeshPro>().text = "270";
            }
            else
            {
                {
                    changed = false;
                    displaynewScoreTMP.GetComponent<TMPro.TextMeshPro>().text = "180";
                    displayHiScoreTMP.GetComponent<TMPro.TextMeshPro>().text = "210";
                }
            }
        }
    }
}
