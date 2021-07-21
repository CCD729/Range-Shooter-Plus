using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairReticleDynamics : MonoBehaviour
{

    public GameObject player;
    public ShootingScript shootingScript;
    private RectTransform reticles;
    public float minSize;
    public float minSizeMoving;
    public float maxSize;
    public float maxSizeMoving;
    public float lerpSpeed;
    public float shootingCounter = 0f;
    public bool shooting = false;
    private float currentSize;

    void Start()
    {
        reticles = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (player.GetComponent<CharacterController>().velocity.magnitude > 0)
        {
            if (shooting)
            {
                Debug.Log("Shooting Moving");
                currentSize = Mathf.Lerp(currentSize, maxSizeMoving, Time.deltaTime * lerpSpeed);
                shootingScript.crosshairMultiplier = Mathf.Lerp(shootingScript.crosshairMultiplier, 3f, Time.deltaTime * lerpSpeed);
            }
            else
            {
                Debug.Log("Moving");
                currentSize = Mathf.Lerp(currentSize, minSizeMoving, Time.deltaTime * lerpSpeed);
                shootingScript.crosshairMultiplier = Mathf.Lerp(shootingScript.crosshairMultiplier, 1.5f, Time.deltaTime * lerpSpeed);
            }
        }
        else
        {
            if (shooting)
            {
                Debug.Log("Shooting");
                currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * lerpSpeed);
                shootingScript.crosshairMultiplier = Mathf.Lerp(shootingScript.crosshairMultiplier, 2f, Time.deltaTime * lerpSpeed);
            }
            else
            {
                Debug.Log("Nothing");
                currentSize = Mathf.Lerp(currentSize, minSize, Time.deltaTime * lerpSpeed);
                shootingScript.crosshairMultiplier = Mathf.Lerp(shootingScript.crosshairMultiplier, 1f, Time.deltaTime * lerpSpeed);
            }
        }
        if(shooting && shootingCounter <= 0f)
        {
            shooting = false;
            shootingCounter = 0f;
        }
        else if (shooting)
        {
            shootingCounter -= Time.deltaTime;
        }
        reticles.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
