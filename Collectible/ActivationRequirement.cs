using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivationRequirement
{
    public string resourceName;
    public int amount;
}

[CreateAssetMenu(fileName = "NewActivationData", menuName = "Activation/ActivationData")]
public class ActivationData : ScriptableObject
{
    public List<ActivationRequirement> requirements;
    public int trees;
    public int stones;
    public int iron;
    // Новое поле для префаба
    public GameObject prefabToSpawn;
}