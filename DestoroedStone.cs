using System.Collections;
using UnityEngine;

public class DestoroedStone : MonoBehaviour
{
    public GameObject logPrefab; // ������ �����
    public int numberOfLogs = 5; // ���������� �����
    public float delayBetweenLogs = 0.1f; // �������� ����� �������

    private void OnEnable()
    {
        Stones.OnPlayerENDMine += TreeFallen;
    }

    private void OnDisable()
    {
        Stones.OnPlayerENDMine -= TreeFallen;
    }

    void TreeFallen(GameObject fallenStone)
    {
        if (fallenStone == gameObject)
        {
            // ��������� �������� �������� � ����������� �����
            StartCoroutine(SpawnAndMoveLogs());
        }
    }

    private IEnumerator SpawnAndMoveLogs()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found");
            yield break;
        }

        // ������� ������� ������ ���� ������ � ������ ������� �������
        Vector3 screenPoint = new Vector3(Screen.width, Screen.height, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenPoint);

        for (int i = 0; i < numberOfLogs; i++)
        {
            // ������� ����� �� ������� �����
            GameObject log = Instantiate(logPrefab, transform.position, Quaternion.identity);

            // ��������� �������� ����������� ����� � �������� ������� ����
            StartCoroutine(MoveLogToPosition(log, targetPosition));

            // �������� ����� ��������� ��������� �����
            yield return new WaitForSeconds(delayBetweenLogs);
        }
    }

    private IEnumerator MoveLogToPosition(GameObject log, Vector3 targetPosition)
    {
        float duration = 1.0f; // ������������ �����������
        float elapsedTime = 0;

        Vector3 startPosition = log.transform.position;

        // �������� ����������� �����
        while (elapsedTime < duration)
        {
            log.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��������� �����������, ������������� ������� ����� � �������
        log.transform.position = targetPosition;
        Destroy(log);
    }
}
