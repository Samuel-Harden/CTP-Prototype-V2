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

    private float idleTimer;
    private float idleTime = 30.0f;

    private List<Vector3> waypoints;

    //[SerializeField] Color waypointColor = Color.magenta;
    [SerializeField] float updateDistance = 1.75f;

    [SerializeField] float maxSteerAngle = 40.0f;
    [SerializeField] float maxPower = 10.0f;
    [SerializeField] float breakPower = 0.1f;

    [SerializeField] WheelCollider wheelFL;
    [SerializeField] WheelCollider wheelFR;

    [SerializeField] Transform viewStart;

    private Vector3 sensorPos;

    public void Initialise(AITrafficController _controller, int _carID)
    {
        direction = "posZ";
        controller = _controller;
        vehicleID = _carID;

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


    private void Update()
    {
        Sensors();
    }


    private void FixedUpdate()
    {
        if (initialised)
        {
            //Sensors();

            if (hasWaypoint)
            {
                if (moving)
                {
                    ApplySteer();

                    ApplyForce();

                    CheckWaypointDistance();
                }

                /*if (!moving )
                    idleTimer += Time.deltaTime;

                if (idleTimer > idleTime)
                {
                    ReleaseBrake();
                    moving = true;
                    idleTime = 0.0f;
                }*/
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


    private void Sensors()
    {
        sensorPos = transform.position;
        sensorPos.z += 0.3f;
        sensorPos.y += 0.1f;

        RaycastHit hit;

        if(Physics.Raycast(viewStart.position, transform.forward, out hit, 0.15f))
        {
            ApplyBrake();
            moving = false;
        }

        else
        {
            ReleaseBrake();
            moving = true;
        }

        if (hit.point != Vector3.zero)
            Debug.DrawLine(viewStart.position, hit.point);
    }


    /*private void OnTriggerEnter(Collider other)
    {
        stopedObject = other.gameObject;
        ApplyBrake();
        moving = false;

        if (other.gameObject.layer == LayerMask.NameToLayer("TrafficSignal"))
            atSignal = true;
    }


    private void OnTriggerExit(Collider other)
    {
        ReleaseBrake();
        moving = true;
        idleTimer = 0.0f;

        if (other.gameObject.layer == LayerMask.NameToLayer("TrafficSignal"))
            atSignal = false;
    }*/


    private void GetNewWaypoints()
    {
        waypoints = controller.GetWaypoint(row, col, direction, vehicleID);
    }


    /*private void OnDrawGizmos()
    {
        if(hasWaypoint)
        {
            Gizmos.color = waypointColor;
            Gizmos.DrawWireCube(waypoints[0], new Vector3(0.2f, 0.2f, 0.2f));
        }
    }*/
}
