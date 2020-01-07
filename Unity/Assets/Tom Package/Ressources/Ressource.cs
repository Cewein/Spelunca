using UnityEngine;
using System;
public enum Resource {normal,fire,water,plant};
public class ResourceBlock : MonoBehaviour
{
    [SerializeField]
    private int quantity = 5;

    private Resource currentResource;

    private Action<bool> isPicking;
}

