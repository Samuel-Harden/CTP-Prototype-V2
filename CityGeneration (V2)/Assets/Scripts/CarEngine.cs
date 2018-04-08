using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45;
    public float updateDistance = 0.5f;

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;

    private List<Transform> nodes;
    private int currentNode = 0;

	// Use this for initialization
	void Start ()
    {
        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if(pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        ApplySteer();
        ApplyForce();
        CheckWaypointDistance();
	}


    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        Debug.Log(newSteer);

        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }


    private void ApplyForce()
    {
        wheelFL.motorTorque = 20.0f;
        wheelFR.motorTorque = 20.0f;
    }


    private void CheckWaypointDistance()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < updateDistance)
        {
            currentNode += 1;

            if (currentNode == nodes.Count)
                currentNode = 0;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if(nodes.Count != 0)
        {
            foreach (Transform pos in nodes)
            {
                Gizmos.DrawWireSphere(pos.position, 0.5f);
            }
        }
    }
}
