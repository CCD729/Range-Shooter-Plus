using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageDisplay : MonoBehaviour
{
    public Text damageDisplayText;
    public GameObject hitTarget;
    public Camera playerCam;
    Vector3 initPosition, modifiedPosition, diffPosition;
    public float hitYPosition = 0f;
    public float moveTime = 1.5f;
    public float fadeTimeInt = 0.5f;
    public float fadeTime = 0.5f;
    public float timeCounter = 0f;
    public float lerpPercentage = 0f;

    void Start()
    {
        damageDisplayText = gameObject.GetComponent<Text>();
        playerCam = GameObject.Find("Player1stCam").GetComponent<Camera>();
        hitYPosition = transform.position.y;
        initPosition = playerCam.WorldToScreenPoint(transform.position);
        transform.position = initPosition;
        //Considering making modified position dynamic...
        modifiedPosition = (new Vector3(Random.Range(initPosition.x - 100f, initPosition.x + 100f), Random.Range(initPosition.y + 200f, initPosition.y + 300f), 0f));
        diffPosition = modifiedPosition - initPosition;
        timeCounter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        initPosition = playerCam.WorldToScreenPoint(new Vector3(hitTarget.transform.position.x, hitYPosition, hitTarget.transform.position.z));
        modifiedPosition = initPosition + diffPosition;
        timeCounter += Time.deltaTime;
        if (timeCounter <= moveTime)
        {
            transform.position = Vector3.Lerp(initPosition, modifiedPosition, timeCounter / moveTime);
        }
        else if (timeCounter >= (moveTime + fadeTimeInt) && (timeCounter <= moveTime + fadeTimeInt + fadeTime) )
        {
            transform.position = modifiedPosition;
            lerpPercentage = 1 - (timeCounter - moveTime - fadeTimeInt) / fadeTime;
            damageDisplayText.color = new Color(damageDisplayText.color.r, damageDisplayText.color.g, damageDisplayText.color.b, lerpPercentage);
            Debug.Log(damageDisplayText.color);
        }
        else
        {
            transform.position = modifiedPosition;
        }
        if(timeCounter > moveTime + fadeTimeInt + fadeTime)
        {
            Destroy(gameObject);
        }

    }
}
