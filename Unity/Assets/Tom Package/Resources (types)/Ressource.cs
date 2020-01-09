using UnityEngine;
using System;

// NOTE : All this script is a draft !!!
public enum Resource {normal,fire,water,plant};
public class ResourceCollectible: MonoBehaviour
{
    [SerializeField]
    private int quantity = 5;

    private Resource resource;

    private Action<bool> isPicking;
}
