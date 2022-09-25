using UnityEditor;

namespace Khanekg.EditorTools
{
    [CustomEditor(typeof(DefineSymbolSettings))]
    public class DefineSymbolSettingsEditor : Editor
    {
        private SerializedProperty scriptingDefineSymbols;

        private void OnEnable()
        {
            scriptingDefineSymbols = serializedObject.FindProperty("scriptingDefineSymbols");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(scriptingDefineSymbols);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
