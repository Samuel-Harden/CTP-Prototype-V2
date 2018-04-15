using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGen : MonoBehaviour
{
    [SerializeField] Material pathMat;
    [SerializeField] Material parkMat;

    [SerializeField] List<Texture> roofTextures;
    [SerializeField] List<Texture> wallTextures;
    [SerializeField] List<Texture> slantTextures;

    [SerializeField] GameObject planeMeshPrefab;
    [SerializeField] GameObject cuboidMeshPrefab;

    [SerializeField] GameObject pavementContainer;
    [SerializeField] GameObject buildingContainer;

    [SerializeField] GameObject baseBuildingObj;

    private PlaneMesh planeMesh;

    private int minBuildDepth;
    private float roadHeight;

    private int panelSize = 1;

    private List<GameObject> paths;
    private List<GameObject> buildings;


    public void Initialze(int _minBuildDepth, float _roadHeight)
    {
        minBuildDepth = _minBuildDepth;

        roadHeight = 0.07f;
    }


    // loop through each side of each building creating each panel
    public void GenerateBuildings(List<BuildingLot> _lots)
    {
        ClearList(buildings);

        foreach (BuildingLot lot in _lots)
        {
            // Build Something
            if (lot.Division() > minBuildDepth)
            {
                var buildingRoot = Instantiate(baseBuildingObj, Vector3.zero, Quaternion.identity);

                Vector3 newPos = lot.transform.position;

                newPos.x += lot.GetLotWidth() / 2;
                newPos.z += lot.GetLotLength() / 2;

                newPos.y += (lot.GetMaxBuildingHeight() / 2);

                buildingRoot.transform.position = newPos;

                // loop through panels for each face of building
                for (int i = 0; i < lot.GetBuildingPanels().Count; i++)
                {
                    if (lot.GetBuildingPanel(i) != null)
                    {
                        var panel = planeMesh.GeneratePlane(planeMeshPrefab, panelSize, panelSize);

                        panel.transform.eulerAngles = lot.GetBuildingPanel(i).Rotation();

                        panel.transform.position = lot.GetBuildingPanel(i).Position();

                        SetTexture(panel);

                        panel.transform.parent = buildingRoot.transform;
                    }
                }

                // Merge components into parents mesh
                buildingRoot.GetComponent<MeshCombine>().CombineMeshes();

                // now all panels have been merged, we can apply a texture to the mesh
                SetTexture(buildingRoot);

                newPos.y += roadHeight;

                buildingRoot.transform.position = newPos;

                buildingRoot.transform.parent = buildingContainer.transform;

                buildings.Add(buildingRoot);
            }
        }
    }


    public void GeneratePaths(List<BuildingLot> _lots)
    {
        ClearList(paths);

        foreach (BuildingLot lot in _lots)
        {
            var path = planeMesh.GeneratePlane(planeMeshPrefab, lot.GetLotWidth(), lot.GetLotLength());

            Vector3 pos = lot.transform.position;

            pos.x += lot.GetLotWidth() / 2;
            pos.z += lot.GetLotLength() / 2;
            pos.y = roadHeight;

            path.transform.position = pos;

            if(lot.Division() > minBuildDepth)
            {
                path.GetComponent<Renderer>().material = pathMat;
            }

            else
                path.GetComponent<Renderer>().material = parkMat;

            path.transform.parent = pavementContainer.transform;

            paths.Add(path);
        }
    }

    private void Awake()
    {
        paths = new List<GameObject>();

        buildings = new List<GameObject>();

        planeMesh = GetComponent<PlaneMesh>();
    }


    private void ClearList(List<GameObject> _list)
    {
        foreach (GameObject obj in _list)
        {
            Destroy(obj);
        }

        _list.Clear();
    }


    private void SetTexture(GameObject _building)
    {
        int roof = Random.Range(0, roofTextures.Count);
        int wall = Random.Range(0, wallTextures.Count);

        _building.GetComponent<Renderer>().material.SetTexture("_RoofTex", roofTextures[roof]);
        _building.GetComponent<Renderer>().material.SetTexture("_WallTex", wallTextures[wall]);
    }
}
