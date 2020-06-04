using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HealthPoint : MonoBehaviour
{
    [Tooltip("The animator of the glow effect of the health point.")][SerializeField]
    private Animator glowAnimator;
    
    [Tooltip("The animator of the inside of the health point.")][SerializeField]
    private Animator insideAnimator;

    public bool hit = false;

    public bool enable = true;

    public float timeBeforeDesapear = 10f;
    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hit == true && enable == true)
        {
            StartCoroutine(DisablePoint());
        }
        glowAnimator.SetBool("hit",this.hit);
        glowAnimator.SetBool("enable",this.enable);
        insideAnimator.SetBool("hit",this.hit);
        insideAnimator.SetBool("enable",this.enable);
    }

    IEnumerator DisablePoint()
    {
        yield return new WaitForSeconds(timeBeforeDesapear);
        enable = false;
    }
}
