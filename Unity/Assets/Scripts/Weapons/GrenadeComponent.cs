using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class GrenadeComponent : MonoBehaviour
{
    public float delayedExplosionTime = 1f;
    public GameObject ExplosionEffect;
    private bool gonnaBoom = false;
    public float explosionRadius = 1f;
    public float damages = 10f;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision other)
    {
        if (!gonnaBoom)
        {
            StartCoroutine(waitAndBoom());
        }
    }

    private IEnumerator waitAndBoom()
    {
        gonnaBoom = true;
        yield return new WaitForSeconds(delayedExplosionTime);
        this.GetComponent<MeshRenderer>().enabled = false;
        Instantiate(ExplosionEffect, transform.position, transform.rotation);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.up);
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                hit.collider.gameObject.GetComponent<EnemyComponent>().hit(damages);
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
