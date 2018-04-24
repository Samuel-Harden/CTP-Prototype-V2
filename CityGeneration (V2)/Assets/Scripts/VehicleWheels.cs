using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VehicleWheels : MonoBehaviour
{
    [SerializeField] WheelCollider wheel;

    private Vector3 wheelPos = new Vector3();
    private Quaternion wheelRot = new Quaternion();


    private void Update()
    {
        wheel.GetWorldPose(out wheelPos, out wheelRot);

        wheelRot = Quaternion.Euler(wheelRot.eulerAngles.x,
            wheelRot.eulerAngles.y, wheelRot.eulerAngles.z + 90.0f);

        transform.position = wheelPos;

        transform.rotation = wheelRot;
    }
}
