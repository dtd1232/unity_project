using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    Mesh mesh;
    int[] indices;
    // Start is called before the first frame update
  
    public void UpdateMeshInfo(Vector3[] vertices, Color[] colors)
    {
        if (mesh == null)
        {

            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            // Create a mesh with a single point
            int num = vertices.Length;
            indices = new int[num];
            for (int i = 0; i < num; i++) { indices[i] = i; }

            // Set the mesh to the mesh filter
            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.SetIndices(indices, MeshTopology.Points, 0);

            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }
        else
        {
            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.RecalculateBounds();
        }
    }
}
