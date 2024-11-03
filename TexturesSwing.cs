using UnityEngine;

public class TexturesSwing : MonoBehaviour
{
    public float scrollSpeedX = 0.5f; // �������� �������� �������� �� ��� X
    public float scrollSpeedZ = 0.0f; // �������� �������� �������� �� ��� Y

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedZ * Time.deltaTime;

        // ��������� �������� � ��������� ���������
        rend.material.mainTextureOffset = offset;
    }
}
