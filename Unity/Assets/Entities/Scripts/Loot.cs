using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    
    [Tooltip("The items to loot.")]
    public GameObject[] lootPrefabs;
    [Tooltip("Their probabilities")] [Range(0,1)]
    public float[] lootProba;
    [Tooltip("The max quantity of each item.")]
    public float[] lootMaxQuantity;
    [Tooltip("The height at which they need to spawn")]
    public float lootYOffset;
    [Tooltip("The radius of the zone in which they will spawn")]
    public float lootRange;
    // Start is called before the first frame update
    public void lootItems()
    {
        Debug.Log("loot !");
        float z = transform.position.z + lootYOffset;//ca sert a rien de le calculer a chaque item du coup je le met en dehors du for
        Quaternion rot = new Quaternion();
            
        for (int i = 0; i < lootPrefabs.Length; i++)
        {
            for (int j = 0; j < lootMaxQuantity[i]; j++)
            {
                float diceThrow = Random.Range(0f, 1f);
                if (diceThrow <= lootProba[i])
                {
                    float x = Random.Range(-lootRange, lootRange);
                    float y = Random.Range(-lootRange, lootRange);
                    Vector3 position = new Vector3(x, y, z);
                    Instantiate(lootPrefabs[i], transform.position+position, rot);
                }
            }
        }
    }
}
