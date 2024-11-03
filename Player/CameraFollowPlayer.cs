using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // ���������� ��� �������� ������
    public Transform player;

    // �������� ������ ������������ ������ (�� ��������� ������� -7, 9, 3 ������������ ������)
    private Vector3 offset = new Vector3(-7, 9, 3);

    // ������������� ���� ������� ������
    private Vector3 fixedRotation = new Vector3(46, 166, 0);

    void Start()
    {
        // ������������� ��������� ���� ������� ������
        transform.eulerAngles = fixedRotation;
    }

    void LateUpdate()
    {
        // ���� ����� �� ������, �� ��������� ����������
        if (player == null) return;

        // ��������� ������� ������ ������������ ������� ������ � ������ ��������
        transform.position = player.position + offset;

        // ���������� ������ �� ������
        transform.LookAt(player.position);

        // ��������� ������������� ���� ������� ������
        transform.eulerAngles = new Vector3(fixedRotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
