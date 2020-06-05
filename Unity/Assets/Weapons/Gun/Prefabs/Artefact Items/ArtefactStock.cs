using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtefactStock", menuName = "ScriptableObjects/Artefact/ArtefactStock", order = 1)]
public class ArtefactStock : SingletonScriptableObject<ArtefactStock>
{
   [SerializeField] private Artefact defaultWeaponArtefact;
   private Artefact[] stock;
   public Artefact[] Stock{get => stock;}

   private ArtefactStock()
   {
      stock = new Artefact[4];
      stock[0] = defaultWeaponArtefact;
   }

   public void TakeArtefact(Artefact a)
   {
      if (!a.name.Equals(defaultWeaponArtefact.name))
      {
         for (int i = 1; i < stock.Length; i++) // first is ignore because it's the default one.
         {
            if (stock[i].name == a.name) stock[i] = null;
         }
      }
      else
      {
         Debug.Log("Cannot remove the default artefact ! "); //TODO : in game warning
      }
   }

   public void SetArtefact(Artefact a)
   {
      try{stock[SlotNextEmptySocket()] = a;}
      catch (Exception e)
      {
         Debug.Log("Stock is full, choose an artefact to throw away"); //TODO : in game choice
      }
   }
   
   public int SlotNextEmptySocket()
   {
      int counter = 0;
      foreach (Artefact item in stock)
      {
         if (item != null) counter++;
         else return counter;
      }
      return counter;
   }
   public override string ToString()
   {
      string str = "";
      foreach (Artefact artefact in stock)
      {
         if (artefact != null) str += "-" + artefact.name + "\n";
      }
      return str;
   }

   
}
