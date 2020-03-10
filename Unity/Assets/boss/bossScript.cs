using UnityEngine;

public class bossScript : MonoBehaviour, IDamageable
{
    public float life = 5;
    public bool isAlive = true;

    void Update()
    {
        if (life <= 0)
            isAlive = false;
    }

    public void setDamage(RaycastHit hit, ParticleSystem damageEffect, float damage, ResourceType type)
    {
        life -= damage;
        ParticleSystem d = Instantiate(damageEffect, hit.point, Quaternion.identity);
        d.Play();
    }
}
