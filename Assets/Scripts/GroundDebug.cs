using UnityEngine;

public class GroundDebug : PlaneGenerator
{
    [SerializeField] private float yLevel;
    
    [Header("terrain settings")]
    [SerializeField] private float terrainAmplitude;
    [SerializeField] private float terrainFrequency;
    
    private Vector3[] baseVertices;
    void Awake()
    {
        GenerateMesh(yLevel);
        baseVertices = mesh.vertices;
        //GenerateTerrain();
    }

    void Update()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int i = 0; i < vertices.Length; ++i)
        {
            Vector3 vertex = baseVertices[i];
            
            float noiseValue = 0f;
            float amplitude = terrainAmplitude;
            float frequency = terrainFrequency;
            
            for (int octave = 0; octave < 3; octave++)
            {
                noiseValue += Mathf.PerlinNoise(vertex.x * frequency, vertex.z * frequency) * amplitude;
                frequency *= 2f;
                amplitude *= 0.5f;
            }
            
            vertex.y += noiseValue * terrainAmplitude;
            vertices[i] = vertex;
        }
        
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
