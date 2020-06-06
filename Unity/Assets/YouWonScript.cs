using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouWonScript : MonoBehaviour
{
    private ParticleSystem[] confetti;

    void winAnimationStartFluff()
    {
        foreach (ParticleSystem ps in confetti)
        {
            ps.Play();
        }
    }
}
