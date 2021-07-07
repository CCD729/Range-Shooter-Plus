using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialScript : MonoBehaviour
{
    public ShootingScript ShootingScript;
    public GameObject text_TrialName;
    public GameObject text_TrialHint;
    public GameObject text_TrialDataRight;
    public GameObject text_TrialDataLeft;
    public GameObject text_TrialResult;
    public float[] trialNewScore = new float[4];
    //public float[] trialHiScore = new float[4];
    public string[] trialUnitMark = new string[4];
    public GameObject[] trialNewScoreTMP = new GameObject[4];
    public GameObject[] trialHiScoreTMP = new GameObject[4];
    /*public GameObject text_ReactionTrialTime;
    public GameObject text_TimedTrialTime;
    public GameObject text_TimedTrialScore;
    public GameObject text_FreemoveTrialTime;
    public GameObject text_FreemoveTrialProgress;*/

    public bool trailActive = false;
    public string[] trialNames;
    public string[] trialHints;

    //references needed by Grenade Trial
    [Header("Grenade Trial References")]
    public GameObject grenadeObj;
    public Vector3 grenadeInitialPosition;
    public bool grenadeBeforeGone = true;

    void Start()
    {
        //trialNewScore = new float[4];
        grenadeBeforeGone = true;
        text_TrialName.SetActive(false);
        text_TrialHint.SetActive(false);
        text_TrialDataRight.SetActive(false);
        text_TrialDataLeft.SetActive(false);
        /*text_ReactionTrialTime.SetActive(false);
        text_TimedTrialTime.SetActive(false);
        text_TimedTrialScore.SetActive(false);
        text_FreemoveTrialTime.SetActive(false);
        text_FreemoveTrialProgress.SetActive(false);*/

        //For first time loading hiScores without PlayerPrefs
        for(int i = 0; i < 4; i++)
        {
            if (PlayerPrefs.GetFloat("Trial" + i.ToString() + "HiScore", 0f) == 0f)
                trialHiScoreTMP[i].GetComponent<TMPro.TextMeshPro>().text = "N/A";
        }
    }

    void Update()
    {
        if(ShootingScript.currentTrial == 0 && grenadeObj && grenadeBeforeGone)
        {
            //Track grenade Position (UI tracking distance)
            //Within good area
            if (grenadeObj.transform.position.z < 34f && grenadeObj.transform.position.z > 6 && grenadeObj.transform.position.x > -10.5f)
            {
                if(grenadeObj.transform.position.x < 10f)
                {
                    text_TrialDataRight.GetComponent<Text>().text = "0.00 m";
                }
                else
                {
                    text_TrialDataRight.GetComponent<Text>().text = ((grenadeObj.transform.position.x-10f)/ShootingScript.distanceUnitRatio).ToString("F2") + " m";
                }
            }
            else
            {
                Debug.Log("Grenade trial interrupted because out of range");
                StopTrial();
            }

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
        text_TrialDataRight.GetComponent<Text>().text = "0.00 m";
        text_TrialDataRight.SetActive(true);
        //Modify grenade function to sync data to distance indicator
        //make ratio converter unify in Shootingscript
        //modify grenade function to return data when hit within range (with invisible floor if collider works smoother)
        //grenade live check if hit other collider or fall out of range to stop
        //Update last score and hiscore

    }
    public void GrenadeTrialDataRecord(float lastXValue)
    {
        trialNewScore[0] = (lastXValue - 10f) / ShootingScript.distanceUnitRatio;
        Debug.Log("Grenade trial recording score...");
        UpdateScore(0);
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
    public void StopTrial()
    {
        ShootingScript.currentTrial = -1;
        text_TrialDataLeft.SetActive(false);
        text_TrialDataLeft.GetComponent<Text>().text = "TrialDataLeftTextExample";
        text_TrialDataRight.SetActive(false);
        text_TrialDataRight.GetComponent<Text>().text = "TrialDataRightTextExample";
        text_TrialName.SetActive(false);
        text_TrialHint.SetActive(false);
        trailActive = false;
    }
    void UpdateScore(int typeIdentifier)
    {

        //New Score Display
        if (trialNewScore[typeIdentifier] == 0f)
        {
            trialNewScoreTMP[typeIdentifier].GetComponent<TMPro.TextMeshPro>().text = "N/A";
        }
        else
        {
            if (typeIdentifier == 2)
            {
                trialNewScoreTMP[typeIdentifier].GetComponent<TMPro.TextMeshPro>().text = trialNewScore[typeIdentifier].ToString("F0");
            }
            else
            {
                trialNewScoreTMP[typeIdentifier].GetComponent<TMPro.TextMeshPro>().text = trialNewScore[typeIdentifier].ToString("F2") + " " + trialUnitMark[typeIdentifier];
            }
        }
        

        //Hi-Score with PlayerPrefs (TODO: Add cases)
        switch (typeIdentifier)
        {
            case 0:
                if (PlayerPrefs.GetFloat("Trial0HiScore", 0f) < trialNewScore[0])
                {
                    PlayerPrefs.SetFloat("Trial0HiScore", trialNewScore[0]);
                    trialHiScoreTMP[typeIdentifier].GetComponent<TMPro.TextMeshPro>().text = trialNewScore[typeIdentifier].ToString("F2") + " " + trialUnitMark[typeIdentifier];
                }
                break;
            default:
                break;
        }
    }
}
