using UnityEditor.Presets;
using UnityEngine;

namespace Khanekg.EditorTools
{
    internal sealed class DefineSymbolSettingsReceiver : PresetSelectorReceiver
    {
        private bool append;
        private DefineSymbolSettings currentSettings;
        private DefineSymbolsTool currentWindow;
        private Preset initialValues;

        public void Init(DefineSymbolSettings settings, DefineSymbolsTool window, bool append = false)
        {
            currentWindow = window;
            currentSettings = settings;
            initialValues = new Preset(currentSettings);
            this.append = append;
        }

        public override void OnSelectionChanged(Preset selection)
        {
            if (!selection)
                selection = initialValues;
            if (!append)
            {
                currentSettings.InjectPreset(currentWindow, selection);
            }
        }

        public override void OnSelectionClosed(Preset selection)
        {
            if (append)
                currentSettings.AppendPreset(currentWindow, selection);
            else
                OnSelectionChanged(selection);
            DestroyImmediate(this);
        }
    }
}
