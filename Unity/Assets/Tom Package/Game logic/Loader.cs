using UnityEngine;

[RequireComponent(typeof(GameManager), typeof(ResourcesStock))]

/// <summary>
///  This loader instantiate persistent singletons at each scenes beginning.
/// </summary>
public class Loader : MonoBehaviour
{
    private GameManager gameManager;
    private ResourcesStock resourcesStock;
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        resourcesStock = GetComponent<ResourcesStock>();
        
        if (GameManager.instance == null)Instantiate(gameManager);
        if (ResourcesStock.instance == null)Instantiate(resourcesStock);
    }
}