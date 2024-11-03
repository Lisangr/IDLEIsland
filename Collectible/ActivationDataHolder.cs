using TMPro;
using UnityEngine;

public class ActivationDataHolder : MonoBehaviour
{
    public ActivationData activationData;
    public TextMeshProUGUI stoneCountText;
    public TextMeshProUGUI treeCountText;
    public TextMeshProUGUI ironCountText;

    private void Start()
    {
        if (stoneCountText != null)
        stoneCountText.text = activationData.stones.ToString();
        
        if (treeCountText != null)
        treeCountText.text = activationData.trees.ToString();
        
        if(ironCountText != null)
        ironCountText.text = activationData.iron.ToString();
    }
}
