using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotating : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private float rotateX = 1f;
    [SerializeField] private float rotateY = 1f;
    [SerializeField] private float rotateZ = 1f;
    [SerializeField] private float floatAmp = 0.4f;
    [SerializeField] private float floatFrequency = 1f;
    //[SerializeField] private float floatYPosition = 0f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float currentScale = 1f;
    [SerializeField] private float currentEmissionMultiplier = 1f;
    [SerializeField] private float currentLightIntensityMultiplier = 1f;

    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Color initColor;
    [SerializeField] private float initLightIntensity;
    public bool stopped = false;
    public float rotateTarget = 0.0f;
    public float floatTarget = 0.0f;
    public float scaleTarget = 0.0f;
    public float emissionTarget = 0.0f;
    public float lightIntensityTarget = 0.0f;
    public float smoothTime = 0.8f;
    [SerializeField] private float dampVelocityRotation = 0.0f;
    [SerializeField] private float dampVelocityFloat = 0.0f;
    [SerializeField] private float dampVelocityScale = 0.0f;
    [SerializeField] private float dampVelocityEmission = 0.0f;
    [SerializeField] private float dampVelocityLightIntensity = 0.0f;

    void Start()
    {
        initialPosition = transform.position;
        initColor = gameObject.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        initLightIntensity = gameObject.GetComponentInChildren<Light>().intensity;
    }

    void Update()
    {
        if (stopped)
        {
            rotateTarget = 0f;
            floatTarget = 0f;
            scaleTarget = 0.5f;
            emissionTarget = 0f;
            lightIntensityTarget = 0f;
        }
        else
        {
            rotateTarget = 1f;
            floatTarget = 1f;
            scaleTarget = 1f;
            emissionTarget = 1f;
            lightIntensityTarget = 1f;
        }
        rotateSpeed = Mathf.SmoothDamp(rotateSpeed, rotateTarget, ref dampVelocityRotation, smoothTime);
        floatSpeed = Mathf.SmoothDamp(floatSpeed, floatTarget, ref dampVelocityFloat, smoothTime);
        currentScale = Mathf.SmoothDamp(currentScale, scaleTarget, ref dampVelocityScale, smoothTime);
        currentEmissionMultiplier = Mathf.SmoothDamp(currentEmissionMultiplier, emissionTarget, ref dampVelocityEmission, smoothTime);
        currentLightIntensityMultiplier = Mathf.SmoothDamp(currentLightIntensityMultiplier, lightIntensityTarget, ref dampVelocityLightIntensity, smoothTime);

        //Rotate
        transform.Rotate(new Vector3(rotateX, rotateY, rotateZ) * rotateSpeed * Time.timeScale);

        // Float 
        Vector3 temp = initialPosition;
        temp.y += Mathf.Sin(Time.fixedTime * Mathf.PI * floatFrequency) * floatAmp * floatSpeed;
        //floatYPosition = temp;
        transform.position = temp;

        //Scale
        transform.localScale = new Vector3 (currentScale, currentScale, currentScale);

        //Emission

        gameObject.GetComponent<MeshRenderer>().material.SetVector("_EmissionColor", initColor * currentEmissionMultiplier);

        //Light intensity
        gameObject.GetComponentInChildren<Light>().intensity = initLightIntensity * currentLightIntensityMultiplier;
    }
}
