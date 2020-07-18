using System;
using UnityEngine;

public class LifePotion : MonoBehaviour
{
   [SerializeField] private int healPoints = 1;

   public void callback()
   {
      FindObjectOfType<PlayerStats>().RestoreLife(healPoints); // TODO : not very good to use findObjectOfType
   }
}
