using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITrafficController : MonoBehaviour
{
    [SerializeField] List<GameObject> vehiclePrefabs;
    [SerializeField] int noVehicles;

    [SerializeField] Transform vehicleContainer;

    private RoadSection[,] roadNetwork;
    private List<RoadSection> roadNetworkList;

    private List<AIVehicle> vehicles;
    private List<RoadSection> index9Sections;


    private int cityWidth;
    private int cityLength;

	public void Initialise(RoadSection[,] _roadNetwork, List<RoadSection> _roadNetworkList,
        int _cityWidth, int _cityLength)
    {
        roadNetworkList = _roadNetworkList;

        cityWidth = _cityWidth;
        cityLength = _cityLength;

        roadNetwork = new RoadSection[cityLength, cityWidth];

        roadNetwork = _roadNetwork;

        GenerateVehicles();
    }


    private void Awake()
    {
        vehicles = new List<AIVehicle>();

        index9Sections = new List<RoadSection>();
    }


    private void GenerateVehicles()
    {
        ClearVehicles();

        for (int i = 0; i < noVehicles; i++)
        {
            var vehicle = Instantiate(vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)],
                Vector3.zero, Quaternion.identity);

            vehicle.GetComponent<AIVehicle>().Initialise(this, i);

            vehicles.Add(vehicle.GetComponent<AIVehicle>());

            vehicle.transform.parent = vehicleContainer.transform;
        }

        SetupPositions();
    }


    private void ClearVehicles()
    {
        foreach (AIVehicle vehicle in vehicles)
        {
            Destroy(vehicle.gameObject);
        }

        vehicles.Clear();
    }


    private void SetupPositions()
    {
        // Can use indexs to identify what road section we are on,
        // Ideally only use a specific tile type to spawn at the start,
        //ie Index 9 (Spawn them facing up). So we know we want a waypoint
        // with a -x value (left side), and we know the facing off all vehicles! :)

        index9Sections.Clear();

        // Create List of all index 9 sections
        for (int i = 0; i < roadNetworkList.Count; i++)
        {
            if(roadNetworkList[i].Index() == 9)
            {
                index9Sections.Add(roadNetworkList[i]);
            }
        }

        foreach (AIVehicle vehicle in vehicles)
        {
            int i = Random.Range(0, index9Sections.Count);

            List<Vector3> positions = new List<Vector3>();

            positions = index9Sections[i].GetWaypoints("posZ");

            vehicle.transform.position = new Vector3(positions[0].x, 0.25f, positions[0].z);

            vehicle.SetCurrentSection(index9Sections[i].Row(), index9Sections[i].Col());

            vehicle.SetWaypoints(positions);

            //Stops the same section being used, (and spawning another vehicle on this one)
            index9Sections.RemoveAt(i);
        }
    }


    public List<Vector3> GetWaypoint(int _row, int _col, string _direction, int _vehicleID)
    {
        // depending on the facing of the vehicle, we check
        // the road section in that direction

        List<Vector3> newWaypoints = new List<Vector3>();

        switch (_direction)
        {
            case "posX":
                newWaypoints = AssignWaypoints(_row, _col, _row, _col += 1, _direction, _vehicleID);
                return newWaypoints;

            case "negX":
                newWaypoints = AssignWaypoints(_row, _col, _row, _col -= 1, _direction, _vehicleID);
                return newWaypoints;

            case "posZ":
                newWaypoints = AssignWaypoints(_row, _col, _row += 1, _col, _direction, _vehicleID);
                return newWaypoints;

            case "negZ":
                newWaypoints = AssignWaypoints(_row, _col, _row -= 1, _col, _direction, _vehicleID);
                return newWaypoints;

            default:
                Debug.Log("Error, incorrect direction");
                newWaypoints.Add(Vector3.zero);
                return newWaypoints;
        }
    }


    // Current ie(where the vehicle currently is)
    // row & col are where we need to check
    private List<Vector3> AssignWaypoints(int _currentRow, int _currentCol, int _row, int _col,
        string _direction, int _vehicleID)
    {
        List<Vector3> positions = new List<Vector3>();

        // if the next section is an intersection
        if(roadNetwork[_row, _col].Index() != 6 && roadNetwork[_row, _col].Index() != 9)
        {
            bool sectionFound = false;

            while (!sectionFound)
            {
                // Pick a random neighbor (as it might turn)
                int i = Random.Range(0, roadNetwork[_row, _col].GetNeighbours().Count);

                int newRow = roadNetwork[_row, _col].GetNeighbours()[i].Row();
                int newCol = roadNetwork[_row, _col].GetNeighbours()[i].Col();

                // If this is a different tile (Not the Current Tile)
                if(newRow != _currentRow || newCol != _currentCol)
                {
                    // We should move here!

                    _direction = UpdateDirection(_direction, _currentRow, _currentCol, newRow, newCol);

                    vehicles[_vehicleID].UpdateData(_direction, newRow, newCol);

                    positions = roadNetwork[newRow, newCol].GetWaypoints(_direction);

                    sectionFound = true;
                }
            }
            // Update Direction

            return positions;
        }

        vehicles[_vehicleID].UpdateData(_direction, _row, _col);

        positions = roadNetwork[_row, _col].GetWaypoints(_direction);

        return positions;
    }


    private string UpdateDirection(string _direction, int _currentRow, int _currentCol, int _newRow, int _newCol)
    {
        // If we are moving right
        if (_direction == "posX")
        {
            // Going straight across Junction
            if (_newRow == _currentRow)
                return _direction;

            // turning Left at Junction
            if (_newRow > _currentRow)
                return "posZ";

            // Turning Right at junction
            if (_newRow < _currentRow)
                return "negZ";
        }

        // If we are moving left
        if (_direction == "negX")
        {
            // Going straight across Junction
            if (_newRow == _currentRow)
                return _direction;

            // turning Left at Junction
            if (_newRow > _currentRow)
                return "posZ";

            // Turning Right at junction
            if (_newRow < _currentRow)
                return "negZ";
        }

        // If we are moving Up
        if (_direction == "posZ")
        {
            // Going straight across Junction
            if (_currentRow == _newRow)
                return _direction;

            // turning Left at Junction
            if (_newCol < _currentCol)
                return "negX";

            // Turning Right at junction
            if (_newCol > _currentCol)
                return "posX";
        }

        // If we are moving down
        if (_direction == "negZ")
        {
            // Going straight across Junction
            if (_currentRow == _newRow)
                return _direction;

            // turning Left at Junction
            if (_newCol < _currentCol)
                return "negX";

            // Turning Right at junction
            if (_newCol > _currentCol)
                return "posX";
        }

        return _direction;
    }
}
