using System;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
    [SerializeField]
    private string switchInput = "Mouse ScrollWheel";
    [SerializeField] private Transform[] weapons;
    private int index = 0;

    private void Start()
    {
        switcher();
    }

    private void Update()    
    {
        if (Input.GetAxis(switchInput) > 0f)
        {
            index = (index + 1) % weapons.Length;
            switcher();
        }
        else if (Input.GetAxis(switchInput) < 0f)
        {
            index = Mathf.Abs( (index - 1) % weapons.Length );
            switcher();
        }
        
    }

    private void switcher()
    {
        weapons[ Mathf.Abs( (index - 1) % weapons.Length ) ].gameObject.SetActive(false);
        weapons[index].gameObject.SetActive(true);
    }
}
