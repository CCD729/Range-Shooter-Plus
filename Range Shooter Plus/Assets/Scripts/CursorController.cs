using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursorTex;
    void Awake() { Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.ForceSoftware); }
    void Start()
    {
        //Cursor.visible = false;

    }
    void Update()
    {
        //Vector2 cursorPos = Input.mousePosition;
        //transform.position = cursorPos;
    }
}
