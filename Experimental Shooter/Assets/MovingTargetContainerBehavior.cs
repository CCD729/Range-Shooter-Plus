using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetContainerBehavior : MonoBehaviour
{
    public enum MovementType
    {
        None,
        StraightLine,
        Flying
    }
    public GameObject TargetObj;
    public GameObject[] TargetGroup;
    public TrialScript trialScript;

    //public Vector3 initPosition;
    //public Vector3 initRotationEuler;

    public bool startUpDown = false;
    public bool active;
    public bool hasAnimation;
    //If the target has some rotation
    public bool selfRotating;
    public float rotateX = 0f;
    public float rotateY = 1f;
    public float rotateZ = 0f;
    public float rotationSpeed = 1f; //Maybe needs rework

    public MovementType movementType;

    public float timeDuration;
    public float timeCounter;

    //StraightLine Moving Properties
    [Header("StraightLine Moving Properties")]
    public Vector3 startPos;
    public Vector3 finishPos;

    //Flying Properties
    [Header("Flying Properties")]
    //public Vector3 initRotation; // No long needed
    public int flyingDirection = 0; //Maybe could be applied to all targets

    //Color changing Properties
    [Header("Color Changing Properties")]
    [SerializeField] private float currentEmissionMultiplier = 1f;
    [SerializeField] private float currentLightIntensityMultiplier = 1f;
    [SerializeField] private Color initColor;
    [SerializeField] private float initLightIntensity;

    void Start()
    {
        //initPosition = transform.localPosition;
        //initRotation = transform.localRotation;
        if (startUpDown)
        {
            if (hasAnimation)
            {
                TargetObj.GetComponent<animController>().TargetPopDownAnimation();
            }
            TargetObj.GetComponent<TargetBehavior>().targetDown = true;
            TargetObj.GetComponent<TargetBehavior>().damageTaking = false;
           TargetObj.GetComponent<TargetBehavior>().damageDisplay = false;
        }
        if (TargetObj.GetComponent<TargetBehavior>().redBoxhitMaterialChange)
        {
            initColor = TargetObj.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
            initLightIntensity = TargetObj.GetComponentInChildren<Light>().intensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            //Time Counting when objective Finished and unhit
            if (timeCounter >= timeDuration && TargetObj.GetComponent<TargetBehavior>().TimedTrialUse)
            {
                TargetObj.GetComponent<TargetBehavior>().targetDown = true;
                TargetObj.GetComponent<TargetBehavior>().damageTaking = false;
                TargetObj.GetComponent<TargetBehavior>().damageDisplay = false;
                FinishDown();
            }
            else
            {
                timeCounter += Time.deltaTime;
            }

            if (movementType == MovementType.None)
            {
                //Nothing here... Don't move
            }
            else if (movementType == MovementType.StraightLine)
            {
                transform.position = Vector3.Lerp(startPos, finishPos, timeCounter / timeDuration);
            }
            else if (movementType == MovementType.Flying)
            {
                if(flyingDirection == 0)
                    transform.position = Vector3.Lerp(startPos, finishPos, timeCounter / timeDuration);
                else
                    transform.position = Vector3.Lerp(finishPos, startPos, timeCounter / timeDuration);
            }
            if (selfRotating)
            {
                transform.Rotate(new Vector3(rotateX, rotateY, rotateZ) * rotationSpeed * Time.timeScale);
            }
        }
        if (TargetObj.GetComponent<TargetBehavior>().redBoxhitMaterialChange)
        {
            TargetObj.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", initColor * currentEmissionMultiplier);
            //Light intensity
            TargetObj.GetComponentInChildren<Light>().intensity = initLightIntensity * currentLightIntensityMultiplier;
        }
    }
     
    public void FinishDown()
    {
        if (hasAnimation)
        {
            TargetObj.GetComponent<animController>().TargetPopDownAnimation();
        }
        if (TargetObj.GetComponent<TargetBehavior>().TimedTrialUse)
        {
            //Trigger another to spawn soon
            int choice = Random.Range(0, TargetGroup.Length);
            while (TargetGroup[choice].GetComponent<MovingTargetContainerBehavior>().active)
            {
                choice = Random.Range(0, TargetGroup.Length);
            }
            if (trialScript.timedCurrentTarget < trialScript.timedMaxTarget)
            {
                TargetGroup[choice].GetComponent<MovingTargetContainerBehavior>().StartUp();
                trialScript.timedCurrentTarget++;
            }
            trialScript.timedCurrentTarget--;
            if (movementType == MovementType.Flying)
            {
                StartCoroutine(ResetFlyingTargetPosition(5f));
            }
        }
        if (TargetObj.GetComponent<TargetBehavior>().redBoxhitMaterialChange)
        {
            currentEmissionMultiplier = 0f;
            currentLightIntensityMultiplier = 0f;
        }
        active = false;
    }
    public void StartUp()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        if (TargetObj.GetComponent<TargetBehavior>().kinematicWhenActive)
            TargetObj.GetComponent<Rigidbody>().isKinematic = true;
        if (TargetObj.GetComponent<Rigidbody>())
        {
            TargetObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            TargetObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        TargetObj.transform.localPosition = TargetObj.GetComponent<TargetBehavior>().initPosition;
        TargetObj.transform.localRotation = Quaternion.Euler(TargetObj.GetComponent<TargetBehavior>().initRotationEuler);
        //transform.localPosition = initPosition;
        //transform.localRotation = Quaternion.Euler(initRotationEuler);

        if (movementType == MovementType.StraightLine)
        {
            transform.position = startPos;
        }
        if (movementType == MovementType.Flying) //TODO: Flipping changed to flying
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            TargetObj.transform.localPosition = TargetObj.GetComponent<TargetBehavior>().initPosition;
            TargetObj.transform.localRotation = Quaternion.Euler(TargetObj.GetComponent<TargetBehavior>().initRotationEuler);
            flyingDirection = Random.Range(0, 2);
            if (flyingDirection == 0)
            {
                transform.position = startPos;
            }
            else
            {
                transform.position = finishPos;
            }
            if (TargetObj.GetComponent<TargetBehavior>().changeMaterial)
                TargetObj.GetComponent<TargetBehavior>().ResetMaterial();
        }
        if (hasAnimation)
        {
            TargetObj.GetComponent<animController>().TargetPopUpAnimation();
        }
        TargetObj.GetComponent<TargetBehavior>().targetDown = false;
        TargetObj.GetComponent<TargetBehavior>().damageTaking = TargetObj.GetComponent<TargetBehavior>().damageTakingDefault;
        TargetObj.GetComponent<TargetBehavior>().damageDisplay = TargetObj.GetComponent<TargetBehavior>().damageDisplayDefault;
        TargetObj.GetComponent<TargetBehavior>().hitPoints = TargetObj.GetComponent<TargetBehavior>().maxHitPoints;

        if (TargetObj.GetComponent<TargetBehavior>().redBoxhitMaterialChange)
        {
            currentEmissionMultiplier = 1f;
            currentLightIntensityMultiplier = 1f;
        }
        //Reset time counter
        timeCounter = 0f;
        active = true;
    }

    IEnumerator ResetFlyingTargetPosition(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        transform.localPosition = startPos;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        TargetObj.transform.localPosition = TargetObj.GetComponent<TargetBehavior>().initPosition;
        TargetObj.transform.localRotation = Quaternion.Euler(TargetObj.GetComponent<TargetBehavior>().initRotationEuler);
        flyingDirection = 0;
        TargetObj.GetComponent<Rigidbody>().isKinematic = true;
    }
}
