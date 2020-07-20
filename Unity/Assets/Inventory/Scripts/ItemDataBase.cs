using System.Collections.Generic;
using UnityEngine;


public class ItemDataBase :  MonoBehaviour
{
    private static ItemDataBase _singleton;
    public static ItemDataBase Instance { get { return _singleton; } }

    public List<Consumable> consumables; 
    public List<Artifact> artifacts;
    public List<Resource> resources;

    private void Awake()
    {
        _singleton = this;
    }
}
