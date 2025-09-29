using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private GroundGenerator groundGenerator;

    [SerializeField] private GameObject[] Objects;
    [SerializeField] private float spawnChance = 0.1f; // 10% chance
    [SerializeField] private float heightThreshold = 0.5f; // grayscale threshold

    void Start()
    {
        groundGenerator = GetComponent<GroundGenerator>();
        Generate();
    }

    public void Generate()
    {
         Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Texture2D map = groundGenerator.map;
        
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                
                /* Get grayscale value from the heightmap
                float height = map.GetPixelBilinear(u, v).grayscale;

                vertex.y = height * terrainAmplitude + yLevel;*/
        
                // Normalize x,z into [0,1] to sample the heightmap
                float u = Mathf.InverseLerp(mesh.bounds.min.x, mesh.bounds.max.x, vertex.x);
                float v = Mathf.InverseLerp(mesh.bounds.min.z, mesh.bounds.max.z, vertex.z);
        
                float grayscale = map.GetPixelBilinear(u, v).grayscale;
        
                if (grayscale * groundGenerator.terrainAmplitude + groundGenerator.yLevel> heightThreshold && Random.value < spawnChance)
                {
                    GameObject prefab = Objects[Random.Range(0, Objects.Length)];
        
                    // Convert vertex position to world space
                    Vector3 worldPos = transform.TransformPoint(vertex);
        
                    // Get normal in world space
                    Vector3 normal = transform.TransformDirection(normals[i]);
        
                    // Align prefab's up to the normal
                    Quaternion alignToNormal = Quaternion.FromToRotation(Vector3.up, normal);
        
                    // Add random Y rotation
                    Quaternion randomYRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    Quaternion finalRot = alignToNormal * randomYRot;
        
                    Instantiate(prefab, worldPos, finalRot, transform);
                }
            }
    }
}