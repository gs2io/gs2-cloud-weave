using System;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Core.Editor
{
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        // Height of the property.
        private float _propertyHeight;
     
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _propertyHeight;
        }
     
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Set the global variables.
            var drawIf = attribute as DrawIfAttribute;
            var serializedProperty = property.serializedObject.FindProperty(drawIf.ComparedPropertyName);

            var propertyStringValue = "";
            switch (serializedProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    propertyStringValue = serializedProperty.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    propertyStringValue = serializedProperty.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    propertyStringValue = ((long)serializedProperty.floatValue).ToString();
                    break;
                case SerializedPropertyType.String:
                    propertyStringValue = serializedProperty.intValue.ToString();
                    break;
                default:
                    throw new NotSupportedException();
            }
     
            // Is the condition met? Should the field be drawn?
            var conditionMet = false;

            // Compare the values to see if the condition is met.
            switch (drawIf.ComparisonType)
            {
                case ComparisonType.Equals:
                    if (propertyStringValue.Equals(drawIf.ComparedValue.ToString()))
                        conditionMet = true;
                    break;
     
                case ComparisonType.NotEqual:
                    if (!propertyStringValue.Equals(drawIf.ComparedValue.ToString()))
                        conditionMet = true;
                    break;
     
                case ComparisonType.GreaterThan:
                    if (Convert.ToInt64(propertyStringValue) > Convert.ToInt64(drawIf.ComparedValue.ToString()))
                        conditionMet = true;
                    break;
     
                case ComparisonType.SmallerThan:
                    if (Convert.ToInt64(propertyStringValue) < Convert.ToInt64(drawIf.ComparedValue.ToString()))
                        conditionMet = true;
                    break;
     
                case ComparisonType.SmallerOrEqual:
                    if (Convert.ToInt64(propertyStringValue) <= Convert.ToInt64(drawIf.ComparedValue.ToString()))
                        conditionMet = true;
                    break;
     
                case ComparisonType.GreaterOrEqual:
                    if (Convert.ToInt64(propertyStringValue) >= Convert.ToInt64(drawIf.ComparedValue.ToString()))
                        conditionMet = true;
                    break;
            }
     
            // The height of the property should be defaulted to the default height.
            _propertyHeight = base.GetPropertyHeight(property, label);
       
            // If the condition is met, simply draw the field. Else...
            if (conditionMet)
            {
                EditorGUI.PropertyField(position, property);
            }
            else
            {
                //...check if the disabling type is read only. If it is, draw it disabled, else, set the height to zero.
                if (drawIf.DisablingType == DisablingType.ReadOnly)
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property);
                    GUI.enabled = true;
                }
                else
                {
                    _propertyHeight = 0f;
                }
            }
        }
    }
}