# unity-define-symbols-tool
An editor tool for easily applying groups of scripting define symbols.

## Feature
- Easily apply different groups of scripting define symbols
- Manage groups of scripting define symbols through dedicated editor tool and Unity Presets
- Reusable assets to avoid copying and pasting strings to Scripting Define Symbols in the Player Settings

## Installation
1. Window -> Package Manager -> Add package from git URL...
2. Paste `https://github.com/kg905812262/unity-define-symbols-tool.git`

## Usage
### Creating Scripting Define Symbols
1. Choose any location under Assets folder
2. Right click Create -> Define Symbols Tool -> `Scripting Define Symbol`
3. Select the asset you just created and enter any valid value as a define symbol
### Editing
1. Open the tool via menu Window -> Define Symbols Tool
2. Add a define symbol to the list **Scripting Define Symbols** via the `+` sign and choose or drag any `Scripting Define Symbol` asset to the object field
3. If "New Settings" shows in the header, then you have to create a Preset to save your changes. Click the Preset icon on the right side of the header -> Save current to...
4. If a Preset name shows in the header, then you just need to click `Save Preset` button at the bottom to save your changes to the Preset
5. Click `Append Preset` to append define symbols from another Preset to the current one
6. Click `Revert` to revert changes to the Preset
7. Click `Apply` to apply define symbols from the Preset to **Scripting Define Symbols** of current active build target
