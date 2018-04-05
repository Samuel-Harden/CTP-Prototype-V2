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

    private int panelSize = 1;

    // the min size a building can be
    private int minBuildingSize = 2;

    // These will be used to generate the mesh for the building
    private List<bool> panelStates;
    private List<Vector3> panelPositions;
    private List<Vector3> panelRotations;

    public int posXOffset;
    public int posZOffset;

    public int mainBuildingWidth;
    public int mainBuildingLength;

    public int height = 2;

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


    public void CalculateBuildingData()
    {
        if (division > buildDepth)
        {
            // 5 side left, right, forward, back, up
            // for each side generate a2d array representing each panel
            // object gen will then generate each panel
            // finally all panels will be merged into one complete mesh

            panelPositions = new List<Vector3>();
            panelRotations = new List<Vector3>();
            panelStates = new List<bool>();

            GenerateBuildingMain();
        }
    }


    private void GenerateBuildingMain()
    {
        // Scale min building size to == half the size of the lot
        int baseBuildingSizeX = (int)(lotWidthUpdated - 1) / 2;
        int baseBuildingSizeZ = (int)(lotLengthUpdated - 1) / 2;

        // Size
        mainBuildingWidth  = Random.Range(baseBuildingSizeX, ((int)lotWidthUpdated));
        mainBuildingLength = Random.Range(baseBuildingSizeZ, ((int)lotLengthUpdated));

        //stops buildings being 1 wide or 1 length, and set it to min if too small
        if (mainBuildingWidth < minBuildingSize)
            mainBuildingWidth = minBuildingSize;

        if (mainBuildingLength < minBuildingSize)
            mainBuildingLength = minBuildingSize;

        // Set positional offset, so that not all buildings are central to their lot
        posXOffset = Random.Range(0, ((int)lotWidthUpdated - 1) - mainBuildingWidth);
        posZOffset = Random.Range(0, ((int)lotLengthUpdated - 1) - mainBuildingLength);

        GenerateZPanelList();
        GenerateXPanelList();
        GenerateRoofPanelList();
    }


    private void GenerateZPanelList()
    {
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < (int)lotWidthUpdated - 1; w++)
            {
                // Z panel Negative
                if (w >= posXOffset && w < (mainBuildingWidth + posXOffset))
                {
                    panelStates.Add(true);

                    panelPositions.Add(new Vector3(w + ((float)panelSize / 2),
                        h + ((float)panelSize / 2), posZOffset));

                    panelRotations.Add(new Vector3(90.0f, 180.0f, 0.0f));
                }

                else
                {
                    panelStates.Add(false);

                    panelPositions.Add(Vector3.zero);

                    panelRotations.Add(Vector3.zero);
                }

                // Z panel Positive
                if (w >= posXOffset && w < (mainBuildingWidth + posXOffset))
                {
                    panelStates.Add(true);

                    panelPositions.Add(new Vector3(w + ((float)panelSize / 2),
                        h + ((float)panelSize / 2), posZOffset + mainBuildingLength));

                    panelRotations.Add(new Vector3(90.0f, 0.0f, 0.0f));
                }

                else
                {
                    panelStates.Add(false);

                    panelPositions.Add(Vector3.zero);

                    panelRotations.Add(Vector3.zero);
                }
            }
        }
    }


    private void GenerateXPanelList()
    {
        for (int h = 0; h < height; h++)
        {
            for (int l = 0; l < (int)lotLengthUpdated - 1; l++)
            {
                // x panel negative
                if (l >= posZOffset && l < (mainBuildingLength + posZOffset))
                {
                    panelStates.Add(true);

                    panelPositions.Add(new Vector3( posXOffset,
                        h + ((float)panelSize / 2), l + ((float)panelSize / 2)));

                    panelRotations.Add(new Vector3(90.0f, 270.0f, 0.0f));
                }

                else
                {
                    panelStates.Add(false);

                    panelPositions.Add(Vector3.zero);

                    panelRotations.Add(Vector3.zero);
                }

                // x panel Positive
                if (l >= posZOffset && l < (mainBuildingLength + posZOffset))
                {
                    panelStates.Add(true);

                    panelPositions.Add(new Vector3( posXOffset + mainBuildingWidth,
                        h + ((float)panelSize / 2), l + ((float)panelSize / 2)));

                    panelRotations.Add(new Vector3(90.0f, 90.0f, 0.0f));
                }

                else
                {
                    panelStates.Add(false);

                    panelPositions.Add(Vector3.zero);

                    panelRotations.Add(Vector3.zero);
                }
            }
        }
    }


    private void GenerateRoofPanelList()
    {
        for (int l = 0; l < lotLengthUpdated - 1; l++)
        {
            for (int w = 0; w < lotWidthUpdated - 1; w++)
            {
                // roof panels
                if (l >= posZOffset && l < (mainBuildingLength + posZOffset) && w >= posXOffset && w < (mainBuildingWidth + posXOffset))
                {
                    panelStates.Add(true);

                    panelPositions.Add(new Vector3( w + ((float)panelSize / 2),
                        height, l + ((float)panelSize / 2)));

                    panelRotations.Add(new Vector3(0.0f, 0.0f, 0.0f));
                }

                else
                {
                    panelStates.Add(false);

                    panelPositions.Add(Vector3.zero);

                    panelRotations.Add(Vector3.zero);
                }
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


    public List<Vector3> GetPanelPositions()
    {
        return panelPositions;
    }


    public List<Vector3> GetPanelRotations()
    {
        return panelRotations;
    }


    public List<bool> GetPanelStates()
    {
        return panelStates;
    }


    public Vector3 GetPanelPosition(int _index)
    {
        return panelPositions[_index];
    }


    public Vector3 GetPanelRotation(int _index)
    {
        return panelRotations[_index];
    }


    public bool GetPanelState(int _index)
    {
        return panelStates[_index];
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
