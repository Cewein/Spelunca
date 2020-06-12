using Unity.Entities;
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
        private ResourceType type = ResourceType.normal;
        [Tooltip("The resource color displayed on gauge ui and gun ammo.")][SerializeField]
        private Color color = Color.black;
        [Tooltip("The resource icon displayed on gauge ui.")][SerializeField]
        private Sprite icon = null;
        [Tooltip("The resource prefab instantiate in game.")][SerializeField]
        private GameObject prefab;

        public ResourceType Type => type;
        public Color Color => color;
        public Sprite Icon => icon;
        public GameObject Prefab => prefab;

        public override string ToString()
        {
                return type.ToString();
        }
}

    