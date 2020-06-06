using System;
using System.Collections;
using System.Collections.Generic;
using UI.Menu.RingMenu;
using UnityEngine;
using UnityEngine.UI;

public class GridMenu : MonoBehaviour
{
    [SerializeField] private List<Image> slots;
    [SerializeField] private RingMenu ringMenu;

    private void Start()
    {
        UpdateSlots();
        ArtefactStock.Instance.update += UpdateSlots;
    }

    private void UpdateSlots()
    { 
       Artefact current;
       for (int i = 0; i < ArtefactStock.Instance.Stock.Length; i++)
       {
           current = ArtefactStock.Instance.Stock[i];
           if (current != null)
           {
               slots[i].sprite = current.Icon;
               ringMenu.Data.Elements[i].Icon = slots[i].sprite ;
               ringMenu.Data.Elements[i].Artefact = current;
           }
           else
           {
               ringMenu.Data.Elements[i].Icon = null ;
               ringMenu.Data.Elements[i].Artefact = null;
           }
          
       }
    }
}
