using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Menu.RingMenu
{
    [CreateAssetMenu(fileName = "RingElement", menuName = "Menu/RingMenu/Element", order = 2)]
    public class RingElement : ScriptableObject
    {
        public ResourceType Type;
        public Sprite Icon;
        public Artefact Artefact;
    }
}

