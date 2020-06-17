using System;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class Hexaslot : MonoBehaviour
{
   [SerializeField] private Transform slot1;
   [SerializeField] private Transform slot2;
   [SerializeField] private Transform slot3;
   
   private void Start()
   {
      Inventory.Instance.updateArtifactStock += Refresh;
      Refresh();
   }

   private void Refresh()
   {
      try
      {
         if(Inventory.Instance.ArtifactStock.ElementAt(0) != null)
            Set(slot1, Inventory.Instance.ArtifactStock.ElementAt(0).Icon);
         else Empty(slot1);
      }
      catch(ArgumentOutOfRangeException e){{Empty(slot1);}}
      
      try
      {
         if(Inventory.Instance.ArtifactStock.ElementAt(1) != null)
            Set(slot2, Inventory.Instance.ArtifactStock.ElementAt(1).Icon);
         else Empty(slot2);
      }
      catch(ArgumentOutOfRangeException e){{Empty(slot2);}}
      
      try
      {
         if(Inventory.Instance.ArtifactStock.ElementAt(2) != null)
            Set(slot3, Inventory.Instance.ArtifactStock.ElementAt(2).Icon);
         else Empty(slot3);
      }
      catch(ArgumentOutOfRangeException e){{Empty(slot3);}}

   }

   private void Empty(Transform slot)
   {
      slot.GetComponent<Image>().sprite = null;
      slot.GetComponent<Image>().enabled  = false;
   }

   private void Set(Transform slot, Sprite icon)
   {
      slot.GetComponent<Image>().sprite = icon;
      slot.GetComponent<Image>().enabled  = true;
   }
}
