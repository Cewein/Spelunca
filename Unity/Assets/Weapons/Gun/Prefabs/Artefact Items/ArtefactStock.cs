/*using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtefactStock", menuName = "ScriptableObjects/Artefact/ArtefactStock", order = 1)]
public class ArtefactStock : SingletonScriptableObject<ArtefactStock>
{
   private Artefact[] stock;
   public Artefact[] Stock{get => stock;}
   public Action update;
   public Artefact alreadyEquipedArtefact;

   private ArtefactStock()
   {
      stock = new Artefact[4];
   }

   public void TakeArtefact(Artefact a)
   { 
      for (int i = 0; i < stock.Length; i++) 
      {
         if (stock[i].name == a.name) stock[i] = null;
      }
      
      
   }

   public void SetArtefact(Artefact a)
   {
      try{stock[SlotNextEmptySocket()] = a;}
      catch (Exception e)
      {
         Debug.Log("Stock is full, choose an artefact to throw away"); //TODO : in game choice
      }
      update?.Invoke();
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
*/