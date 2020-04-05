using UnityEngine;

public class bossScript : MonoBehaviour, IDamageable
{
    public string blaze = "The Bad Guy";
    public float life = 50;
    public bool isAlive = true;
    private float deathTime = 5f;
    public GameObject root = null;
    void Update()
    {
        if (life <= 0)
        {
            isAlive = false;
            Destroy(root);
        }
    }

    public void setDamage(RaycastHit hit, ParticleSystem damageEffect, float damage, ResourceType type)
    {
        life -= damage;
        ParticleSystem d = Instantiate(damageEffect, hit.point, Quaternion.identity);
        d.Play();
    }


}
