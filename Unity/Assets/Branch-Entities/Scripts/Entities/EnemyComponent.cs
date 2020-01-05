using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public float startHP = 10f;

    private float HP;
    // Start is called before the first frame update
    void Start()
    {
        HP = startHP;
    }

    private void Update()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void hit(float damages)
    {
        HP -= damages;
    }
}
