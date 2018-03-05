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

    private List<Vector3> waypoints;

    [SerializeField] Color waypointColor = Color.magenta;
    [SerializeField] float updateDistance = 0.75f;

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
    }

    // Update is called once per frame
    void Update ()
    {
        if(initialised)
        {
            if (hasWaypoint)
            {
                //rb.AddForce(transform.forward * 0.5f);
                // if distance is less than x
                // hasWaypoint = false;

                float step = speed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, new Vector3(waypoints[0].x, transform.position.y, waypoints[0].z), step);

            if(Vector3.Distance(transform.position, waypoints[0]) < updateDistance)
                {
                    waypoints.RemoveAt(0);

                    if(waypoints.Count == 0)
                        hasWaypoint = false;
                }
            }

            if (!hasWaypoint)
            {
                GetNewWaypoints();

                hasWaypoint = true;
            }
        }
    }


    /*private void FixedUpdate()
    {
        {
            if(hasWaypoint)
            {
                Vector3 direction = new Vector3(waypoint.x, transform.position.y, waypoint.z) - transform.position;
                Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);

                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 0.5f * Time.time);
            }
        }
    }*/


    private void GetNewWaypoints()
    {
        waypoints = controller.GetWaypoint(row, col, direction, vehicleID);
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


    private void OnDrawGizmos()
    {
        if(hasWaypoint)
        {
            Gizmos.color = waypointColor;
            Gizmos.DrawWireCube(waypoints[0], new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
}
