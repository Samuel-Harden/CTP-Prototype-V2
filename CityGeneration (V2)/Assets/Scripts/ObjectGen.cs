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
    private CuboidMesh cuboidMesh;

    private int minBuildDepth;
    private float roadHeight;

    private int panelSize = 1;


    public void Initialze(int _minBuildDepth, float _roadHeight)
    {
        planeMesh = GetComponent<PlaneMesh>();
        cuboidMesh = GetComponent<CuboidMesh>();

        minBuildDepth = _minBuildDepth;

        roadHeight = 0.07f;
    }


    public void GenerateLots(List<BuildingLot> _lots)
    {
        GenerateBuildings(_lots);
    }


    // loop through each side of each building creating each panel
    private void GenerateBuildings(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
        {
            // Build Something
            if (lot.Division() > minBuildDepth)
            {
                var buildingRoot = Instantiate(baseBuildingObj, Vector3.zero, Quaternion.identity);

                Vector3 newPos = lot.transform.position;

                newPos.x += lot.WidthUpdated() / 2;
                newPos.z += lot.LengthUpdated() / 2;

                newPos.y = lot.height / 2 + 0.07f; // 0.07f (Offset for road Mesh)

                buildingRoot.transform.position = newPos;

                // loop through panels
                for (int i = 0; i < lot.GetPanelPositions().Count; i++)
                {
                    if (lot.GetPanelState(i) == true)
                    {
                        var panel = planeMesh.GeneratePlane(planeMeshPrefab, panelSize, panelSize);

                        panel.transform.eulerAngles = lot.GetPanelRotation(i);

                        Vector3 pos = lot.transform.position;

                        pos.x += lot.GetPanelPosition(i).x + (float)panelSize / 2;
                        pos.y += lot.GetPanelPosition(i).y;
                        pos.z += lot.GetPanelPosition(i).z + (float)panelSize / 2;

                        panel.transform.position = pos;

                        SetTexture(panel);

                        panel.transform.parent = buildingRoot.transform;
                    }
                }

                // Merge components into parents mesh
                buildingRoot.GetComponent<MeshCombine>().CombineMeshes();

                SetTexture(buildingRoot);

                buildingRoot.transform.parent = buildingContainer.transform;
            }
        }
    }


    public void GeneratePaths(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
        {
            var path = planeMesh.GeneratePlane(planeMeshPrefab, lot.WidthUpdated(), lot.LengthUpdated());

            Vector3 pos = lot.transform.position;

            pos.x += lot.WidthUpdated() / 2;
            pos.z += lot.LengthUpdated() / 2;
            pos.y = roadHeight;

            path.transform.position = pos;

            if(lot.Division() > minBuildDepth)
            {
                path.GetComponent<Renderer>().material = pathMat;
            }

            else
                path.GetComponent<Renderer>().material = parkMat;

            path.transform.parent = pavementContainer.transform;
        }
    }


    private void SetTexture(GameObject _building)
    {
        int roof = Random.Range(0, roofTextures.Count);
        int wall = Random.Range(0, wallTextures.Count);

        _building.GetComponent<Renderer>().material.SetTexture("_RoofTex", roofTextures[roof]);
        _building.GetComponent<Renderer>().material.SetTexture("_WallTex", wallTextures[wall]);
    }
}
