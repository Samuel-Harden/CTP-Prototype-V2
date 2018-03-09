using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    [SerializeField] float delayTime = 2.0f;
    [SerializeField] float sectionTime = 8.5f;

    private RoadSection roadSection;

    private List<GameObject> trafficZones;

    private float delayTimer = 0;
    private float sectionTimer = 0;

    private int currentSection;

    private bool onDelay;

    private Vector3 currentGreen;


    public void Initialise()
    {
        trafficZones = new List<GameObject>();

        roadSection = GetComponent<RoadSection>();

        SetupIntersection();

        currentSection = 0;
    }


    private void Update()
    {
        // Cycle through traffic Sections
        // after X secs disable one direction,
        // and enable another x sec later
        // continuously cycle through connections

        if (onDelay)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > delayTime)
            {
                // allow traffic through this section
                DisableCurrentTrafficZone();

                // reset timer
                delayTimer = 0;

                // set to not on delay
                onDelay = false;
            }
        }

        if(!onDelay)
        {
            sectionTimer += Time.deltaTime;

            if(sectionTimer > sectionTime)
            {
                // Set Delay & enable all sections
                EnableAllTrafficZones();

                //Reset Timer
                sectionTimer = 0;

                // Set to On Delay
                onDelay = true;
            }
        }
    }


    private void DisableCurrentTrafficZone()
    {
        trafficZones[currentSection].transform.position = new Vector3(trafficZones[currentSection].transform.position.x, 3.0f, trafficZones[currentSection].transform.position.z);

        currentGreen = trafficZones[currentSection].transform.position;

        currentSection++;

        if (currentSection == trafficZones.Count)
            currentSection = 0;
    }


    private void EnableAllTrafficZones()
    {
        foreach(GameObject trafficZone in trafficZones)
        {
            trafficZone.transform.position = new Vector3(trafficZone.transform.position.x, 0.1f, trafficZone.transform.position.z);
        }
    }


    private void SetupIntersection()
    {
        // Set locations of colliders to stop Vehicles
        foreach (RoadSection junction in roadSection.GetNeighbours())
        {
            // Check if connection is above or below this junction
            if(junction.Row() > roadSection.Row())
            {
                Vector3 pos = new Vector3(transform.position.x + 0.2f, 0.1f, transform.position.z + 0.4f);

                GenerateTrafficZone(pos);
            }

            if(junction.Row() < roadSection.Row())
            {
                Vector3 pos = new Vector3(transform.position.x - 0.2f, 0.1f, transform.position.z - 0.4f);

                GenerateTrafficZone(pos);
            }

            // Check if connection is to the left or right of this junction
            if (junction.Col() > roadSection.Col())
            {
                Vector3 pos = new Vector3(transform.position.x + 0.4f, 0.1f, transform.position.z - 0.2f);

                GenerateTrafficZone(pos);
            }

            if(junction.Col() < roadSection.Col())
            {
                Vector3 pos = new Vector3(transform.position.x - 0.4f, 0.1f, transform.position.z + 0.2f);

                GenerateTrafficZone(pos);
            }
        }
    }

    
    private void GenerateTrafficZone(Vector3 _pos)
    {
        GameObject trafficZone = new GameObject("TrafficZone");

        trafficZone.transform.position = _pos;

        trafficZone.transform.rotation = Quaternion.identity;

        trafficZone.transform.parent = transform;

        trafficZone.AddComponent<BoxCollider>();

        trafficZone.GetComponent<BoxCollider>().isTrigger = true;

        trafficZone.GetComponent<BoxCollider>().transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);

        trafficZone.layer = LayerMask.NameToLayer("TrafficSignal");
;
        trafficZones.Add(trafficZone);


    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (trafficZones.Count != 0)
        {
            if(!onDelay)
            {
                Gizmos.DrawWireSphere(new Vector3(currentGreen.x, 0.1f, currentGreen.z), 0.15f);
            }
        }
    }
}
