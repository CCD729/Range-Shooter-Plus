using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageDisplay : MonoBehaviour
{
    public Text damageDisplayText;
    public Camera playerCam;
    Vector3 initPosition, modifiedPosition;
    public float moveTime = 1.5f;
    public float fadeTimeInt = 0.5f;
    public float fadeTime = 0.5f;
    public float timeCounter = 0f;
    public float lerpPercentage = 0f;

    void Start()
    {
        damageDisplayText = gameObject.GetComponent<Text>();
        playerCam = GameObject.Find("Player1stCam").GetComponent<Camera>();
        initPosition = playerCam.WorldToScreenPoint(transform.position);
        transform.position = initPosition;
        //Considering making modified position dynamic...
        modifiedPosition = (new Vector3(Random.Range(initPosition.x - 100f, initPosition.x + 100f), Random.Range(initPosition.y + 200f, initPosition.y + 300f), 0f));
        timeCounter = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeCounter += Time.fixedDeltaTime;
        if (timeCounter <= moveTime)
        {
            transform.position = Vector3.Lerp(initPosition, modifiedPosition, timeCounter / moveTime);
        }
        else if (timeCounter >= moveTime + fadeTimeInt && timeCounter <= moveTime + fadeTimeInt +fadeTime)
        {
            lerpPercentage = (timeCounter - moveTime + fadeTimeInt) / fadeTime;
            damageDisplayText.color = new Color(damageDisplayText.color.r, damageDisplayText.color.g, damageDisplayText.color.b, lerpPercentage);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
