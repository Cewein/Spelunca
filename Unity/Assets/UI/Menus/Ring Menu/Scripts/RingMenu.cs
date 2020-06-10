﻿using UnityEngine;
using System;
namespace UI.Menu.RingMenu
{
    


public class RingMenu : MonoBehaviour
{
   public Ring Data;
   public RingPiece RingCakePiecePrefab;
   public float GapWidthDegree = 1f;
   public Action<ResourceType,Artifact> callback;
   private RingPiece[] Pieces;
   private RingMenu Parent;
   private int activeElement;
   private ResourceType choosenType;
   private bool run = false;
   void Awake()
    {
        var stepLength = 360f / Data.Elements.Length;
        var iconDist = Vector3.Distance(RingCakePiecePrefab.Icon.transform.position, RingCakePiecePrefab.CakePiece.transform.position);

        //Position it
        Pieces = new RingPiece[Data.Elements.Length];

        for (int i = 0; i < Data.Elements.Length; i++)
        {
            Pieces[i] = Instantiate(RingCakePiecePrefab, transform);
            //set root element
            Pieces[i].transform.localPosition = Vector3.zero;
            Pieces[i].transform.localRotation = Quaternion.identity;

            //set cake piece
            Pieces[i].CakePiece.fillAmount = 1f / Data.Elements.Length - GapWidthDegree / 360f;
            Pieces[i].CakePiece.transform.localPosition = Vector3.zero;
            Pieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0, 0, -stepLength / 2f + GapWidthDegree / 2f + i * stepLength);
            Pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

            //set icon
            Pieces[i].Icon.transform.localPosition = Pieces[i].CakePiece.transform.localPosition +
                                                     Quaternion.AngleAxis(i * stepLength, Vector3.forward) *
                                                     Vector3.up * iconDist;
            Pieces[i].Icon.sprite = Data.Elements[i].Icon;
            
            
            //set data
            Pieces[i].Data = Data.Elements[i];
        }
    }

   private void UpdateIconAndData()
   {
       for (int i = 0; i < Data.Elements.Length; i++)
       {
          
           Pieces[i].Icon.sprite = Data.Elements[i].Icon;
           Pieces[i].Icon.color = (Pieces[i].Icon.sprite == null) ? new Color(Pieces[i].Icon.color.r,Pieces[i].Icon.color.g,Pieces[i].Icon.color.b,0): new Color(Pieces[i].Icon.color.r,Pieces[i].Icon.color.g,Pieces[i].Icon.color.b,1);
           Pieces[i].Data = Data.Elements[i];
       }
   }

   public void SetActive(bool isActive)
   {
       gameObject.SetActive(isActive);
      // Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
       UpdateIconAndData();
       if (!isActive) return;
       // call in menuManager
       callback?.Invoke(Pieces[activeElement].Data.Type,Pieces[activeElement].Data.Artefact);
       run = isActive;
    }
    private void Update()
    {
        if (!run) return;
        var stepLength = 360f / Pieces.Length;
        var mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - transform.position, Vector3.forward) + stepLength / 2f);
        activeElement = (int)(mouseAngle / stepLength);
        for (int i = 0; i < Pieces.Length; i++)
        {
            int j = (i + 1) % Pieces.Length;
            Pieces[j].CakePiece.color = i == activeElement  ? new Color(1f, 1f, 1f, 0.75f) : new Color(1f, 1f, 1f, 0.5f);
    
        }

    }

    private float NormalizeAngle(float a) => (a + 360f) % 360f;
}
}