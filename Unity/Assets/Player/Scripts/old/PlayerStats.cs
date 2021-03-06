﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Tooltip("Player maximum life amount.")][SerializeField]
    private int _maxHP = 50;

    [Tooltip("Invincibility HUD image.")] [SerializeField]
    private Image invincibleImage;
    
    [Tooltip("Player life amount.")][SerializeField]private int _HP;


    public int Life { 
        get => _HP;
        set => _HP = value;
    }

    private void Awake()
    {
        _HP = _maxHP;
    }

    public int MaxLife{get => _maxHP;}

    public Action<int, Vector3, ResourceType> hurt;
    public Action<int> heal;
    public Action<bool> die;
    public bool invincible = false;

    private void FixedUpdate()
    {
        if (invincible)
        {
            Debug.Log("Player can't die");
            invincibleImage.enabled = true;
        }
        else
        {
         //   Debug.Log("Player can die");
            //invincibleImage.enabled = false;
        }
    }


    public void SetDamage(int damage, Vector3 direction, ResourceType damageType = ResourceType.normal)
    {
        if (!invincible)
        {
            hurt?.Invoke(damage, direction, damageType);
            _HP -= damage;
            GameManager.Instance.HPLost(damage);
        }  
        if (_HP <= 0 && !invincible) isPlayerDead(true);
    }

    public void RestoreLife(int hp)
    {
        heal?.Invoke(hp);
        _HP = ( _HP+hp < _maxHP )?_HP+hp : _maxHP;
    }
    
    public void isPlayerDead(bool isDead)
    {
        die?.Invoke(isDead);
    }

    public void setDamage(RaycastHit hit, ParticleSystem damageEffect, float damage, ResourceType type)
    {
        SetDamage((int) damage, Vector3.forward ,type);
    }
}
