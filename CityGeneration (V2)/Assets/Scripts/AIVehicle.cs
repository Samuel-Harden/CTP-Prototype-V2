using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicle : MonoBehaviour
{
    //[SerializeField] Color waypointColor = Color.magenta;
    [SerializeField] float updateDistance = 1.75f;

    [Space]
    [SerializeField] float maxSteerAngle = 40.0f;
    [SerializeField] float maxPower = 10.0f;

    [Space]
    [SerializeField] WheelCollider wheelFL;
    [SerializeField] WheelCollider wheelFR;

    [Space]
    [Header("Vehicle Sensors")]
    [SerializeField] Transform sensorLeft;
    [SerializeField] Transform sensorRight;
    [SerializeField] Transform sensorCenter;

    private AITrafficController controller;

    private string direction;

    private int row;
    private int col;
    private int vehicleID;

    private bool hasWaypoint;
    private bool initialised;
    private bool moving;

    private float idleTimer;
    private float idleTime   = 30.0f;
    private float breakPower =  0.1f;

    private List<Vector3> waypoints;

    private Transform currentSensor;


    public void Initialise(AITrafficController _controller, int _carID)
    {
        direction = "posZ";

        controller = _controller;

        vehicleID = _carID;

        currentSensor = sensorCenter;

        initialised = true;
    }


    public void SetCurrentSection(int _row, int _col)
    {
        row = _row;
        col = _col;
    }


    public void SetWaypoints(List<Vector3> _waypoints)
    {
        waypoints = _waypoints;

        hasWaypoint = true;
        moving = true;
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


    private void FixedUpdate()
    {
        if (initialised)
        {
            SetCurrentSensor();

            Sensors();

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


    private void SetCurrentSensor()
    {
        float steer = wheelFL.steerAngle;

        if (steer < -6.0f)
            currentSensor = sensorLeft;

        else if (steer > 6.0f)
            currentSensor = sensorRight;

        else
        {
            if (currentSensor != sensorCenter)
            currentSensor = sensorCenter;
        }
    }


    private void Sensors()
    {
        RaycastHit hit;

        Quaternion rot = Quaternion.Euler(0.0f, wheelFL.steerAngle, 0.0f);

        Vector3 direction = rot * (wheelFL.transform.forward / 5);

        //Debug.DrawLine(currentSensor.position, currentSensor.position + direction);

        if (Physics.Raycast(currentSensor.position, direction, out hit, 0.2f))
        {
            ApplyBrake();
            moving = false;
        }

        else
        {
            ReleaseBrake();
            moving = true;
        }
    }


    private void GetNewWaypoints()
    {
        waypoints = controller.GetWaypoint(row, col, direction, vehicleID);
    }


    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Quaternion rot = Quaternion.Euler(0.0f, wheelFL.steerAngle, 0.0f);

        Vector3 direction = rot * (wheelFL.transform.forward / 4);

        if (currentSensor != null)
        {
            Gizmos.DrawWireCube(currentSensor.position + direction, new Vector3(0.1f, 0.1f, 0.1f));

            Gizmos.DrawWireSphere(currentSensor.position, 0.05f);
        }
    }*/
}
