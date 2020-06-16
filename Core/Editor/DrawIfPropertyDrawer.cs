using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Core.Editor
{
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        // Height of the property.
        private readonly Dictionary<string, float> _propertyHeight = new Dictionary<string, float>();
     
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_propertyHeight.ContainsKey(property.propertyPath))
            {
                return _propertyHeight[property.propertyPath];
            }

            return 0;
        }
     
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Set the global variables.
            var drawIf = attribute as DrawIfAttribute;
            var propertyStringValue = "";
            if (property.depth > 1)
            {
                propertyStringValue = GetValue(GetParent(property), drawIf.ComparedPropertyName).ToString();
            }
            else
            {
                var serializedProperty = property.serializedObject.FindProperty(drawIf.ComparedPropertyName);
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
            _propertyHeight[property.propertyPath] = base.GetPropertyHeight(property, label);
       
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
                    _propertyHeight[property.propertyPath] = 0f;
                }
            }
        }
        public object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach(var element in elements.Take(elements.Length-1))
            {
                if(element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }
 
        public object GetValue(object source, string name)
        {
            if(source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if(f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if(p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }
 
        public object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while(index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}
