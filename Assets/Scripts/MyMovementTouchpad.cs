using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMovementTouchpad : MonoBehaviour
{
    public GameObject targetobject;

    public float mspeed = 0.1f;
    public Vector3 defaultPosition; // Default position of the hammer

    Vector2 currentMousePosition;
    Vector2 mouseDeltaPosition;
    Vector2 lastMousePosition;

    public static MyMovementTouchpad instance;

    [HideInInspector]
    public bool istouchpadactive;

    private void Start()
    {
        instance = this;
        ResetMousePosition();

        if (targetobject != null)
        {
            defaultPosition = targetobject.transform.position; // Store the default position
        }
    }

    public void ResetMousePosition()
    {
        if (Input.touchCount == 1)
        {
            currentMousePosition = Input.GetTouch(0).position;
        }
        else
        {
            currentMousePosition = Input.mousePosition;
        }

        lastMousePosition = currentMousePosition;
        mouseDeltaPosition = currentMousePosition - lastMousePosition;
    }

    void LateUpdate()
    {
        if (istouchpadactive)
        {
            if (Input.touchCount == 1)
            {
                currentMousePosition = Input.GetTouch(0).position;
            }
            else
            {
                currentMousePosition = Input.mousePosition;
            }

            mouseDeltaPosition = currentMousePosition - lastMousePosition;

            if (targetobject != null)
            {
                // Move the hammer
                targetobject.transform.Translate(mouseDeltaPosition.x * mspeed, mouseDeltaPosition.y * mspeed, 0f);
            }

            lastMousePosition = currentMousePosition;
        }
    }

    public void ActivateTouchpad()
    {
        ResetMousePosition();
        istouchpadactive = true;
    }

    public void DeactivateTouchpad()
    {
        istouchpadactive = false;

        // Return the hammer to its default position
        if (targetobject != null)
        {
            targetobject.transform.position = defaultPosition;
        }
    }
}