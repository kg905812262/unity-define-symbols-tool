using UnityEditor;
using UnityEngine;

namespace Khanekg.EditorTools
{
    [CustomPropertyDrawer(typeof(ScriptingDefineSymbol))]
    public class ScriptingDefineSymbolDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope = new EditorGUI.PropertyScope(position, label, property))
            {
                var labelRect = position;
                labelRect.width = 200f;
                if (property.objectReferenceValue)
                {
                    var value = new SerializedObject(property.objectReferenceValue).FindProperty("value");
                    EditorGUI.SelectableLabel(labelRect, value.stringValue);
                }
                var objectFieldRect = position;
                objectFieldRect.x += 200f;
                objectFieldRect.width -= 200f;
                property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, property.objectReferenceValue, typeof(ScriptingDefineSymbol), false);
            }
        }
    }
}
