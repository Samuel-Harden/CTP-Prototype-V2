using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    private float delayTime;
    private float sectionTime;

    [SerializeField] float sectionTimeAlternate = 2.0f;
    [SerializeField] float sectionTimeAllowTraffic = 8.5f;

    [SerializeField] float delayTimeAlternate = 1.0f;
    [SerializeField] float delayTimeAllowTraffic = 1.5f;

    private RoadSection roadSection;

    private List<TrafficLight> trafficLights;

    private float delayTimer = 0;
    private float sectionTimer = 0;

    private int currentLight;

    private bool onDelay;

    private SignalState signalState;

    enum SignalState { AllowTraffic, Alternate };


    public void Initialise(GameObject _trafficLight)
    {
        trafficLights = new List<TrafficLight>();

        roadSection = GetComponent<RoadSection>();

        SetupIntersection(_trafficLight);

        currentLight = 0;

        SignalState signalState = SignalState.Alternate;

        delayTime = delayTimeAlternate;
        sectionTime = sectionTimeAlternate;
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
                UpdateState();

                DisableCurrentTrafficLight();

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
                EnableAllTrafficLights();

                //Reset Timer
                sectionTimer = 0;

                // Set to On Delay
                onDelay = true;
            }
        }
    }


    private void UpdateState()
    {
        foreach (TrafficLight light in trafficLights)
        {
            if (light.IsTrafficWaiting())
            {
                signalState = SignalState.AllowTraffic;
                sectionTime = sectionTimeAllowTraffic;
                return;
            }
        }

        signalState = SignalState.Alternate;
        sectionTime = sectionTimeAlternate;
    }


    private void DisableCurrentTrafficLight() // GREEN
    {
        int counter = 0;
        if (signalState == SignalState.AllowTraffic)
        {
            while (counter < trafficLights.Count)
            {
                if (trafficLights[currentLight].IsTrafficWaiting())
                {
                    trafficLights[currentLight].SetLight(true); // Green Light

                    trafficLights[currentLight].TrafficWaiting(false);

                    currentLight++;

                    if (currentLight == trafficLights.Count)
                        currentLight = 0;

                    return;
                }

                currentLight++;

                if (currentLight == trafficLights.Count)
                    currentLight = 0;

                counter++;
            }
        }

        if (signalState == SignalState.Alternate)
        {
            trafficLights[currentLight].SetLight(true); // Green Light

            currentLight++;

            if (currentLight == trafficLights.Count)
                currentLight = 0;
        }
    }


    private void EnableAllTrafficLights() // RED
    {
        foreach(TrafficLight trafficLight in trafficLights)
        {
            trafficLight.SetLight(false); // Red Light
        }
    }


    private void SetupIntersection(GameObject _trafficLight)
    {
        // Set locations of colliders to stop Vehicles
        foreach (RoadSection junction in roadSection.GetNeighbours())
        {
            // Check if connection is above or below this junction
            if(junction.Row() > roadSection.Row())
            {
                Vector3 pos = new Vector3(transform.position.x + 0.45f, 0.07f, transform.position.z + 0.55f);
                Quaternion rot = Quaternion.Euler(-90.0f, 0.0f, 180.0f);

                GenerateTrafficLight(pos, rot, _trafficLight);
            }

            if(junction.Row() < roadSection.Row())
            {
                Vector3 pos = new Vector3(transform.position.x - 0.45f, 0.07f, transform.position.z - 0.55f);
                Quaternion rot = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

                GenerateTrafficLight(pos, rot, _trafficLight);
            }

            // Check if connection is to the left or right of this junction
            if (junction.Col() > roadSection.Col())
            {
                Vector3 pos = new Vector3(transform.position.x + 0.55f, 0.07f, transform.position.z - 0.45f);
                Quaternion rot = Quaternion.Euler(-90.0f, 0.0f, -90.0f);

                GenerateTrafficLight(pos, rot, _trafficLight);
            }

            if(junction.Col() < roadSection.Col())
            {
                Vector3 pos = new Vector3(transform.position.x - 0.55f, 0.07f, transform.position.z + 0.45f);
                Quaternion rot = Quaternion.Euler(-90.0f, 0.0f, 90.0f);

                GenerateTrafficLight(pos, rot, _trafficLight);
            }
        }
    }


    private void GenerateTrafficLight(Vector3 _pos, Quaternion _rotation, GameObject _trafficLight)
    {
        var trafficLight = Instantiate(_trafficLight, _pos, _rotation);

        trafficLight.GetComponent<TrafficLight>().Initialise();

        trafficLight.transform.parent = transform;

        trafficLights.Add(trafficLight.GetComponent<TrafficLight>());
    }
}
