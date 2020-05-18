using System;
using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
   public Image gauge;
   public GameObject bossHUDRoot;
   public bossScript boss;
   public Transform playerTransform;
   public Text text;
   private bool ok = true;
   private void Start()
   {
      text.text = boss.blaze;
   }

   private void Update()
   {
      try
      {
         bossHUDRoot.SetActive(Vector3.Distance(boss.gameObject.transform.position, playerTransform.position) < 18);

      }
      catch (MissingReferenceException e)
      {
         bossHUDRoot.SetActive(false);
         ok = false;
      }
   }

   private void LateUpdate()
   {
      if(ok) gauge.fillAmount = boss.life / 100;
   }
}
