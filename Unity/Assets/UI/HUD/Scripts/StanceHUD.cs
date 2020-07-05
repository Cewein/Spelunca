using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StanceHUD : MonoBehaviour
{
    [SerializeField] private Sprite crouchIcon = null;
    [SerializeField] private Sprite standIcon = null;
    [SerializeField] private MinerController miner = null;

    private void LateUpdate()
    {
        Refresh(miner.IsCrouching);
    }

    private void Refresh(bool isCrouching)
    {
        GetComponent<Image>().sprite = isCrouching ? crouchIcon : standIcon;
    }
}
