﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGen : MonoBehaviour
{
    [SerializeField] int cityWidth;
    [SerializeField] int cityLength;
    [SerializeField] int maxDepth;
    [SerializeField] float perlinNoise = 50;
    [SerializeField] bool showPositions;
    [SerializeField] int minBuildDepth = 2;

    [SerializeField] GameObject lotPrefab;
    [SerializeField] GameObject lotContainer;

    private int tileSize = 1;

    private List<Vector3> perlinPositions;
    private List<BuildingLot> buildingLots;

    private RoadGen roadGen;
    private ObjectGen objectGen;
    private AITrafficController trafficController;


    private void Awake()
    {
        buildingLots = new List<BuildingLot>();

        roadGen = GetComponent<RoadGen>();
        objectGen = GetComponent<ObjectGen>();

        trafficController = GetComponentInChildren<AITrafficController>();
    }


    public void GenerateAll()
    {
        GeneratePositions();

        GenerateBuildingLots();

        GenerateRoads();

        GeneratePaths();

        GenerateBuildings();

        GenerateTraffic();
    }


    public void GenerateTraffic()
    {
        if (roadGen.RoadsValid()) // if there are roads, we can spawn traffic
        trafficController.Initialise(roadGen.GetRoadNetwork(),
            roadGen.GetRoadNetworkList(), cityWidth, cityLength);
    }


    public void RegenBuildings()
    {
        if (buildingLots.Count == 0)
            return;

        GenerateBuildingLots();

        UpdateLotSize();

        GenerateLotData();

        objectGen.GenerateBuildings(buildingLots);
    }


    void GeneratePositions()
    {
        perlinPositions = new List<Vector3>();

        float seed = Random.Range(0, 100);

        for (int l = tileSize / 2; l < cityLength; l += tileSize)
        {
            for (int w = tileSize / 2; w < cityWidth; w += tileSize)
            {
                int result = (int)(Mathf.PerlinNoise(w / perlinNoise + seed,
                    l / perlinNoise + seed) * 100);

                Vector3 pos = new Vector3(w, 0, l);

                if(result > 50)
                    perlinPositions.Add(pos);
            }
        }
    }


    private void GenerateBuildingLots()
    {
        Vector3 pos = Vector3.zero;
        int nodeSizeX = cityWidth;
        int nodeSizeZ = cityLength;

        ClearBuildingLots();

        var buildingLot = Instantiate(lotPrefab, pos, Quaternion.identity);

        buildingLots.Add(buildingLot.GetComponent<BuildingLot>());

        buildingLot.GetComponent<BuildingLot>().Initialise(pos, nodeSizeX, nodeSizeZ, maxDepth,
            lotPrefab, tileSize, buildingLots, perlinPositions, 0);

        ClearDividedLots();
    }


    private void ClearBuildingLots()
    {
        foreach (BuildingLot lot in buildingLots)
        {
            Destroy(lot.gameObject);
        }

        buildingLots.Clear();
    }


    private void GenerateRoads()
    {
        roadGen.Initialise(buildingLots, cityWidth, cityLength, tileSize);

        UpdateLotSize();
    }


    private void GeneratePaths()
    {
        objectGen.Initialze(minBuildDepth, roadGen.RoadHeight());

        objectGen.GeneratePaths(buildingLots);
    }


    private void GenerateBuildings()
    {
        GenerateLotData();

        objectGen.GenerateBuildings(buildingLots);
    }


    private void UpdateLotSize()
    {
        foreach (BuildingLot lot in buildingLots)
        {
            lot.RecalculateLotSize();
        }
    }


    private void GenerateLotData()
    {
        foreach (BuildingLot lot in buildingLots)
        {
            lot.CalculateBuildingData();
        }
    }


    private void ClearDividedLots()
    {
        for (int i = buildingLots.Count - 1; i >= 0; i--)
        {
            if (buildingLots[i].Divided())
            {
                Destroy(buildingLots[i].gameObject);
                buildingLots.RemoveAt(i);
            }

            buildingLots[i].transform.parent = lotContainer.transform;
        }
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (!showPositions)
            return;

        Gizmos.color = Color.white;

        foreach (Vector3 pos in perlinPositions)
        {
            Gizmos.DrawWireSphere(pos, 1);
        }
    }
}
