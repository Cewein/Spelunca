/*
public class MenusManager : MonoBehaviour
{
    [Header("Menus")]
    [Tooltip("The resource magazine selection menu.")][SerializeField]
    private RingMenu resourceLoaderSelectionMenu;
    [Tooltip("The artefact selection menu.")][SerializeField]
    private RingMenu artefactSelectionMenu;

    [Header("Inputs")] 
    [Tooltip("The input to display resource magazine selection menu.")][SerializeField]
    [InputName]
    private string resourceLoaderSelectionInput = "ResourceSelection";
    [Tooltip("The input to display artefact selection menu.")][SerializeField]
    [InputName]
    private string artefactSelectionInput = "ArtefactSelection";

    [Header("Objects linked")]
    [Tooltip("The gun magazine.")] [SerializeField]
    private GunMagazine gunMagazine;
    public Resource[] list;
    private void Start()
    {
        resourceLoaderSelectionMenu.callback += (type,artefact) =>
        {
            gunMagazine.CurrentResource = list.First(item => item.Type == type);
        };

    }


    private void Update()
    {
        resourceLoaderSelectionMenu.SetActive(Input.GetButton(resourceLoaderSelectionInput) && !Input.GetButton(artefactSelectionInput));
        artefactSelectionMenu.SetActive(Input.GetButton(artefactSelectionInput) && !Input.GetButton(resourceLoaderSelectionInput));
        Cursor.lockState = (artefactSelectionMenu.isActiveAndEnabled || resourceLoaderSelectionMenu.isActiveAndEnabled) ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
*/