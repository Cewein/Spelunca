using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws MinMaxAttribute slider in inspector.
/// </summary>
[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Vector2)
        {

            MinMaxAttribute minMax = attribute as MinMaxAttribute;
            Rect rectangle  = EditorGUI.PrefixLabel(position, label);
            Rect leftRect   = new Rect(rectangle.x, rectangle.y, 50, rectangle.height);
            Rect rightRect  = new Rect(rectangle.xMax - leftRect.width, rectangle.y, leftRect.width, rectangle.height);
            Rect sliderRect = new Rect(leftRect.xMax + 3, rectangle.y, rectangle.width - ( leftRect.width + rightRect.width + 10), rectangle.height);
            float minCurrentValue = EditorGUI.FloatField(leftRect, property.vector2Value.x);
            float maxCurrentValue = EditorGUI.FloatField(rightRect, property.vector2Value.y);
            EditorGUI.MinMaxSlider(sliderRect, ref minCurrentValue, ref maxCurrentValue, minMax.min, minMax.max);
            property.vector2Value = new Vector2(minCurrentValue, maxCurrentValue);

        }
        else GUI.Label(position, "MinMAx attribute can only work with Vector2 !");
    }
}
