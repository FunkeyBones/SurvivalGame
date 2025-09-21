using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    [Header("plane settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    
    [HideInInspector]public Vector3[] vertices;
    [HideInInspector] public Mesh mesh;
    private int[] triangles;
    private Vector2[] uvs;
    
    void Awake()
    {
        GenerateMesh();
    }

    public void GenerateMesh(float yOffset = 0)
    {
        mesh = new Mesh();
        
        GenerateVertices(yOffset);
        GenerateTriangles();
        GenerateUV();
        AssignMesh();
    }
    
    public void GenerateVertices(float yOffset = 0)
    {
        //make plane spawn at centre
        float offsetX = width * cellSize * 0.5f;
        float offsetZ = height * cellSize * 0.5f;
        
        vertices = new Vector3[(width + 1) * (height + 1)];

        for (int z = 0, i = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                vertices[i] = new Vector3(x * cellSize - offsetX, yOffset, z * cellSize - offsetZ);
            }
        }
    }

    public void GenerateTriangles()
    {
        triangles = new int[width * height * 6];
        for (int z = 0, vert = 0, tris = 0; z < height; z++, vert++)
        {
            for (int x = 0; x < width; x++, vert++, tris += 6)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
            }
        }
        
    }

    public void GenerateUV()
    {
        uvs = new Vector2[vertices.Length];

        for (int z = 0, i = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                // Each cell = 1 texture unit
                uvs[i] = new Vector2(x, z);
            }
        }
    }

    public void AssignMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
