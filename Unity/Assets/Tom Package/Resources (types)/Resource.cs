using UnityEngine;
using UnityEngine.UI;

public enum ResourceType
{
        normal,
        fire,
        water,
        plant
};

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resources/ResourceType", order = 1)]

public class Resource: ScriptableObject
{

        [Tooltip("The resource type")][SerializeField]
        private ResourceType type;
        [Tooltip("The resource color displayed on gauge ui and gun ammo.")][SerializeField]
        private Color color;
        [Tooltip("The resource icon displayed on gauge ui.")][SerializeField]
        private Image gaugeIcon;
        [Tooltip("The resource icon displayed on inventory.")][SerializeField]
        private Image inventoryIcon;

        public ResourceType Type => type;
        public Color Color => color;
        public Image GaugeIcon => gaugeIcon;
        public Image InventoryIcon => inventoryIcon;

        public override string ToString()
        {
                return type.ToString();
        }
}

    