using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Tooltip("Player maximum life amount.")][SerializeField]
    private float maxLife = 100;
    private float life;
    

    public float Life { get => life; }

    public float MaxLife{get => maxLife;}

    public Action<float, Vector3, ResourceType> hurt;
    public Action<float> heal;
    public Action<bool> die;

    private void Awake()
    {
        life = maxLife;
    }

    public void SetDamage(int damage, Vector3 direction, ResourceType damageType = ResourceType.normal)
    {
        hurt?.Invoke(damage, direction, damageType);
        life -= damage;
        if (life <= 0) isPlayerDie(true);
    }

    public void RestoreLife(int hp)
    {
        heal?.Invoke(hp);
        life += hp;
    }
    
    private void isPlayerDie(bool isDie)
    {
        die?.Invoke(isDie);
        life = 0;
    }
}
