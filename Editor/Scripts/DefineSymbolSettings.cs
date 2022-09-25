using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Khanekg.EditorTools
{
    public class DefineSymbolSettings : ScriptableObject
    {
        [SerializeField]
        private List<ScriptingDefineSymbol> scriptingDefineSymbols;

        public bool TryGetDefines(out string[] defines)
        {
            if (scriptingDefineSymbols == null || scriptingDefineSymbols.Count == 0)
            {
                defines = null;
                return false;
            }
            defines = scriptingDefineSymbols.Where(x => x != null && !string.IsNullOrWhiteSpace(x.Value)).Select(x => x.Value).ToArray();
            return true;
        }

        public bool TryLoadFromGUID(string guid, out Preset preset)
        {
            if (string.IsNullOrEmpty(guid))
            {
                preset = null;
                return false;
            }
            var path = AssetDatabase.GUIDToAssetPath(guid);
            preset = AssetDatabase.LoadAssetAtPath<Preset>(path);
            if (preset)
            {
                preset.ApplyTo(this);
                return true;
            }
            return false;
        }

        internal void AppendPreset(DefineSymbolsTool window, Preset other)
        {
            if (other == null)
                return;
            var settings = ScriptableObject.CreateInstance<DefineSymbolSettings>();
            other.ApplyTo(settings);
            scriptingDefineSymbols ??= new List<ScriptingDefineSymbol>();
            scriptingDefineSymbols.AddRange(settings.scriptingDefineSymbols);
            window.SetModified();
        }

        internal void InjectPreset(DefineSymbolsTool window, Preset preset)
        {
            if (!window || !preset)
                return;

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(preset.GetInstanceID(), out string guid, out long _))
            {
                window.GUID = guid;
                window.CurrentPreset = preset;
            }
            preset.ApplyTo(this);
            window.Repaint();
        }
    }
}
