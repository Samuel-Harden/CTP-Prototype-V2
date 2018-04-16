using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
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
        //Look around with Right Mouse
        if (Input.GetMouseButton(1))
        {
            yaw += lookSpeedH * Input.GetAxis("Mouse X");

            pitch -= lookSpeedV * Input.GetAxis("Mouse Y");
     
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
     
        //drag camera around with Middle Mouse
        if (Input.GetMouseButton(2))
        {
            transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed,
                -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
        }
     
        //Zoom in and out with Mouse Wheel
        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);

        KeyboardInputs();
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
