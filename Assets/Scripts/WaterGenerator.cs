using UnityEngine;

public class WaterGenerator : PlaneGenerator
{
    [Header("water settings")]
    [SerializeField] private float waveAmplitude;
    [SerializeField] private float waveFrequency;
    [SerializeField] private float waveSpeed;

    [SerializeField] private Transform player;
    
    private Vector3[] baseVertices;
    private float range;
    void Awake()
    {
        GenerateMesh();
        baseVertices = mesh.vertices;

        range = width / 2 * cellSize;
    }

    void Update()
    {
        if (player.position.x >= transform.position.x - range && player.position.x <= transform.position.x + range &&
            player.position.z >= transform.position.z - range && player.position.z <= transform.position.z + range)
        {
            AnimateWave();
        }
    }

    void AnimateWave()
    {
        for (int i = 0; i < vertices.Length; ++i)
        {
            Vector3 vertex = baseVertices[i];
            
            float noiseValueA = Mathf.PerlinNoise(
                vertex.x * waveFrequency + Time.time * waveSpeed,
                vertex.z * waveFrequency + Time.time * waveSpeed);
            
            float noiseValueB = Mathf.PerlinNoise(
                vertex.x * (waveFrequency / 2) + Time.time * -waveSpeed,
                vertex.z * (waveFrequency / 2) + Time.time * -waveSpeed);
            
            vertex.y += (noiseValueA + noiseValueB) * waveAmplitude;
            vertices[i] = vertex;
        }
        
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
