using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public bool cursorLock = true;
    public Texture2D customCursor;
    public Vector2 hotspot = Vector2.zero;
    public bool cursorApplied = false;

    public bool isInGame = false;

    void Update()
    {
        DrawMyCursor();
    }

    public virtual void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }

        if (cursorLock && isInGame)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public virtual void DrawMyCursor()
    {
        if (cursorLock && !cursorApplied && customCursor != null)
        {
            Cursor.SetCursor(customCursor, hotspot, CursorMode.ForceSoftware);
            cursorApplied = true;
        }
        else if (!cursorLock && cursorApplied)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            cursorApplied = false;
        }
    }

}
