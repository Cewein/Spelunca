using System;
using UnityEngine;

public class LineMenu : MonoBehaviour
{
   [SerializeField] private int slotsNumber = 3;
   [SerializeField] private GameObject slotPrefab = null;

   private void Awake()
   {
      ConsumableStock.Instance.update += UpdateSlots;
   }

   private void UpdateSlots()
   {
      Clear();
      foreach (Consumable[] slot in ConsumableStock.Instance.Stock.Values)
      {
         Instantiate(slotPrefab, transform);
         slotPrefab.GetComponent<LinePiece>().icon.sprite = slot[0].Icon;
         slotPrefab.GetComponent<LinePiece>().SetQuantity(ConsumableStock.Instance.SlotNextEmptySocket(slot));
         slotPrefab.GetComponent<LinePiece>().Name = slot[0].Name;
      }
   }

   private void Clear()
   {
      foreach (Transform child in GetComponentsInChildren<Transform>()) {
         if(child != this.transform) Destroy(child.gameObject);
      }
   }
}
