using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Presets;
using UnityEngine;

namespace Khanekg.EditorTools
{
    internal class DefineSymbolsTool : EditorWindow, IActiveBuildTargetChanged
    {
        private static class Styles
        {
            public static readonly Color headerTint = new Color(0.55f, 0.55f, 0.55f, 0.2f);
            public static readonly Color splitterTint = new Color(0.12f, 0.12f, 0.12f, 1.333f);
            public static readonly GUIContent presetIcon = EditorGUIUtility.IconContent("Preset.Context");
            public static readonly GUIStyle iconButton = new GUIStyle("IconButton");
        }

        public Preset CurrentPreset
        {
            get => currentPreset;
            set => currentPreset = value;
        }

        public string GUID
        {
            get => EditorPrefs.GetString(kEditorPrefsKey, null);
            set => EditorPrefs.SetString(kEditorPrefsKey, value);
        }

        public int callbackOrder => 100;
        private const string kActiveBuildTargetLabel = "Active Build Target";
        private const string kEditorPrefsKey = "DefineSymbolsTool.Preset.GUID";
        private const string kDefaultTitle = "New Settings";
        private DefineSymbolSettings currentSettings;
        private Editor settingsEditor;
        private MessageType messageType;
        private Preset currentPreset;
        private string activeBuildTargetName;
        private string helpBoxMessage;
        private bool isChanged;

        [MenuItem("Window/Define Symbols Tool")]
        private static void OpenWindow()
        {
            var window = GetWindow<DefineSymbolsTool>(ObjectNames.NicifyVariableName(nameof(DefineSymbolsTool)));
            window.minSize = new Vector2(500f, 600f);
            window.activeBuildTargetName = EditorUserBuildSettings.activeBuildTarget.ToString();
        }

        private void OnEnable()
        {
            currentSettings = ScriptableObject.CreateInstance<DefineSymbolSettings>();
            if (!currentSettings.TryLoadFromGUID(GUID, out currentPreset))
            {
                helpBoxMessage = "No preset loaded.";
                messageType = MessageType.Warning;
            }
            settingsEditor = Editor.CreateEditor(currentSettings);
        }

        private void OnDisable()
        {
            DestroyImmediate(currentSettings);
            DestroyImmediate(settingsEditor);
        }

        private void OnGUI()
        {
            using (var scope = new EditorGUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.PrefixLabel(kActiveBuildTargetLabel, EditorStyles.boldLabel);
                if (GUILayout.Button(activeBuildTargetName, EditorStyles.miniButton))
                {
                    EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                }
                GUILayout.FlexibleSpace();
            }

            DrawHeader();
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                settingsEditor.OnInspectorGUI();
                if (scope.changed)
                {
                    if (!string.IsNullOrEmpty(helpBoxMessage))
                        helpBoxMessage = string.Empty;
                    isChanged = true;
                }
            }

            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (var _ = new EditorGUI.DisabledGroupScope(!currentPreset || !isChanged))
                {
                    if (GUILayout.Button("Save Preset"))
                    {
                        if (currentPreset.UpdateProperties(currentSettings))
                        {
                            isChanged = false;
                            helpBoxMessage = "Preset saved!";
                            messageType = MessageType.Info;
                        }
                        else
                        {
                            helpBoxMessage = "Failed to save preset.";
                            messageType = MessageType.Error;
                        }
                    }
                }
                if (GUILayout.Button("Append Preset"))
                {
                    ShowPresetSelector(true);
                }
                using (var _ = new EditorGUI.DisabledGroupScope(!currentPreset || !isChanged))
                {
                    if (GUILayout.Button("Revert"))
                    {
                        currentPreset.ApplyTo(currentSettings);
                        isChanged = false;
                    }
                }
                if (GUILayout.Button("Apply"))
                {
                    if (currentSettings.TryGetDefines(out var defines) && defines.Length > 0)
                    {
                        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
#if UNITY_2021_2_OR_NEWER
                        var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines);
#else
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
#endif
                        helpBoxMessage = "Apply to current active build target.";
                        messageType = MessageType.Info;
                    }
                    else
                    {
                        helpBoxMessage = "Failed to apply preset.";
                        messageType = MessageType.Error;
                    }
                }
            }
            EditorGUILayout.Space();
            float fadeValue = string.IsNullOrWhiteSpace(helpBoxMessage) ? 0f : 1f;
            using (var scope = new EditorGUILayout.FadeGroupScope(fadeValue))
            {
                if (scope.visible)
                    EditorGUILayout.HelpBox(helpBoxMessage, messageType);
            }
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            activeBuildTargetName = newTarget.ToString();
        }

        internal void SetModified() => isChanged = true;

        private void ShowPresetSelector(bool append = false)
        {
            var presetReceiver = ScriptableObject.CreateInstance<DefineSymbolSettingsReceiver>();
            presetReceiver.Init(currentSettings, this, append);
            PresetSelector.ShowSelector(currentSettings, null, true, presetReceiver);
            if (!string.IsNullOrEmpty(helpBoxMessage))
                helpBoxMessage = string.Empty;
        }

        private static void DrawSplitter(float thickness)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight));
            r.x -= 2f;
            r.width += 6f;
            DrawSplitter(r, thickness);
        }

        private static void DrawSplitter(Rect r, float thickness)
        {
            r.height = thickness;
            EditorGUI.DrawRect(r, Styles.splitterTint);
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space();
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                Rect r = scope.rect;
                r.yMax += 2f;
                r.yMin -= 2f;

                var labelRect = r;
                labelRect.xMin += 19f;
                labelRect.width = 200f;

                r.xMin = 0f;
                r.width += 6f;
                DrawSplitter(r, 1f);
                DrawdHeaderBackground(r, 1f);

                var title = currentPreset ? currentPreset.name : kDefaultTitle;
                EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (EditorGUILayout.DropdownButton(Styles.presetIcon, FocusType.Passive, Styles.iconButton))
                {
                    ShowPresetSelector();
                }
            }
            EditorGUILayout.Space();
        }

        private void DrawdHeaderBackground(Rect r, float splitterThickness)
        {
            r.yMin += splitterThickness;
            EditorGUI.DrawRect(r, Styles.headerTint);
        }
    }
}
