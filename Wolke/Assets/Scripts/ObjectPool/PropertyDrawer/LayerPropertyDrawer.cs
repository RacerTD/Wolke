using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            Debug.LogWarning("LayerAttribute can only be applied on integer properties/fields");
            return;
        }

        property.intValue = EditorGUI.LayerField(position, property.name, property.intValue);
    }
}