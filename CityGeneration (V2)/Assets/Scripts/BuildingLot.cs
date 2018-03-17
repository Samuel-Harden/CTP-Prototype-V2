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

    private int tileSize;

    private float lotWidthUpdated;
    private float lotLengthUpdated;

    private int division;

    private Color myColor = Color.red;

    [SerializeField] bool gizmosEnabled;

    public void Initialise(Vector3 _pos, float _lotWidth, float _lotLength, int _maxDepth,
        GameObject _lotPrefab, int _tileSize, List<BuildingLot> _buildingLots,
        List<Vector3> _perlinPositions, int _division)
    {
        divided = false;
        lotWidth = _lotWidth;
        lotLength = _lotLength;
        tileSize = _tileSize;

        division = _division;

        if (_maxDepth > 0)
        {
            if (DivideCheck(_perlinPositions))
            {
                _division++;
                Divide(_perlinPositions, _maxDepth, _lotPrefab, _buildingLots, _tileSize, _division);
            }
        }
    }

    // This recalculates the lot size and factors in the roads
    public void RecalculateLotSize()
    {
        Vector3 updatedPos = transform.position;

        updatedPos.x += (float)tileSize / 2;
        updatedPos.z += (float)tileSize / 2;

        transform.position = updatedPos;

        lotLengthUpdated = lotLength - tileSize;
        lotWidthUpdated  = lotWidth  - tileSize;
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
        GameObject _lotPrefab, List<BuildingLot> _buildingLots, int _tileSize, int _division)
    {
        _maxDepth -= 1;

        bool offsetWidth = false;
        bool offsetLength = false;
        bool sizeX = false;
        bool sizeZ = false;

        Vector3 newPos = transform.position;

        int count = 0;

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
                _maxDepth, _lotPrefab, _buildingLots, _tileSize, _division);

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
        List<BuildingLot> _buildingLots, int _tileSize, int _division)
    {
        var buildingLot = Instantiate(_lotPrefab, _newPos, Quaternion.identity);

        _buildingLots.Add(buildingLot.GetComponent<BuildingLot>());

        buildingLot.GetComponent<BuildingLot>().Initialise(_newPos, _lotSizeX,
            _lotSizeZ, _maxDepth, _lotPrefab, _tileSize, _buildingLots, _perlinPositions, _division);
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

    public float LengthUpdated()
    {
        return lotLengthUpdated;
    }


    public float WidthUpdated()
    {
        return lotWidthUpdated;
    }


    public int Division()
    {
        return division;
    }


    private void OnDrawGizmos()
    {
        if (gizmosEnabled)
        {
            Gizmos.color = myColor;

            // Bottom Left to Bottom Right
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + lotWidthUpdated, 0, transform.position.z));

            // Bottom Right to Top Right
            Gizmos.DrawLine(new Vector3(transform.position.x + lotWidthUpdated, 0, transform.position.z), new Vector3(transform.position.x + lotWidthUpdated, 0, transform.position.z + lotLengthUpdated));

            // Top Right to Top Left
            Gizmos.DrawLine(new Vector3(transform.position.x + lotWidthUpdated, 0, transform.position.z + lotLengthUpdated), new Vector3(transform.position.x, 0, transform.position.z + lotLengthUpdated));

            // Top Left to Bottom Left
            Gizmos.DrawLine(new Vector3(transform.position.x, 0, transform.position.z + lotLengthUpdated), transform.position);
        }
    }
}
