using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicle : MonoBehaviour
{
    private AITrafficController controller;

    private string direction;

    private int row;
    private int col;
    private int vehicleID;
    private bool hasWaypoint;
    private bool initialised;
    private bool moving;

    private List<Vector3> waypoints;

    [SerializeField] LayerMask vehicleMask;

    [SerializeField] Color waypointColor = Color.magenta;
    [SerializeField] float updateDistance = 0.75f;

    [SerializeField] float maxSteerAngle = 40.0f;
    [SerializeField] float maxPower = 15.0f;
    [SerializeField] float breakPower = 0.1f;

    [SerializeField] WheelCollider wheelFL;
    [SerializeField] WheelCollider wheelFR;

    private Rigidbody rb;

    float speed = 1.0f;

    public void Initialise(AITrafficController _controller, int _carID)
    {
        direction = "posZ";
        controller = _controller;
        vehicleID = _carID;

        rb = GetComponent<Rigidbody>();

        hasWaypoint = false;

        initialised = true;

        moving = true;
    }


    public void SetCurrentSection(int _row, int _col)
    {
        row = _row;
        col = _col;
    }


    public void UpdateData(string _direction, int _row, int _col)
    {
        direction = _direction;
        row = _row;
        col = _col;
    }


    public void SetMoving(bool _moving)
    {
        moving = _moving;
    }

    // Update is called once per frame
    void Update ()
    {

    }


    private void FixedUpdate()
    {
        if (initialised)
        {
            if (hasWaypoint)
            {
                if (moving)
                {
                    ApplySteer();

                    ApplyForce();

                    CheckWaypointDistance();
                }
            }

            if (!hasWaypoint)
            {
                GetNewWaypoints();

                hasWaypoint = true;
            }
        }
    }


    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, waypoints[0]) < updateDistance)
        {
            waypoints.RemoveAt(0);

            if (waypoints.Count == 0)
                hasWaypoint = false;
        }
    }


    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(waypoints[0]);

        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }


    private void ApplyForce()
    {
        wheelFL.motorTorque = maxPower;
        wheelFR.motorTorque = maxPower;
    }


    private void ApplyBrake()
    {
        wheelFL.brakeTorque = breakPower;
        wheelFR.brakeTorque = breakPower;
    }


    private void ReleaseBrake()
    {
        wheelFL.brakeTorque = 0.0f;
        wheelFR.brakeTorque = 0.0f;
    }


    private void OnTriggerEnter(Collider other)
    {
        ApplyBrake();
        moving = false;
    }


    private void OnTriggerExit(Collider other)
    {
        ReleaseBrake();
        moving = true;
    }


    private void GetNewWaypoints()
    {
        waypoints = controller.GetWaypoint(row, col, direction, vehicleID);
    }


    private void OnDrawGizmos()
    {
        if(hasWaypoint)
        {
            Gizmos.color = waypointColor;
            Gizmos.DrawWireCube(waypoints[0], new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
}
