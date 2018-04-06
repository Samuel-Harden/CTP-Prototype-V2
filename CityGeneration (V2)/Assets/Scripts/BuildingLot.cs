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

    // The panels that make up the building
    private List<List<Panel>> buildingPanels;
    private List<Panel> posXPanels;
    private List<Panel> posZPanels;
    private List<Panel> negXPanels;
    private List<Panel> negZPanels;
    private List<Panel> posYPanels;

    private List<int> mutationDirections;

    private int posXOffset;
    private int posZOffset;

    private int mainBuildingWidth;
    private int mainBuildingLength;

    private int mainBuildingHeight;
    private int currentMutationHeight;

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
            buildingPanels = new List<List<Panel>>();

            // here is where the data for each face is made, once this is complete, panels
            // can be enabled to generate desired building shape

            // Get main building data first
            GenerateBuildingMain();

            Vector3 pos = transform.position + new Vector3(lotWidthUpdated - ((float)panelSize / 2),
                ((float)panelSize / 2), (float)panelSize);

            posXPanels = BuildingCreationKit.GeneratePosXList(((int)lotLengthUpdated - panelSize),
                mainBuildingHeight, pos);

            buildingPanels.Add(posXPanels);


            pos = transform.position + new Vector3((float)panelSize, ((float)panelSize / 2),
                lotLengthUpdated - ((float)panelSize / 2));

            posZPanels = BuildingCreationKit.GeneratePosZList(((int)lotWidthUpdated - panelSize),
                mainBuildingHeight, pos);

            buildingPanels.Add(posZPanels);


            pos = transform.position + new Vector3(((float)panelSize / 2), ((float)panelSize / 2),
                (float)panelSize);

            negXPanels = BuildingCreationKit.GenerateNegXList(((int)lotLengthUpdated - panelSize),
                mainBuildingHeight, pos);

            buildingPanels.Add(negXPanels);            


            pos = transform.position + new Vector3((float)panelSize, ((float)panelSize / 2),
                ((float)panelSize / 2));

            negZPanels = BuildingCreationKit.GenerateNegZList(((int)lotWidthUpdated - panelSize),
                mainBuildingHeight, pos);

            buildingPanels.Add(negZPanels);

            pos = transform.position + new Vector3((float)panelSize, mainBuildingHeight, (float)panelSize);

            posYPanels = BuildingCreationKit.GeneratePosYList(((int)lotWidthUpdated - panelSize),
                ((int)lotLengthUpdated - panelSize), pos);
            buildingPanels.Add(posYPanels);


            BuildingCreationKit.UpdateAllLists(posXPanels, negXPanels, posZPanels, negZPanels, posYPanels,
                ((int)lotLengthUpdated - 1), ((int)lotWidthUpdated - 1), mainBuildingHeight,
                mainBuildingLength, mainBuildingWidth, posXOffset, posZOffset);

            BuildingCreationKit.GenerateMutations(this);

            BuildingCreationKit.ClearUnusedPanels(buildingPanels);
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

        // Set Height
        mainBuildingHeight = mainBuildingLength + mainBuildingWidth * 2;
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


    public List<List<Panel>> GetBuildingPanelList()
    {
        return buildingPanels;
    }


    public List<Panel> GetBuildingPanels(int _index)
    {
        return buildingPanels[_index];
    }


    public Panel GetBuildingPanel(int _listIndex, int _index)
    {
        return buildingPanels[_listIndex][_index];
    }


    public float GetMainBuildingHeight()
    {
        return mainBuildingHeight;
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
