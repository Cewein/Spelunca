using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMenu : MonoBehaviour
{
  [SerializeField] private LinePiece piece;

  private void Awake()
  {
    Inventory.Instance.updateConsomableStock += Display;
  }

  private void Display()
  {
    Clear();
    foreach (KeyValuePair<Consumable,int> item in Inventory.Instance.ConsumableStock)
    {
      LinePiece p = Instantiate(piece, transform);
      p.icon.sprite = item.Key.Icon;
      p.quantity.text =  item.Value.ToString();
      p.content = item.Key;
    }
    Debug.Log("ar");
  }
  
  private void Clear()
  {
    foreach (Transform child in GetComponentsInChildren<Transform>()) {
      if(child != this.transform) Destroy(child.gameObject);
    }
  }
  
  
}
