using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGen : MonoBehaviour
{
    [SerializeField] GameObject TrafficLightPrefab;
    [SerializeField] List<Mesh> roadMeshPrefabs;
    [SerializeField] List<Material> roadMaterials;
    [Space] [SerializeField] GameObject roadPrefab;
    [Space] [SerializeField] GameObject roadContainer;

    private int cityWidth;
    private int cityLength;
    private int roadSize;
    private float roadHeight;
    private bool roadsValid;

    private RoadSection[,] roadMap;
    private List<RoadSection> roadMapList;

    // Road Section Lookup Tables for Road Mesh
    Dictionary<int, int> roadMeshs = new Dictionary<int, int>()
    {
        {  3, 0 }, {  5, 1 }, { 6,  2 }, {  7, 3 }, { 9,   4 }, { 10, 5 },
        { 11, 6 }, { 12, 7 }, { 13, 8 }, { 14, 9 }, { 15, 10 }
    };


    Dictionary<string, int> roadMaterial = new Dictionary<string, int>()
    {
        {  "Index_3", 0 }, {  "Index_5", 1 }, { "Index_6",  2 }, {  "Index_7", 3 }, { "Index_9",   4 }, { "Index_10", 5 },
        { "Index_11", 6 }, { "Index_12", 7 }, { "Index_13", 8 }, { "Index_14", 9 }, { "Index_15", 10 }
    };

    public void Initialise(List<BuildingLot> _buildingLots, int _cityWidth,
        int _cityLength, int _roadSize)
    {
        roadSize   = _roadSize;
        cityWidth  = _cityWidth;
        cityLength = _cityLength;

        GenerateRoads(_buildingLots);

        roadHeight = roadMapList[0].GetComponent<MeshFilter>().mesh.bounds.extents.y;

        SetNeighbours();
    }


    public void ResetRoads()
    {
        foreach (RoadSection road in roadMapList)
        {
            Destroy(road.gameObject);
        }

        roadMapList.Clear();

        roadsValid = false;
    }


    public bool RoadsValid()
    {
        return roadsValid;
    }


    private void Awake()
    {
        roadMapList = new List<RoadSection>();
    }


    private void GenerateRoads(List<BuildingLot> _buildingLots)
    {
        ResetRoads();

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

        roadsValid = true;
    }


    private RoadSection CreateTile(int _posX, int _posZ)
    {
        var tile = Instantiate(roadPrefab, new Vector3(_posX, 0, _posZ),
            Quaternion.identity);

        tile.GetComponent<RoadSection>().SetData(_posX, _posZ);

        roadMapList.Add(tile.GetComponent<RoadSection>());

        return tile.GetComponent<RoadSection>();
    }


    private void SetNeighbours()
    {
        List<RoadSection> neighbours = new List<RoadSection>();

        for (int l = 0; l < cityLength; l++)
        {
            for (int w = 0;  w < cityWidth; w++)
            {
                neighbours.Clear();
            }
        }
    }


    private void AssignType(RoadSection _section)
    {
        List<RoadSection> neighbours = new List<RoadSection>();

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
                neighbours.Add(roadMap[(int)roadPos.z + roadSize, (int)roadPos.x]);
                up = true;
                index += 1;
            }
        }

        if (roadPos.z - roadSize >= 0)
        {
            //Check Down
            if (roadMap[(int)roadPos.z - roadSize, (int)roadPos.x] != null)
            {
                neighbours.Add(roadMap[(int)roadPos.z - roadSize, (int)roadPos.x]);
                down = true;
                index += 8;
            }
        }

        if (roadPos.x - roadSize >= 0)
        {
            //Check Left
            if (roadMap[(int)roadPos.z, (int)roadPos.x - roadSize] != null)
            {
                neighbours.Add(roadMap[(int)roadPos.z, (int)roadPos.x - roadSize]);
                left = true;
                index += 2;
            }
        }


        if (roadPos.x + roadSize <= cityWidth)
        //Check Right
        if (roadMap[(int)roadPos.z, (int)roadPos.x + roadSize] != null)
        {
            neighbours.Add(roadMap[(int)roadPos.z, (int)roadPos.x + roadSize]);
            right = true;
            index += 4;
        }

        _section.SetIndex(index);

        _section.GetComponent<MeshFilter>().mesh = roadMeshPrefabs[GetLookupValue(index)];

        Material[] _sectionMats = new Material[2];

        // 3DS Max flips the Mats after Index_7? so a quick fix
        if (index <= 7)
        {
            _sectionMats[0] = roadMaterials[0];
            _sectionMats[1] = roadMaterials[1];
        }

        else
        {
            _sectionMats[0] = roadMaterials[1];
            _sectionMats[1] = roadMaterials[0];
        }

        _section.GetComponent<Renderer>().sharedMaterials = _sectionMats;

        _section.gameObject.AddComponent<MeshCollider>();

        // Compensate for 3Ds Max's Rotations
        _section.transform.rotation = Quaternion.Euler(-90.0f, 180.0f, 0.0f);

        _section.transform.parent = roadContainer.transform;

        _section.SetNeighbours(neighbours);


        if (_section.Index() == 6 || _section.Index() == 9)
            _section.GetComponent<RoadSection>().SetWaypoints();

        else if (_section.GetNeighbours().Count > 2)
        {
            _section.gameObject.AddComponent<Intersection>();
            _section.GetComponent<Intersection>().Initialise(TrafficLightPrefab);
        }

    }


    public RoadSection[,] GetRoadNetwork()
    {
        return roadMap;
    }


    public List<RoadSection> GetRoadNetworkList()
    {
        return roadMapList;
    }


    public float RoadHeight()
    {
        return roadHeight;
    }


    private int GetLookupValue(int _value)
    {
        int value = 0;

        roadMeshs.TryGetValue(_value, out value);

        return value;
    }
}
