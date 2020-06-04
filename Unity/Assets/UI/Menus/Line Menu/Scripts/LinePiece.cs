using UnityEngine;
using UnityEngine.UI;

public class LinePiece : MonoBehaviour
{
    [Tooltip("frame image")] [SerializeField]
    private Image frame = null;
    [Tooltip("icon placeholder image")] [SerializeField]
    public Image icon = null;
    [Tooltip("Text to display quantity")] [SerializeField]
    private Text quantity = null;

    public string Name;

    public void SetQuantity(int q)
    {
        quantity.text = q.ToString();
    }
}
