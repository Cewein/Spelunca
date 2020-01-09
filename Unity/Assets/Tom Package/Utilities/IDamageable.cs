using UnityEngine;

public interface IDamageable
{
    void setDamage(RaycastHit hit, ParticleSystem damageEffect);
}
