using System;
using UnityEngine;

public class DRAFT_enemy : MonoBehaviour, IDamageable
{
    public void setDamage(RaycastHit hit, ParticleSystem damageEffect)
    {
        ParticleSystem d = Instantiate(damageEffect, hit.point, Quaternion.identity);
        d.Play();
    }
}
