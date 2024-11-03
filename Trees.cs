using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YG;

public class Trees : MonoBehaviour
{
    public GameObject[] trees;
    public Text counter;
    private int logsPerTree = 5;
    private static int treesCount;
    private bool[] isLumbering;

    public delegate void PlayerAction(GameObject tree);
    public static event PlayerAction OnPlayerENDLumb;
    public static event PlayerAction OnPlayerStartLumb;
    public delegate void PlayerAnimAction();
    public static event PlayerAnimAction OnPlayerENDLumbAnim;

    private void Start()
    {
        // Инициализация treesCount из PlayerPrefs при старте игры
        treesCount = PlayerPrefs.GetInt("Trees", 0);
        isLumbering = new bool[trees.Length];
        counter.text = treesCount.ToString();
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerLumbing += StartLumbering;
        //YandexGame.GetDataEvent += LoadData;
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerLumbing -= StartLumbering;
        //YandexGame.GetDataEvent -= LoadData;
    }
    
    private void StartLumbering(GameObject tree)
    {
        int treeIndex = System.Array.IndexOf(trees, tree);

        if (treeIndex != -1 && !isLumbering[treeIndex])
        {
            isLumbering[treeIndex] = true;
            StartCoroutine(HideTreeAfterLumb(tree, treeIndex));
        }
    }

    private IEnumerator HideTreeAfterLumb(GameObject tree, int index)
    {
        OnPlayerStartLumb?.Invoke(tree);

        yield return new WaitForSeconds(5);
        OnPlayerENDLumb?.Invoke(tree);

        yield return new WaitForSeconds(1.505f);
        tree.SetActive(false);

        treesCount = PlayerPrefs.GetInt("Trees", 0);
        treesCount += logsPerTree;
        PlayerPrefs.SetInt("Trees", treesCount);
        PlayerPrefs.Save();

        //YandexGame.savesData.trees = treesCount;
        //YandexGame.SaveProgress();
        counter.text = treesCount.ToString();

        OnPlayerENDLumbAnim?.Invoke();

        yield return new WaitForSeconds(15);
        tree.transform.rotation = Quaternion.identity;
        tree.SetActive(true);
        isLumbering[index] = false;
    }
}
