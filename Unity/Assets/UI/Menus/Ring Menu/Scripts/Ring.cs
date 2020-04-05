using UnityEngine;
namespace UI.Menu.RingMenu
{
    [CreateAssetMenu(fileName = "Ring", menuName = "Menu/RingMenu/Ring", order = 1)]
    public class Ring : ScriptableObject
    {    
        public RingElement[] Elements;
    }
}

