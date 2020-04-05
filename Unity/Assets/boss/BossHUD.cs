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

   private void Start()
   {
      text.text = boss.blaze;
   }

   private void LateUpdate()
   {
      gauge.fillAmount = boss.life / 100;
   }
}
