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

    //references needed by Reaction Trial
    [Header("Reaction Trial References")]
    public float reactionTrialminPopTime = 3f;
    public float reactionTrialMaxPopTime = 10f;
    public GameObject reactionTarget;
    public bool reactionTrialTargetUp;
    public float reactionTrialTimeCounter = 0f;
    public float reactionTrialPenalty = 0f;
    public Vector3 reactionTargetContainerInitialPosition;
    //adjustable popup range (optional)

    //references needed by Timed Trial
    public int timedTrialScore = 0;
    public float timedTrialTimeCounter = 0f;

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

        reactionTargetContainerInitialPosition = reactionTarget.transform.parent.position;

        //For first time loading hiScores with/without PlayerPrefs
        for (int i = 0; i < 4; i++)
        {
            if (PlayerPrefs.GetFloat("Trial" + i.ToString() + "HiScore", 0f) == 0f)
                trialHiScoreTMP[i].GetComponent<TMPro.TextMeshPro>().text = "N/A";
            else 
            {
                if (i == 2)
                {
                    trialHiScoreTMP[i].GetComponent<TMPro.TextMeshPro>().text = PlayerPrefs.GetFloat("Trial" + i.ToString() + "HiScore", 0f).ToString("F0");
                }
                else
                {
                    trialHiScoreTMP[i].GetComponent<TMPro.TextMeshPro>().text = PlayerPrefs.GetFloat("Trial" + i.ToString() + "HiScore", 0f).ToString("F2") + " " + trialUnitMark[i];
                }
            }
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
                if(grenadeObj.transform.position.x < 10.5f)
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
        if (ShootingScript.currentTrial == 1)
        {
            //Count time begin when Animation started
            if (!reactionTrialTargetUp)
                text_TrialDataRight.GetComponent<Text>().text = reactionTrialPenalty.ToString("F2") + " s";
            else
            {
                reactionTrialTimeCounter += Time.deltaTime;
                text_TrialDataRight.GetComponent<Text>().text = (reactionTrialTimeCounter + reactionTrialPenalty).ToString("F2") + " s";
            }
            //TBD conditions
            //Debug.Log("Reaction trial interrupted");
            //StopTrial();
        }
        if (ShootingScript.currentTrial == 2)
        {
            if (timedTrialTimeCounter <= 0f)
            {
                timedTrialTimeCounter = 0f;
                text_TrialDataLeft.GetComponent<Text>().text = "0.00 s";
                TimedTrialDataRecord();
                StopTrial();
            }
            else
            {
                timedTrialTimeCounter -= Time.deltaTime;
                text_TrialDataLeft.GetComponent<Text>().text = (timedTrialTimeCounter).ToString("F2") + " s";
                text_TrialDataRight.GetComponent<Text>().text = (timedTrialScore).ToString();
            }
            //TBD conditions
            //Debug.Log("Timed Trial interrupted");
            //StopTrial();
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
        trialNewScore[0] = Mathf.Clamp((lastXValue - 10.5f), 0f, Mathf.Infinity) / ShootingScript.distanceUnitRatio;
        Debug.Log("Grenade trial recording score...");
        UpdateScore(0);
    }

    void StartReactionTrial()
    {
        ShootingScript.currentTrial = 1;
        reactionTrialPenalty = 0f;
        reactionTrialTimeCounter = 0f;
        reactionTrialTargetUp = false;
        reactionTarget.GetComponent<animController>().TargetPopDownAnimation();
        text_TrialDataRight.GetComponent<Text>().text = "0.00 s";
        text_TrialDataRight.SetActive(true);
        StartCoroutine(PopReactionTarget(Random.Range(reactionTrialminPopTime, reactionTrialMaxPopTime)));
    }
    public void ReactionTrialDataRecord()
    {
        trialNewScore[1] = reactionTrialTimeCounter + reactionTrialPenalty;
        reactionTarget.GetComponent<animController>().TargetPopDownAnimation();
        StartCoroutine(ResetReactionTarget());
        Debug.Log("Reaction trial recording score...");
        UpdateScore(1);
    }
    void StartTimedTrial()
    {
        ShootingScript.currentTrial = 2;
        timedTrialScore = 0;
        timedTrialTimeCounter = 30f;
        //TODO: StartSpawnTimedTargets();
        text_TrialDataRight.GetComponent<Text>().text = "Score: 0";
        text_TrialDataRight.SetActive(true);
        text_TrialDataLeft.GetComponent<Text>().text = "30.00 s";
        text_TrialDataLeft.SetActive(true);

    }
    public void TimedTrialDataRecord()
    {
        trialNewScore[2] = timedTrialScore;
        StartCoroutine(ResetTimedTargets());
        Debug.Log("Timed trial recording score...");
        UpdateScore(2);
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
                    trialHiScoreTMP[0].GetComponent<TMPro.TextMeshPro>().text = trialNewScore[0].ToString("F2") + " " + trialUnitMark[0];
                }
                break;
            case 1:
                if ((PlayerPrefs.GetFloat("Trial1HiScore", 0f) != 0f && PlayerPrefs.GetFloat("Trial1HiScore", 0f) > trialNewScore[1])
                    || PlayerPrefs.GetFloat("Trial1HiScore", 0f) == 0f)
                {
                    PlayerPrefs.SetFloat("Trial1HiScore", trialNewScore[1]);
                    trialHiScoreTMP[1].GetComponent<TMPro.TextMeshPro>().text = trialNewScore[1].ToString("F2") + " " + trialUnitMark[1];
                }
                break;
            case 2:
                if (PlayerPrefs.GetFloat("Trial2HiScore", 0f) < trialNewScore[0])
                {
                    PlayerPrefs.SetFloat("Trial2HiScore", trialNewScore[2]);
                    trialHiScoreTMP[2].GetComponent<TMPro.TextMeshPro>().text = trialNewScore[2].ToString("F0");
                }
                break;
            case 3:
                if (PlayerPrefs.GetFloat("Trial3HiScore", 0f) > trialNewScore[3])
                {
                    PlayerPrefs.SetFloat("Trial3HiScore", trialNewScore[3]);
                    trialHiScoreTMP[3].GetComponent<TMPro.TextMeshPro>().text = trialNewScore[3].ToString("F2") + " " + trialUnitMark[3];
                }
                break;
            default:
                Debug.Log("Error: Invalid typeIdentifier found for Trial HiScore in UpdateScore()");
                break;
        }
    }
    IEnumerator PopReactionTarget(float popTime)
    {
        yield return new WaitForSecondsRealtime(popTime);
        reactionTarget.transform.parent.position = new Vector3(Random.Range(20f,50f), reactionTarget.transform.parent.position.y, Random.Range(4f, -18f));
        reactionTarget.GetComponent<animController>().TargetPopUpAnimation();
        reactionTarget.GetComponent<TargetBehavior>().damageDisplay = true;
        reactionTrialTargetUp = true;
    }
    IEnumerator ResetReactionTarget()
    {
        yield return new WaitForSecondsRealtime(1f);
        reactionTarget.transform.parent.position = reactionTargetContainerInitialPosition;
        reactionTarget.GetComponent<animController>().TargetPopUpAnimation();
    }
    IEnumerator ResetTimedTargets()
    {
        //TODO: Reset Displaying Targets to default positions with stepped animations
        yield return new WaitForSecondsRealtime(1f);

    }
}
