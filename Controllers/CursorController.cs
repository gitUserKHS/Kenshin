using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour
{
    bool cursorVisible = false;

    void Update()
    {
        if (cursorVisible == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            cursorVisible = !cursorVisible;
        }
    }
}
