using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SineWaveDeformer : MonoBehaviour
{
    public float amplitude = 0.5f;      // Амплитуда волны
    public float wavelength = 2f;       // Длина волны
    public float waveSpeed = 1f;        // Скорость движения волны
    public float period = 1f;           // Период изменения длины волны

    private Mesh mesh;
    private Vector3[] originalVertices;
    private float offset;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
    }

    void Update()
    {
        // Изменяем offset по времени
        offset += Time.deltaTime * waveSpeed;

        // Масштабируем длину волны с учетом периода
        float currentWavelength = wavelength + Mathf.Sin(Time.time * period) * (wavelength / 2);

        Vector3[] deformedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            // Применяем смещение по оси Y, для получения синусоидальной волны
            vertex.y += Mathf.Sin((vertex.x + offset) * Mathf.PI * 2 / currentWavelength) * amplitude;

            deformedVertices[i] = vertex;
        }

        // Обновляем вершины сетки
        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals(); // Обновляем нормали для корректного отображения света
        mesh.RecalculateBounds();
    }
}
