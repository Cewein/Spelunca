using System;
using UnityEngine;

public class VisualFeedbacks : MonoBehaviour
{
   [Tooltip("Hit mark prefab.")] [SerializeField]
   private GameObject hitMark = null;
   [Tooltip("PlayerStats.")] [SerializeField]
   private PlayerStats player = null;

   private void Awake()
   {
      player.hurt += (_, direction, __) => {hurtFeedback(direction) ; };
   }

   private void hurtFeedback(Vector3 direction)
   {
      Instantiate(hitMark, transform).transform.rotation = Quaternion.Euler(direction);
   }
}
