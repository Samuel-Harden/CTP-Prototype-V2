using System.Collections;
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
    [SerializeField] GameObject baseLotContainer;

    private int tileSize = 1;
    private int currentBuilding = -1;

    private List<Vector3> perlinPositions;
    private List<BuildingLot> buildingLots;
    private List<GameObject> baseBuildingLots;

    private RoadGen roadGen;
    private ObjectGen objectGen;
    private AITrafficController trafficController;


    private void Awake()
    {
        buildingLots = new List<BuildingLot>();
        baseBuildingLots = new List<GameObject>();

        roadGen = GetComponent<RoadGen>();
        objectGen = GetComponent<ObjectGen>();

        trafficController = GetComponentInChildren<AITrafficController>();
    }


    public void GenerateAll()
    {
        GeneratePositions();

        GenerateBuildingLots(buildingLots);

        GenerateRoads(buildingLots);

        GeneratePaths();

        GenerateBaseBuildings();

        GenerateBuildings(buildingLots);

        currentBuilding = -1;
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

        GenerateBuildingLots(buildingLots);

        UpdateLotSize(buildingLots);

        GenerateBuildings(buildingLots);

        currentBuilding = -1;
    }


    public void SetCurrentBuilding(int _lotIndex)
    {
        currentBuilding = _lotIndex;
    }


    public void RegenBuilding()
    {
        if (currentBuilding == -1)
            return;

        List<BuildingLot> newLot = new List<BuildingLot>();

        var lot = Instantiate(lotPrefab, Vector3.zero, Quaternion.identity);

        lot.GetComponent<BuildingLot>().DeepCopyData(baseBuildingLots[currentBuilding].GetComponent<BuildingLot>());

        Destroy(buildingLots[currentBuilding].gameObject);

        newLot.Add(lot.GetComponent<BuildingLot>());

        UpdateLotSize(newLot);

        GenerateBuildings(newLot);

        //buildingLots.RemoveAt(currentBuilding); // was wrong, trying to access a deleted entry

        buildingLots[currentBuilding] = null; // Null first as its already been deleted!

        buildingLots.Insert(currentBuilding, newLot[0]);

        lot.transform.parent = lotContainer.transform;
    }


    private void GeneratePositions()
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


    private void GenerateBuildingLots(List<BuildingLot> _lots)
    {
        Vector3 pos = Vector3.zero;
        int nodeSizeX = cityWidth;
        int nodeSizeZ = cityLength;

        ClearBuildingLots(_lots);

        var buildingLot = Instantiate(lotPrefab, pos, Quaternion.identity);

        _lots.Add(buildingLot.GetComponent<BuildingLot>());

        buildingLot.GetComponent<BuildingLot>().Initialise(pos, nodeSizeX, nodeSizeZ, maxDepth,
            lotPrefab, tileSize, buildingLots, perlinPositions, 0);

        ClearDividedLots();

        SetLotIDs();
    }


    private void ClearBuildingLots(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
        {
            if (lot != null)
                Destroy(lot.gameObject);
        }

        _lots.Clear();
    }


    private void GenerateRoads(List<BuildingLot> _lots)
    {
        roadGen.Initialise(buildingLots, cityWidth, cityLength, tileSize);

        UpdateLotSize(_lots);
    }


    private void GeneratePaths()
    {
        objectGen.Initialze(minBuildDepth, roadGen.RoadHeight());

        objectGen.GeneratePaths(buildingLots);
    }


    private void GenerateBuildings(List<BuildingLot> _lots)
    {
        GenerateLotData(_lots);

        objectGen.GenerateBuildings(_lots);
    }


    private void GenerateBaseBuildings()
    {
        foreach (GameObject lot in baseBuildingLots)
        {
            Destroy(lot.gameObject);
        }

        baseBuildingLots.Clear();

        foreach (BuildingLot lot in buildingLots)
        {
            // Create Backup
            var backupLot = Instantiate(lot.gameObject, lot.transform.position, lot.transform.rotation);

            baseBuildingLots.Add(backupLot);

            backupLot.GetComponent<BuildingLot>().DeepCopyData(lot);

            backupLot.transform.parent = baseLotContainer.transform;
        }
    }



    private void UpdateLotSize(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
        {
            lot.RecalculateLotSize();
        }
    }


    private void GenerateLotData(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
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


    private void SetLotIDs()
    {
        int index = 0;

        foreach (BuildingLot lot in buildingLots)
        {
            lot.SetLotIndex(index);
            index++;
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
