using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMenu : MonoBehaviour
{
    [SerializeField] private List<Image> slots;

    private void Start()
    {
        ArtefactStock.Instance.update += UpdateSlots;
    }

    private void UpdateSlots()
    {
        int i = 0;
        foreach (Artefact a in ArtefactStock.Instance.Stock)
        {
            if(a!=null) slots[i++].sprite = a.Icon;
        }
    }
}
