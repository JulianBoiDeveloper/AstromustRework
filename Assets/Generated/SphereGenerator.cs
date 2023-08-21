using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    [Tooltip("Number of subdivisions for the sphere")]
    public int subdivisions = 3;

    private MeshFilter meshFilter;
    

    private void OnValidate()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateSphere();
    }

    private void GenerateSphere()
    {
        Mesh mesh = new Mesh();

        int resolution = subdivisions * 2;

        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[resolution * resolution * 6];

        for (int i = 0, y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++, i++)
            {
                float u = (float)x / resolution;
                float v = (float)y / resolution;

                float theta = u * Mathf.PI * 2;
                float phi = v * Mathf.PI;

                float sinTheta = Mathf.Sin(theta);
                float sinPhi = Mathf.Sin(phi);
                float cosTheta = Mathf.Cos(theta);
                float cosPhi = Mathf.Cos(phi);

                float radius = 1f;

                float xPosition = radius * sinPhi * cosTheta;
                float yPosition = radius * cosPhi;
                float zPosition = radius * sinPhi * sinTheta;

                vertices[i] = new Vector3(xPosition, yPosition, zPosition);
                uv[i] = new Vector2(u, v);
            }
        }

        for (int ti = 0, vi = 0, y = 0; y < resolution; y++, vi++)
        {
            for (int x = 0; x < resolution; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + resolution + 1;
                triangles[ti + 5] = vi + resolution + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;
    }
}