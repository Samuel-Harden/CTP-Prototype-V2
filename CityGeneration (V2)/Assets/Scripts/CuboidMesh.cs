using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboidMesh : MonoBehaviour
{
    public GameObject GenerateCuboid(GameObject _prefab, float _width, float _length, float _height)
    {
        var cuboid = Instantiate(_prefab, Vector3.zero, Quaternion.identity);

        MeshFilter filter_one = cuboid.gameObject.GetComponent<MeshFilter>();

        Mesh mesh_cuboid = filter_one.mesh;

        mesh_cuboid.Clear();

        #region Vertices
        Vector3 pos_0 = new Vector3(-_width * 0.5f, -_height * 0.5f, _length  * 0.5f);
        Vector3 pos_1 = new Vector3(_width  * 0.5f, -_height * 0.5f, _length  * 0.5f);
        Vector3 pos_2 = new Vector3(_width  * 0.5f, -_height * 0.5f, -_length * 0.5f);
        Vector3 pos_3 = new Vector3(-_width * 0.5f, -_height * 0.5f, -_length * 0.5f);

        Vector3 pos_4 = new Vector3(-_width * 0.5f, _height * 0.5f, _length  * 0.5f);
        Vector3 pos_5 = new Vector3(_width  * 0.5f, _height * 0.5f, _length  * 0.5f);
        Vector3 pos_6 = new Vector3(_width  * 0.5f, _height * 0.5f, -_length * 0.5f);
        Vector3 pos_7 = new Vector3(-_width * 0.5f, _height * 0.5f, -_length * 0.5f);


        Vector3[] vertices = new Vector3[]
        {
            // Bottom
            pos_0, pos_1, pos_2, pos_3,

            // Left
            pos_7, pos_4, pos_0, pos_3,

            // Front 
            pos_4, pos_5, pos_1, pos_0,

            // Back
            pos_6, pos_7, pos_3, pos_2,

            // Right
            pos_5, pos_6, pos_2, pos_1,

            // Top
            pos_7, pos_6, pos_5, pos_4,
        };
        #endregion


        #region UVs


        Vector2 _00 = new Vector2(0.0f, 0.0f);
        Vector2 _10 = new Vector2(1.0f, 0.0f);
        Vector2 _01 = new Vector2(0.0f, 1.0f);
        Vector2 _11 = new Vector2(1.0f, 1.0f);


        Vector2[] uvs = new Vector2[]
        {
            // Bottom
            _11, _01, _00, _10,

            // Left
            _11, _01, _00, _10,

            // Front
            _11, _01, _00, _10,

            // Back
            _11, _01, _00, _10,

            // Right
            _11, _01, _00, _10,

            // Top
            _11, _01, _00, _10,
        };
        #endregion


        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;


        Vector3[] normals = new Vector3[]
        {
	        // Bottom
	        down, down, down, down,
 
	        // Left
	        left, left, left, left,
 
	        // Front
	        front, front, front, front,
 
	        // Back
	        back, back, back, back,
 
	        // Right
	        right, right, right, right,
 
	        // Top
	        up, up, up, up
        };
        #endregion


        #region Triangles
        int[] triangles = new int[]
        {
            // Bottom
            3, 1, 0,
            3, 2, 1,

  	        // Left
	        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	        // Front
	        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	        // Back
	        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	        // Right
	        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	        // Top
	        3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
        };
        #endregion


        mesh_cuboid.vertices = vertices;
        mesh_cuboid.normals = normals;
        mesh_cuboid.uv = uvs;
        mesh_cuboid.triangles = triangles;
        mesh_cuboid.RecalculateBounds();

        return cuboid;
    }
}
