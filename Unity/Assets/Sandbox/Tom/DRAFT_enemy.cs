using System;
using UnityEngine;

public class DRAFT_enemy : MonoBehaviour, IDamageable, IPickable
{
    public void setDamage(RaycastHit hit, ParticleSystem damageEffect, float damage, ResourceType type)
    {
        ParticleSystem d = Instantiate(damageEffect, hit.point, Quaternion.identity);
        d.Play();
    }

    public void Pickax(RaycastHit hit, float damage)
    {
        Debug.Log("Take damage with pickaxe so it's normal resource type damage !");
    }
}
