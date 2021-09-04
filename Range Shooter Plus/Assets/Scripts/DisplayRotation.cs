using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRotation : MonoBehaviour
{
    public GameObject gunDisplayCenter;
    public float gunRotateSpeed = 10f;

    void Start()
    {
        Time.timeScale = 1.0f;
    }
    // Update is called once per frame
    void Update()
    {
        gunDisplayCenter.transform.localRotation = Quaternion.Euler(new Vector3(gunDisplayCenter.transform.rotation.eulerAngles.x, gunDisplayCenter.transform.rotation.eulerAngles.y + gunRotateSpeed * Time.deltaTime, gunDisplayCenter.transform.rotation.eulerAngles.z));
    }
}
