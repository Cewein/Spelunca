using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable 
{
    void Pickax(RaycastHit hit, float damage);

}
