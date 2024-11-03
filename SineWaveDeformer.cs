using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SineWaveDeformer : MonoBehaviour
{
    public float amplitude = 0.5f;      // ��������� �����
    public float wavelength = 2f;       // ����� �����
    public float waveSpeed = 1f;        // �������� �������� �����
    public float period = 1f;           // ������ ��������� ����� �����

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
        // �������� offset �� �������
        offset += Time.deltaTime * waveSpeed;

        // ������������ ����� ����� � ������ �������
        float currentWavelength = wavelength + Mathf.Sin(Time.time * period) * (wavelength / 2);

        Vector3[] deformedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            // ��������� �������� �� ��� Y, ��� ��������� �������������� �����
            vertex.y += Mathf.Sin((vertex.x + offset) * Mathf.PI * 2 / currentWavelength) * amplitude;

            deformedVertices[i] = vertex;
        }

        // ��������� ������� �����
        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals(); // ��������� ������� ��� ����������� ����������� �����
        mesh.RecalculateBounds();
    }
}
