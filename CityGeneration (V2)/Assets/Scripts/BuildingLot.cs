using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLot : MonoBehaviour
{
    [SerializeField] int divideCount = 1;

    private float lotWidth;
    private float lotLength;

    private bool divided;

    private Vector3 boundsPos;

    private int noDivisions = 4;
    private int buildDepth = 2;
    private int counter;

    private Color myColor = Color.red;

    [SerializeField] bool gizmosEnabled;

    public void Initialise(Vector3 _pos, float _lotWidth, float _lotLength, int _maxDepth,
        GameObject _lotPrefab, int _tileSize, List<BuildingLot> _buildingLots,
        List<Vector3> _perlinPositions)
    {
        divided = false;
        lotWidth = _lotWidth;
        lotLength = _lotLength;

        if (_maxDepth > 0)
        {
            if (DivideCheck(_perlinPositions))
            {
                Divide(_perlinPositions, _maxDepth, _lotPrefab, _buildingLots, _tileSize);
            }
        }
    }


    private bool DivideCheck(List<Vector3> _perlinPositions)
    {
        counter = 0;

        foreach (Vector3 pos in _perlinPositions)
        {
            // is this position within bounds of node
            if (pos.x >= transform.position.x && pos.x < (transform.position.x + lotWidth) &&
                pos.z >= transform.position.z && pos.z < (transform.position.z + lotLength))
            {
                counter++;

                if(counter > divideCount)
                {
                    // if this lot has a pos in its bounds, we need to divide
                    divided = true;
                    return true;
                }
            }
        }

        // No positions found, we dont need to divide
        return false;
    }


    private void Divide(List<Vector3> _perlinPositions, int _maxDepth,
        GameObject _lotPrefab, List<BuildingLot> _buildingLots, int _tileSize)
    {
        _maxDepth -= 1;

        bool offsetWidth = false;
        bool offsetLength = false;
        bool sizeX = false;
        bool sizeZ = false;

        Vector3 newPos = transform.position;

        int count = 0;

        //List<BuildingLotData> lotDataPackages = GenerateLotData(_tileSize);

        // Check if Lot needs an Offset
        offsetWidth  = Offset(lotWidth, _tileSize);
        offsetLength = Offset(lotLength, _tileSize);

        // Order (Bottom left, Bottom right, Top left, Top right)
        for (int i = 0; i < noDivisions; i++)
        {
            float lotSizeX = lotWidth / 2;
            float lotSizeZ = lotLength / 2;

            if (offsetWidth)
            {
                if (sizeX)
                {
                    lotSizeX -= (float)_tileSize / 2;
                    sizeX = false;
                }

                else
                {
                    lotSizeX += (float)_tileSize / 2;
                    sizeX = true;
                }
            }

            if (offsetLength)
            {
                if (sizeZ)
                {
                    lotSizeZ -= (float)_tileSize / 2;
                }

                else
                {
                    lotSizeZ += (float)_tileSize / 2;
                }
            }

            GenerateLot(newPos, lotSizeX, lotSizeZ, _perlinPositions,
                _maxDepth, _lotPrefab, _buildingLots, _tileSize);

            if (offsetWidth && sizeX)
                newPos.x += lotWidth / 2 + (float)_tileSize / 2;

            else
                newPos.x += lotWidth / 2;

            count++;

            if (count > 1)
            {
                newPos.x = transform.position.x;

                if (offsetLength)
                    newPos.z += lotLength / 2 + (float)_tileSize / 2;

                else
                    newPos.z += lotLength / 2;

                sizeZ = true;

                count = 0;
            }
        }
    }


    private void GenerateLot(Vector3 _newPos, float _lotSizeX, float _lotSizeZ,
        List<Vector3> _perlinPositions, int _maxDepth, GameObject _lotPrefab,
        List<BuildingLot> _buildingLots, int _tileSize)
    {
        var buildingLot = Instantiate(_lotPrefab, _newPos, Quaternion.identity);

        _buildingLots.Add(buildingLot.GetComponent<BuildingLot>());

        buildingLot.GetComponent<BuildingLot>().Initialise(_newPos, _lotSizeX,
            _lotSizeZ, _maxDepth, _lotPrefab, _tileSize, _buildingLots, _perlinPositions);
    }


    private bool Offset(float _value, int _tileSize)
    {
        // check if offset is needed
        if ((_value / 2) % _tileSize != 0)
        {
            return true;
        }

        return false;
    }


    private List<BuildingLotData> GenerateLotData(int _tileSize)
    {
        List<BuildingLotData> dataPackages = new List<BuildingLotData>();

        //Split this area so that I have 4 new lots, all different sizes,
        // but will fit into a tilemap after (no remainders based on tileSize)

        for (int i = 0; i < noDivisions; i++)
        {
            BuildingLotData lotData = new BuildingLotData();

            dataPackages.Add(lotData);
        }

        // check if offset is needed
        if ((lotWidth / 2) % _tileSize == 0)
        {
            myColor = Color.green;
            Debug.Log("Perfect Fit");
            // All lots will be exactly the same size

            foreach (BuildingLotData lotData in dataPackages)
            {
                lotData.lotWidth = lotWidth / 2;
                lotData.lotLength = lotLength / 2;
            }
        }

        else
        {

            Debug.Log("Needs to Be Offset");
        }

        //int xCount = lotWidth / _tileSize;
        //int zCount = lotLength / _tileSize;

        return dataPackages;
    }


    public bool Divided()
    {
        return divided;
    }


    public float Width()
    {
        return lotWidth;
    }


    public float Length()
    {
        return lotLength;
    }


    private void OnDrawGizmos()
    {
        if (gizmosEnabled)
        {
            Gizmos.color = myColor;

            // Bottom Left to Bottom Right
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + lotWidth, 0, transform.position.z));

            // Bottom Right to Top Right
            Gizmos.DrawLine(new Vector3(transform.position.x + lotWidth, 0, transform.position.z), new Vector3(transform.position.x + lotWidth, 0, transform.position.z + lotLength));

            // Top Right to Top Left
            Gizmos.DrawLine(new Vector3(transform.position.x + lotWidth, 0, transform.position.z + lotLength), new Vector3(transform.position.x, 0, transform.position.z + lotLength));

            // Top Left to Bottom Left
            Gizmos.DrawLine(new Vector3(transform.position.x, 0, transform.position.z + lotLength), transform.position);
        }
    }
}
