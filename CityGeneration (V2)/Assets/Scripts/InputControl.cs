using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    [SerializeField] private CityGen cityGen;

    [SerializeField] float lookSpeedH = 4f;
    [SerializeField] float lookSpeedV = 4f;
    [SerializeField] float zoomSpeed = 4f;
    [SerializeField] float dragSpeed = 20f;

    [SerializeField] float moveSpeed = 8.0f;
    [SerializeField] float strafeSpeed = 4.0f;

    private float yaw = 0f;
    private float pitch = 0f;
         
    void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            LeftClickInput();
        }

        //Look around with Right Mouse
        if (Input.GetMouseButton(1))
        {
            RightClickInput();
        }
     
        //drag camera around with Middle Mouse
        if (Input.GetMouseButton(2))
        {
            MiddleClickInput();
        }

        MouseWheelInput();

        KeyboardInputs();
    }


    private void LeftClickInput()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit)) // If we havnt hit anything
        {
            if (!EventSystem.current.IsPointerOverGameObject()) // If we arn't over a game object
                cityGen.ResetCurrentBuilding(); // Deselect current building
        }
    }


    private void RightClickInput()
    {
        yaw += lookSpeedH * Input.GetAxis("Mouse X");

        pitch -= lookSpeedV * Input.GetAxis("Mouse Y");
     
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }


    private void MiddleClickInput()
    {
        transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed,
            -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
    }


    private void MouseWheelInput()
    {
        //Zoom in and out with Mouse Wheel
        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);
    }


    private void KeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = moveSpeed * 2;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = moveSpeed / 2;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * strafeSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -transform.right * strafeSpeed * Time.deltaTime;
        }
    }
}
