using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialScript : MonoBehaviour
{
    public ShootingScript ShootingScript;
    public GameObject text_TrialName;
    public GameObject text_TrialHint;

    public bool trailActive = false;
    public string[] trialNames;
    public string[] trialHints;

    //references needed by Grenade Trial
    public GameObject grenadeObj;
    public Vector3 grenadeInitialPosition;

    void Start()
    {
        text_TrialName.SetActive(false);
        text_TrialHint.SetActive(false);
    }

    void Update()
    {
        if(ShootingScript.currentTrial == 0 && grenadeObj)
        {
            //Track grenade Position (UI tracking distance)
        }
    }

    void FixedUpdate()
    {

    }

    public void BeginTrial(int typeIdentifier)
    {
        //check if any trial active
        if (!trailActive)
        {
            trailActive = true;

            switch (typeIdentifier)
            {
                case 0:
                    StartGrenadeTrial();
                    break;
                case 1:
                    StartReactionTrial();
                    break;
                case 2:
                    StartTimedTrial();
                    break;
                case 3:
                    StartFreemoveTrial();
                    break;
                default:
                    Debug.Log("Error: Unexpected typeIdentifier found in BeginTrial()");
                    break;
            }
            //enable distance indicator HUD maybe with trial hint on left/right side
            text_TrialName.GetComponent<Text>().text = trialNames[typeIdentifier];
            text_TrialHint.GetComponent<Text>().text = trialHints[typeIdentifier];
            text_TrialName.SetActive(true);
            text_TrialHint.SetActive(true);
        }
    }
    void StartGrenadeTrial()
    {
        ShootingScript.currentTrial = 0;
        //Modify grenade function to sync data to distance indicator
        //
        //make ratio converter unify in Shootingscript
        //modify grenade function to return data when hit within range (with invisible floor if collider works smoother)
        //grenade live check if hit other collider or fall out of range to stop
        //Update last score and hiscore

    }
    void StartReactionTrial()
    {
        ShootingScript.currentTrial = 1;
    }
    void StartTimedTrial()
    {
        ShootingScript.currentTrial = 2;
    }
    void StartFreemoveTrial()
    {
        ShootingScript.currentTrial = 3;
    }
    void StopTrial()
    {
        ShootingScript.currentTrial = -1;
        text_TrialName.SetActive(false);
        text_TrialHint.SetActive(false);
    }
}
