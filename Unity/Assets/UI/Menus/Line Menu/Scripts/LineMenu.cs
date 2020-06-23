﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LineMenu : MonoBehaviour
{
   [Tooltip("Name of the input used to increment the selected object index.")][SerializeField]
   [InputName] private string increment;
   [Tooltip("Name of the input used to decrement the selected object index.")][SerializeField]
   [InputName] private string decrement;
   [Tooltip("Name of the input to use the selected object.")][SerializeField]
   [InputName] private string use;
   [SerializeField] private int slotsNumber = 3;
   [SerializeField] private GameObject slotPrefab = null;
   private int activeElement;

   private void Awake()
   {
      ConsumableStock.Instance.update += UpdateSlots;
   }
   private void UpdateSlots()
   {
      Clear();
      foreach (List<Consumable> slot in ConsumableStock.Instance.Stock.Values)
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

   private void Update()
   {
      if (ConsumableStock.Instance.Stock.Count < 1) return;
      if (Input.GetButtonDown(increment)) activeElement = (activeElement+1)%ConsumableStock.Instance.Stock.Count;
      else if (Input.GetButtonDown(decrement) ) activeElement = (activeElement-1)%ConsumableStock.Instance.Stock.Count;
      else if (Input.GetButtonDown(use))
      {
         ConsumableStock.Instance.Stock.ElementAt(activeElement).Value[0].Use();
      }
   }
}