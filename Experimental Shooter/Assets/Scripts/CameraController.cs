using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
	public Slider slider;
    private Vector2 mouseCurrent;
    public float sensitivity = 2f;
    private GameObject character;
    bool gamePlaying;
    bool recoil = false;

    void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 2f);
        slider.value = sensitivity;
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
            var mouseGet = new Vector2(Input.GetAxis("Mouse X") * sensitivity, Input.GetAxis("Mouse Y") * sensitivity);
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
            transform.localRotation = Quaternion.AngleAxis(-mouseCurrent.y, Vector3.right);
            character.transform.localRotation = Quaternion.AngleAxis(mouseCurrent.x, character.transform.up);
            if (recoil)
            {
                recoil = false;
            }
        }
		
    }

	public void Sensitivity(float sens) {
		sensitivity = slider.value;
        PlayerPrefs.SetFloat("Sensitivity", slider.value);
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
}
