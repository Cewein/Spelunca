using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Tooltip("Player maximum life amount.")][SerializeField]
    private int maxLife = 100;
    private int life;
    

    public int Life { get => life; }

    public int MaxLife{get => maxLife;}

    public Action<int, Vector3, ResourceType> hurt;
    public Action<int> heal;
    public Action<bool> die;
    private bool invincible = false;
    private void Awake()
    {
        life = maxLife;
    }

    public void SetDamage(int damage, Vector3 direction, ResourceType damageType = ResourceType.normal)
    {
        if (life - damage <= 0)
        {
            if (!invincible)
            {
                isPlayerDie(true);
            }
            else if (invincible) return;
            
        }
        hurt?.Invoke(damage, direction, damageType);
        life -= damage;
        if (life <= 0) isPlayerDie(true);
    }

    public void RestoreLife(int hp)
    {
        heal?.Invoke(hp);
        life += hp;
    }
    
    public void isPlayerDie(bool isDie)
    {
        die?.Invoke(isDie);
        life = 0;
    }

    public void setDamage(RaycastHit hit, ParticleSystem damageEffect, float damage, ResourceType type)
    {
        SetDamage((int) damage, Vector3.forward ,type);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            invincible = !invincible;
        }
    }
}
