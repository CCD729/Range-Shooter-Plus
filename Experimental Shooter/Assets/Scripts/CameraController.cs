using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
	public Slider sensSlider, fovSlider;
    private Vector2 mouseCurrent;
    public float mouseSensitivity = 20f;
    public float fieldOfView = 100f;
    private GameObject character;
    public GameObject gunPosParent;
    bool gamePlaying;
    bool recoil = false;
    bool mouseSensChanged, fovChanged = false;

    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 20f);
        fieldOfView = PlayerPrefs.GetFloat("FOV", 100f);
        gameObject.GetComponent<Camera>().fieldOfView = fieldOfView;
        sensSlider.value = mouseSensitivity;
        fovSlider.value = fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseCurrent.x = 0;
        character = this.transform.parent.gameObject;
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
            if (recoil)
            {
                mouseCurrent.y += 0.5f;
                mouseCurrent.x += Random.Range(-0.5f, 0.5f);
            }
            //in-game model vertical axis for better view (WIP)
            //gunPosParent.transform.localRotation = Quaternion.AngleAxis(-mouseCurrent.y, Vector3.right);
            //camera vertical axis
            transform.localRotation = Quaternion.AngleAxis(-mouseCurrent.y, Vector3.right);
            //player horizontal axis (without model and animation so just horizontal)
            character.transform.localRotation = Quaternion.AngleAxis(mouseCurrent.x, character.transform.up);
            if (recoil)
            {
                recoil = false;
            }
        }
        /*if (mouseSensChanged)
        {
            Debug.Log("SHITTTTT");
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
    public void Recoil()
    {
        recoil = true;
    }
    void UpdateMouseSensitivity()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
    }
    void UpdateCameraFOV()
    {
        fieldOfView = PlayerPrefs.GetFloat("FOV");
        gameObject.GetComponent<Camera>().fieldOfView = fieldOfView;
    }
}
