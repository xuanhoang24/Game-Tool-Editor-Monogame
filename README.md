# Game Editor

A MonoGame-based game editor built with C# and Windows Forms, developed as coursework to demonstrate game tools development and engine architecture.

## Features

- **Visual Editor Interface**: Windows Forms-based editor for game development
- **MonoGame Integration**: Built on MonoGame Framework for cross-platform game development
- **Scripting Support**: Lua scripting integration via MoonSharp
- **Content Pipeline**: Asset management and content building capabilities
- **Modular Architecture**: Organized into Engine, Editor, GUI, and Content modules

## Requirements

- .NET 8.0 or later
- Windows operating system
- Visual Studio 2022 or later

## Getting Started

### Building the Project

1. Clone the repository
2. Open `Editor.sln` in Visual Studio
3. Build the solution (Ctrl+Shift+B)

### Running the Editor

1. Set the Editor project as the startup project
2. Press F5 to run in debug mode, or Ctrl+F5 to run without debugging

## Project Structure

```
├── Editor/           # Main editor application
│   ├── Editor/       # Core editor functionality
│   ├── Engine/       # Game engine components
│   ├── GUI/          # User interface components
│   ├── Content/      # Content pipeline and assets
│   └── External/     # External dependencies
├── Content/          # Game content and assets
├── Engine/           # Shared engine interfaces and scripting
└── External/         # External tools and utilities
```

## Dependencies

- **MonoGame.Framework.WindowsDX**: Core game framework
- **MonoGame.Content.Builder.Task**: Content pipeline build tasks
- **MoonSharp**: Lua scripting engine for .NET

## Development

The editor supports:
- Asset management through the content pipeline
- Lua scripting for game logic
- Visual editing tools
- Content building and deployment

### Managing Assets with MGCB Editor

To add or update model texture assets, use the **MGCB Editor** (MonoGame Content Builder Editor):

#### Using the MGCB Editor GUI
1. Open `Editor/Content/Content.mgcb` with the MGCB Editor
2. **Add Assets**: Right-click in the Project panel → "Add" → "Existing Item" or "New Item"
3. **Configure Properties**: Select the asset and modify settings in the Properties panel
4. **Build Assets**: Use the Build menu or toolbar to compile assets
5. **View Output**: Check the Build Output panel for compilation results

#### Asset Types Supported
- **Textures**: .jpg, .png, .bmp (uses TextureImporter)
- **3D Models**: .fbx, .x (uses FbxImporter/XImporter) 
- **Audio**: .wav, .mp3 (uses WavImporter)
- **Fonts**: .spritefont (uses FontDescriptionImporter)
- **Shaders**: .fx (uses EffectImporter)

#### Build Process
- Assets are processed from source files into XNB format
- Built assets are output to `bin/$(Platform)/` directory
- The editor automatically loads processed assets at runtime

**Note**: Place source asset files in the `Editor/Content/` directory before adding them through the MGCB Editor.