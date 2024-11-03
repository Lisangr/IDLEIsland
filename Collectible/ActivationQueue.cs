using System.Collections.Generic;
using UnityEngine;

public class ActivationQueue : MonoBehaviour
{
    public List<ActivationDataHolder> activationList = new List<ActivationDataHolder>();
    private Queue<ActivationDataHolder> activationQueue = new Queue<ActivationDataHolder>();
    private ActivationDataHolder currentActivationDataHolder;

    private void Start()
    {
        foreach (var dataHolder in activationList)
        {
            activationQueue.Enqueue(dataHolder);
            dataHolder.gameObject.SetActive(false); // Изначально скрываем все объекты
        }

        ActivateNext(); // Активируем первый объект в очереди
    }

    public void CompleteCurrentActivation()
    {
        if (currentActivationDataHolder != null)
        {
            currentActivationDataHolder.gameObject.SetActive(false);
            currentActivationDataHolder = null;
            ActivateNext(); // Переход к следующему объекту
        }
    }

    private void ActivateNext()
    {
        if (activationQueue.Count > 0)
        {
            currentActivationDataHolder = activationQueue.Dequeue();
            currentActivationDataHolder.gameObject.SetActive(true);
        }
    }

    public ActivationDataHolder GetCurrentActivation()
    {
        return currentActivationDataHolder;
    }
}
