using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RotateComponent : IComponentData
{
    public float radiansPerSecond;
}
