using UnityEngine;

public class TexturesSwing : MonoBehaviour
{
    public float scrollSpeedX = 0.5f; // —корость движени€ текстуры по оси X
    public float scrollSpeedZ = 0.0f; // —корость движени€ текстуры по оси Y

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

        // ѕримен€ем смещение к основному материалу
        rend.material.mainTextureOffset = offset;
    }
}
