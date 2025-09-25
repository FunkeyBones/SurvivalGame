using UnityEngine;

public class GroundGenerator : PlaneGenerator
{
    [SerializeField] private float yLevel;
    
    [Header("terrain settings")]
    [SerializeField] private float terrainAmplitude;

    [SerializeField] private Texture2D map;
    
    private Vector3[] baseVertices;
    void Awake()
    {
        GenerateMesh(yLevel);
        baseVertices = mesh.vertices;
        GenerateTerrain();
    }
    
    void GenerateTerrain()
    {
        for (int i = 0; i < vertices.Length; ++i)
        {
            Vector3 vertex = baseVertices[i];

            if (map != null)
            {
                // Normalize x,z into [0,1] to sample the heightmap
                float u = Mathf.InverseLerp(mesh.bounds.min.x, mesh.bounds.max.x, vertex.x);
                float v = Mathf.InverseLerp(mesh.bounds.min.z, mesh.bounds.max.z, vertex.z);

                // Get grayscale value from the heightmap
                float height = map.GetPixelBilinear(u, v).grayscale;

                vertex.y = height * terrainAmplitude + yLevel;
            }
            else
            {
                // fallback if no heightmap is assigned
                vertex.y = 0f;
            }

            vertices[i] = vertex;
        }
        
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}

