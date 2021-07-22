using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
	public Slider sensSlider, fovSlider;
    private Vector2 mouseCurrent;
    [SerializeField]
    private float mouseSensitivity = 20f;
    [SerializeField]
    private float fieldOfView = 100f;
    private GameObject character;
    public GameObject gunPosParent;
    bool gamePlaying;

    //Legacy recoil references
    bool recoil = false;
    float maxHorizontalRecoil = 0.5f;
    float minHorizontalRecoil = 0f;
    float verticalRecoil = 0.5f;

    //Currently slider events are not working as expected so slider data are force updated
    //bool mouseSensChanged, fovChanged = false;



    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 20f);
        fieldOfView = PlayerPrefs.GetFloat("FOV", 90f);
        gameObject.GetComponent<Camera>().fieldOfView = fieldOfView;
        sensSlider.value = mouseSensitivity;
        fovSlider.value = fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseCurrent.x = 0;
        character = this.transform.parent.parent.gameObject;
        gamePlaying = true;
    }

    void Update()
    {
        if (gamePlaying)
        {
            var mouseGet = new Vector2(Input.GetAxis("Mouse X") * mouseSensitivity / 8, Input.GetAxis("Mouse Y") * mouseSensitivity / 8);
            mouseCurrent += mouseGet;
            if (mouseCurrent.y >= 90)
                mouseCurrent.y = 90;
            else if (mouseCurrent.y <= -90)
                mouseCurrent.y = -90;
          /*  if (recoil)
            {
                //Legacy recoil code
                //mouseCurrent.y += verticalRecoil;
                //mouseCurrent.x +=( (Random.Range(0, 2) * 2 - 1) * Random.Range(maxHorizontalRecoil, minHorizontalRecoil) );

            }*/
            //in-game model vertical axis rotationfor better view (WIP)
            //gunPosParent.transform.localRotation = Quaternion.AngleAxis(-mouseCurrent.y, Vector3.right);
            //camera vertical axis
            transform.localRotation = Quaternion.AngleAxis(-mouseCurrent.y, Vector3.right);
            //player horizontal axis (without model and animation so just horizontal)
            character.transform.localRotation = Quaternion.AngleAxis(mouseCurrent.x, character.transform.up);
          /*  if (recoil)
            {
                recoil = false;
            }*/
        }
        /*if (mouseSensChanged)
        {
            mouseSensChanged = false;
            UpdateMouseSensitivity();
        }
        if (fovChanged)
        {
            fovChanged = false;
            UpdateCameraFOV();
        }*/

        //No solutions so far, must force update
        UpdateMouseSensitivity();
        UpdateCameraFOV();
    }
	public void SetMouseSensitivitySlider(float sens) {
        //Debug.Log("sens="+sens);
        //Debug.Log("mouseSens=" + mouseSensitivity);
        PlayerPrefs.SetFloat("mouseSensitivity", sens);
        //mouseSensChanged = true;
    }
    public void SetFOVSlider(float fov)
    {
        //Debug.Log("prevFOV=" + gameObject.GetComponent<Camera>().fieldOfView);
        //Debug.Log("currentFOV=" + gameObject.GetComponent<Camera>().fieldOfView);
        PlayerPrefs.SetFloat("FOV", fov);
        //fovChanged = true;
    }

    public void StopCam()
    {
        gamePlaying = false;
    }
    public void ResumeCam()
    {
        gamePlaying = true;
    }
    public void Recoil(bool ads)
    {
        //recoil = true;
        transform.parent.GetComponent<CameraRecoil>().Recoil(ads);
    }
    public void UpdateWeaponInfo(float maxHRecoil, float minHRecoil, float VRecoil)
    {
        maxHorizontalRecoil = maxHRecoil;
        minHorizontalRecoil = minHRecoil;
        verticalRecoil = VRecoil;
    }
    void UpdateMouseSensitivity()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 20f);
    }
    void UpdateCameraFOV()
    {
        fieldOfView = PlayerPrefs.GetFloat("FOV", 90f);
        gameObject.GetComponent<Camera>().fieldOfView = fieldOfView;
    }
}
