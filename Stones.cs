using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YG;

public class Stones : MonoBehaviour
{
    public GameObject[] stones;
    public Text counter;
    private int logsPerStone = 3;
    private static int stoneCount;
    private bool[] isMining;

    public GameObject logPrefab;
    public int numberOfLogs = 3;
    public float delayBetweenLogs = 0.1f;

    public delegate void PlayerAction(GameObject stone);
    public static event PlayerAction OnPlayerENDMine;
    public static event PlayerAction OnPlayerStartMine;
    public delegate void PlayerAnimAction();
    public static event PlayerAnimAction OnPlayerENDMineAnim;

    private void Start()
    {
        // Инициализация stoneCount из PlayerPrefs
        stoneCount = PlayerPrefs.GetInt("Stones", 0);
        isMining = new bool[stones.Length];
        counter.text = stoneCount.ToString();
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerMining += StartMining;
       // YandexGame.GetDataEvent += LoadData;
        OnPlayerENDMine += SpawnAndMoveLogs;
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerMining -= StartMining;
       // YandexGame.GetDataEvent -= LoadData;
        OnPlayerENDMine -= SpawnAndMoveLogs;
    }

    private void StartMining(GameObject stone)
    {
        int stoneIndex = System.Array.IndexOf(stones, stone);

        if (stoneIndex != -1 && !isMining[stoneIndex])
        {
            isMining[stoneIndex] = true;
            StartCoroutine(HideStoneAfterMine(stone, stoneIndex));
        }
    }

    private IEnumerator HideStoneAfterMine(GameObject stone, int index)
    {
        OnPlayerStartMine?.Invoke(stone);

        yield return new WaitForSeconds(5);
        OnPlayerENDMine?.Invoke(stone);
        stone.SetActive(false);

        stoneCount = PlayerPrefs.GetInt("Stones", 0);
        stoneCount += logsPerStone;
        PlayerPrefs.SetInt("Stones", stoneCount);
        PlayerPrefs.Save();

        //YandexGame.savesData.stones = stoneCount;
        //YandexGame.SaveProgress();
        counter.text = stoneCount.ToString();

        OnPlayerENDMineAnim?.Invoke();

        yield return new WaitForSeconds(15);
        stone.SetActive(true);
        isMining[index] = false;
    }

    private void SpawnAndMoveLogs(GameObject stone)
    {
        StartCoroutine(SpawnLogsCoroutine(stone.transform.position));
    }

    private IEnumerator SpawnLogsCoroutine(Vector3 spawnPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found");
            yield break;
        }

        Vector3 screenPoint = new Vector3(Screen.width, Screen.height, mainCamera.WorldToScreenPoint(spawnPosition).z);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenPoint);

        for (int i = 0; i < numberOfLogs; i++)
        {
            GameObject log = Instantiate(logPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(MoveLogToPosition(log, targetPosition));
            yield return new WaitForSeconds(delayBetweenLogs);
        }
    }

    private IEnumerator MoveLogToPosition(GameObject log, Vector3 targetPosition)
    {
        float duration = 1.0f;
        float elapsedTime = 0;
        Vector3 startPosition = log.transform.position;

        while (elapsedTime < duration)
        {
            log.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        log.transform.position = targetPosition;
        Destroy(log);
    }
}
