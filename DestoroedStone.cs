using System.Collections;
using UnityEngine;

public class DestoroedStone : MonoBehaviour
{
    public GameObject logPrefab; // Префаб доски
    public int numberOfLogs = 5; // Количество досок
    public float delayBetweenLogs = 0.1f; // Задержка между досками

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
            // Запускаем корутину создания и перемещения досок
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

        // Находим верхний правый угол экрана с учетом позиции объекта
        Vector3 screenPoint = new Vector3(Screen.width, Screen.height, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenPoint);

        for (int i = 0; i < numberOfLogs; i++)
        {
            // Создаем доску на позиции камня
            GameObject log = Instantiate(logPrefab, transform.position, Quaternion.identity);

            // Запускаем анимацию перемещения доски к верхнему правому углу
            StartCoroutine(MoveLogToPosition(log, targetPosition));

            // Задержка перед созданием следующей доски
            yield return new WaitForSeconds(delayBetweenLogs);
        }
    }

    private IEnumerator MoveLogToPosition(GameObject log, Vector3 targetPosition)
    {
        float duration = 1.0f; // Длительность перемещения
        float elapsedTime = 0;

        Vector3 startPosition = log.transform.position;

        // Линейное перемещение доски
        while (elapsedTime < duration)
        {
            log.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Завершаем перемещение, устанавливаем позицию точно в целевую
        log.transform.position = targetPosition;
        Destroy(log);
    }
}
