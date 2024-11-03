using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public string groundTag = "Ground";
    public Text stonesText;
    public Text treesText;
    public Text ironText;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isMoving;
    private Animator animator;
    private bool isLumbing = false;
    private bool isMining = false;
    private bool isGrounded = true;
    private bool isSwiming = false;
    private GameObject currentTree;
    private GameObject currentStone;

    private ActivationQueue activationQueue;

    public delegate void PlayerAction(GameObject obj);
    public static event PlayerAction OnPlayerLumbing;
    public static event PlayerAction OnPlayerMining;

    private void OnEnable()
    {
        Trees.OnPlayerENDLumbAnim += PlayerChangeAnimation;
        Stones.OnPlayerENDMineAnim += PlayerChangeAnimation;
    }

    private void OnDisable()
    {
        Trees.OnPlayerENDLumbAnim -= PlayerChangeAnimation;
        Stones.OnPlayerENDMineAnim -= PlayerChangeAnimation;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
        isMoving = false;

        activationQueue = FindObjectOfType<ActivationQueue>(); // Получаем ссылку на очередь активации
        UpdateResourceUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(groundTag))
            {
                targetPosition = hit.point;
                isMoving = true;
            }
        }

        if (isMoving)
            MoveToTargetPosition();
        else
            animator.SetTrigger(rb.velocity.magnitude > 0.2f ? "Run" : isLumbing ? "Lumb" : isMining ? "Mining" : "Idle");

        // Проверяем нажатие "F" для активации текущего объекта в очереди
        ActivationDataHolder currentHolder = activationQueue.GetCurrentActivation();
        if (currentHolder != null && Input.GetKeyDown(KeyCode.F))
        {
            TryActivate(currentHolder.activationData);
        }
    }

    private void MoveToTargetPosition()
    {
        if (isGrounded)
        {
            isSwiming = false;

            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = Vector3.Distance(transform.position, targetPosition) <= 0.1f ? Vector3.zero : direction * moveSpeed;

            if (rb.velocity == Vector3.zero) isMoving = false;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            animator.SetTrigger("Run");
        }
        else if (isSwiming)
        {
            isGrounded = false;

            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = Vector3.Distance(transform.position, targetPosition) <= 0.1f ? Vector3.zero : direction * moveSpeed;

            if (rb.velocity == Vector3.zero) isMoving = false;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            animator.SetTrigger("Swim");
        }
    }

    private void TryActivate(ActivationData activationData)
    {
        // Проверяем, что все требования выполнены
        if (activationData.requirements.TrueForAll(req => PlayerPrefs.GetInt(req.resourceName, 0) >= req.amount))
        {
            // Обновляем ресурсы в PlayerPrefs
            foreach (var req in activationData.requirements)
            {
                int currentAmount = PlayerPrefs.GetInt(req.resourceName, 0);
                int newAmount = currentAmount - req.amount;

                if (newAmount != currentAmount)
                {
                    PlayerPrefs.SetInt(req.resourceName, newAmount);
                    PlayerPrefs.Save();
                    Debug.Log($"{req.resourceName} updated to {newAmount}");
                }
            }

            // Создаем объект, если требуется
            if (activationData.prefabToSpawn != null)
            {
                Instantiate(activationData.prefabToSpawn, activationQueue.GetCurrentActivation().transform.position, Quaternion.identity);
            }

            // Завершаем текущую активацию через очередь
            activationQueue.CompleteCurrentActivation();
            UpdateResourceUI();
        }
    }

    private void InvokeLumbAction(GameObject tree)
    {
        isLumbing = true;
        currentTree = tree;
        OnPlayerLumbing?.Invoke(currentTree);
    }

    private void InvokeMineAction(GameObject stone)
    {
        isMining = true;
        currentStone = stone;
        OnPlayerMining?.Invoke(currentStone);
    }

    private void ResetMiningLumbingState(GameObject obj)
    {
        if (obj == currentTree)
            isLumbing = false;
        else if (obj == currentStone)
            isMining = false;
    }

    private void PlayerChangeAnimation() => isLumbing = isMining = false;

    private void UpdateResourceUI()
    {
        if (stonesText != null)
            stonesText.text = PlayerPrefs.GetInt("Stones").ToString();

        if (treesText != null)
            treesText.text = PlayerPrefs.GetInt("Trees").ToString();

        if (ironText != null)
            ironText.text = PlayerPrefs.GetInt("Irons").ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tree"))
            InvokeLumbAction(other.gameObject);
        else if (other.gameObject.CompareTag("Stones"))
            InvokeMineAction(other.gameObject);
        else if (other.gameObject.CompareTag(groundTag))
            isGrounded = true;
        else if (other.gameObject.CompareTag("Sea"))
            isSwiming = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tree") || other.gameObject.CompareTag("Stones"))
            ResetMiningLumbingState(other.gameObject);
        else if (other.gameObject.CompareTag(groundTag))
            isGrounded = false;
        else if (other.gameObject.CompareTag("Sea"))
            isSwiming = false;
    }
}














/*
using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public string groundTag = "Ground";

    public Text stonesText;
    public Text treesText;
    public Text ironText;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isMoving;
    private Animator animator;
    private bool isLumbing = false;
    private bool isMining = false;
    private bool isGrounded = true;
    private bool isSwiming = false;
    private GameObject currentTree;
    private GameObject currentStone;
    private ActivationDataHolder currentActivationDataHolder;

    public delegate void PlayerAction(GameObject obj);
    public static event PlayerAction OnPlayerLumbing;
    public static event PlayerAction OnPlayerMining;

    private void OnEnable()
    {
        Trees.OnPlayerENDLumbAnim += PlayerChangeAnimation;
        Stones.OnPlayerENDMineAnim += PlayerChangeAnimation;
    }

    private void OnDisable()
    {
        Trees.OnPlayerENDLumbAnim -= PlayerChangeAnimation;
        Stones.OnPlayerENDMineAnim -= PlayerChangeAnimation;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
        isMoving = false;

        UpdateResourceUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(groundTag))
            {
                targetPosition = hit.point;
                isMoving = true;
            }
        }

        if (isMoving)
            MoveToTargetPosition();
        else
            animator.SetTrigger(rb.velocity.magnitude > 0.2f ? "Run" : isLumbing ? "Lumb" : isMining ? "Mining" : "Idle");

        if (currentActivationDataHolder != null && Input.GetKeyDown(KeyCode.F))
            TryActivate(currentActivationDataHolder.activationData);
    }

    private void MoveToTargetPosition()
    {
        if (isGrounded)
        {
            isSwiming = false;

            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = Vector3.Distance(transform.position, targetPosition) <= 0.1f ? Vector3.zero : direction * moveSpeed;

            if (rb.velocity == Vector3.zero) isMoving = false;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            animator.SetTrigger("Run");
        }
        else if (isSwiming)
        {
            isGrounded = false;

            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = Vector3.Distance(transform.position, targetPosition) <= 0.1f ? Vector3.zero : direction * moveSpeed;

            if (rb.velocity == Vector3.zero) isMoving = false;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            animator.SetTrigger("Swim");

        }
    }

    private void TryActivate(ActivationData activationData)
    {
        // Проверяем, что все требования выполнены
        if (activationData.requirements.TrueForAll(req => PlayerPrefs.GetInt(req.resourceName, 0) >= req.amount))
        {
            // Обновляем ресурсы в PlayerPrefs
            foreach (var req in activationData.requirements)
            {
                int currentAmount = PlayerPrefs.GetInt(req.resourceName, 0);
                int newAmount = currentAmount - req.amount;

                // Проверяем изменение и обновляем значение
                if (newAmount != currentAmount)
                {
                    PlayerPrefs.SetInt(req.resourceName, newAmount);
                    PlayerPrefs.Save();
                    Debug.Log($"{req.resourceName} updated to {newAmount}");
                }
            }

            // Создаем объект, если требуется
            if (activationData.prefabToSpawn != null)
            {
                Instantiate(activationData.prefabToSpawn, currentActivationDataHolder.transform.position, Quaternion.identity);
            }

            // Деактивируем текущий элемент
            currentActivationDataHolder.gameObject.SetActive(false);
            currentActivationDataHolder = null;

            // Обновляем UI после обновления всех ресурсов
            UpdateResourceUI();
        }
    }
    private void InvokeLumbAction(GameObject tree)
    {
        isLumbing = true;
        currentTree = tree;
        OnPlayerLumbing?.Invoke(currentTree);
    }

    private void InvokeMineAction(GameObject stone)
    {
        isMining = true;
        currentStone = stone;
        OnPlayerMining?.Invoke(currentStone);
    }

    private void ResetMiningLumbingState(GameObject obj)
    {
        if (obj == currentTree)
            isLumbing = false;
        else if (obj == currentStone)
            isMining = false;
    }

    private void PlayerChangeAnimation() => isLumbing = isMining = false;

    private void UpdateResourceUI()
    {
        if (stonesText != null)
        {
            stonesText.text = PlayerPrefs.GetInt("Stones").ToString();
            //YandexGame.savesData.stones = PlayerPrefs.GetInt("Stones");
            //YandexGame.SaveProgress();
        }

        if (treesText != null)
        {
            treesText.text = PlayerPrefs.GetInt("Trees").ToString();
            //YandexGame.savesData.trees = PlayerPrefs.GetInt("Trees");
            //YandexGame.SaveProgress();
        }
        if (ironText != null)
        {
            ironText.text = PlayerPrefs.GetInt("Irons").ToString();
            //YandexGame.savesData.irons = PlayerPrefs.GetInt("Iron");
            //YandexGame.SaveProgress();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tree"))
            InvokeLumbAction(other.gameObject);
        else if (other.gameObject.CompareTag("Stones"))
            InvokeMineAction(other.gameObject);
        else if (other.gameObject.CompareTag(groundTag))
            isGrounded = true;
        else if (other.gameObject.CompareTag("Sea"))
            isSwiming = true;
        else
            currentActivationDataHolder = other.GetComponent<ActivationDataHolder>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tree") || other.gameObject.CompareTag("Stones"))
            ResetMiningLumbingState(other.gameObject);
        else if (other.gameObject.CompareTag(groundTag))
            isGrounded = false;
        else if (other.gameObject.CompareTag("Sea"))
            isSwiming = false;
        else if (other.GetComponent<ActivationDataHolder>() == currentActivationDataHolder)
            currentActivationDataHolder = null;

    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
            isGrounded = true;
        if (collision.gameObject.CompareTag("Sea"))
            isSwiming = true;
    }
}*/