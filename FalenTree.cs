using UnityEngine;
using System.Collections;

public class FalenTree : MonoBehaviour
{
    public GameObject logPrefab; // ������ �����
    public int numberOfLogs = 5; // ���������� �����
    public float delayBetweenLogs = 0.1f; // �������� ����� �������

    private void OnEnable()
    {
        Trees.OnPlayerENDLumb += TreeFallen;
        Trees.OnPlayerStartLumb += StartAnim;
    }

    private void OnDisable()
    {
        Trees.OnPlayerENDLumb -= TreeFallen;
        Trees.OnPlayerStartLumb -= StartAnim;
    }
    void StartAnim(GameObject fallenTree)
    {
        if (fallenTree == gameObject)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("Shake");
        }
    }

    void TreeFallen(GameObject fallenTree)
    {
        if (fallenTree == gameObject)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("Fall");

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

        // ������� ������� ������ ���� ������
        Vector3 screenPoint = new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenPoint);

        for (int i = 0; i < numberOfLogs; i++)
        {
            // ������� ����� �� ������� ������
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