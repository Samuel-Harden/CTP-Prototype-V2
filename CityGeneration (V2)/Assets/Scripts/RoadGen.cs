using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGen : MonoBehaviour
{
    [SerializeField] List<Mesh> roadMeshPrefabs;
    [Space] [SerializeField] GameObject roadPrefab;
    [Space] [SerializeField] GameObject roadContainer;

    private int cityWidth;
    private int cityLength;
    private int roadSize;

    private RoadSection[,] roadMap;

    // Road Section Lookup Table
    Dictionary<int, int> roadMeshs = new Dictionary<int, int>()
    {
        {  3, 0 }, {  5, 1 }, { 6,  2 }, {  7, 3 }, { 9,   4 }, { 10, 5 },
        { 11, 6 }, { 12, 7 }, { 13, 8 }, { 14, 9 }, { 15, 10 }
    };

    public void Initialise(List<BuildingLot> _buildingLots, int _cityWidth,
        int _cityLength, int _roadSize)
    {
        roadSize   = _roadSize;
        cityWidth  = _cityWidth;
        cityLength = _cityLength;

        GenerateRoads(_buildingLots);
    }


    private void GenerateRoads(List<BuildingLot> _buildingLots)
    {
        roadMap = new RoadSection[cityLength + 1, cityWidth + 1];

        // Add each room pos to tile grid
        for (int i = 0; i < _buildingLots.Count; i++)
        {
            int posX = (int) _buildingLots[i].transform.position.x;
            int posZ = (int) _buildingLots[i].transform.position.z;

            // First, add in all rooms to tile map
            for (int l = 0; l <= _buildingLots[i].Length(); l += roadSize)
            {
                for (int w = 0; w <= _buildingLots[i].Width(); w += roadSize)
                {
                    // if this is an edge tile its a Road!
                    if (l == 0 || w == 0 || 
                        w == _buildingLots[i].Width() || l == _buildingLots[i].Length()) 
                    {
                        if (roadMap[posZ, posX] == null)
                            roadMap[posZ, posX] = CreateTile(posX, posZ);
                    }

                    posX += roadSize;
                }

                posX = (int) _buildingLots[i].transform.position.x;

                posZ += roadSize;
            }
        }

        foreach (RoadSection section in roadMap)
        {
            if(section != null)
                AssignType(section);
        }
    }


    private RoadSection CreateTile(float _posX, float _posZ)
    {
        var tile = Instantiate(roadPrefab, new Vector3(_posX, 0, _posZ),
            Quaternion.identity);

        return tile.GetComponent<RoadSection>();
    }


    private void AssignType(RoadSection _section)
    {
        Vector3 roadPos = _section.transform.position;
        int index = 0;

        bool up    = false;
        bool down  = false;
        bool left  = false;
        bool right = false;


        if (roadPos.z + roadSize <= cityLength)
        {
            //Check Up
            if (roadMap[(int)roadPos.z + roadSize, (int)roadPos.x] != null)
            {
                up = true;
                index += 1;
            }
        }

        if (roadPos.z - roadSize >= 0)
        {
            //Check Down
            if (roadMap[(int)roadPos.z - roadSize, (int)roadPos.x] != null)
            {
                down = true;
                index += 8;
            }
        }

        if (roadPos.x - roadSize >= 0)
        {
            //Check Left
            if (roadMap[(int)roadPos.z, (int)roadPos.x - roadSize] != null)
            {
                left = true;
                index += 2;
            }
        }


        if (roadPos.x + roadSize <= cityWidth)
        //Check Right
        if (roadMap[(int)roadPos.z, (int)roadPos.x + roadSize] != null)
        {
            right = true;
            index += 4;
        }

        _section.SetIndex(index);

        _section.GetComponent<MeshFilter>().mesh = roadMeshPrefabs[GetLookupValue(index)];
        _section.gameObject.AddComponent<MeshCollider>();

        _section.GetComponent<MeshRenderer>().material.color = Color.gray;

        // Compensate for 3Ds Max's Rotations
        _section.transform.rotation = Quaternion.Euler(-90.0f, 180.0f, 0.0f);

        _section.transform.parent = roadContainer.transform;
    }


    private int GetLookupValue(int _value)
    {
        int value = 0;
        roadMeshs.TryGetValue(_value, out value);
        return value;
    }
}
