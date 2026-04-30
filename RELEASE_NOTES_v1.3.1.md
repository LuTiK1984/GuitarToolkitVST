# GuitarToolkit v1.3.1

## Release Assets

- `GuitarToolkit_VST3_v.1.3.1.zip` - VST3 plugin build for DAW hosts.
- `GuitarToolkit_DESKTOP_v.1.3.1.zip` - standalone Windows desktop build.

## Highlights

- Fixed the VST3 release package so transitive alphaTab/AlphaSkia dependencies are copied next to the plugin.
- The VST3 editor no longer instantiates the alphaTab-based Tabs page inside DAW hosts; Tabs remain desktop-only for now.
- Updated project, VST plugin, and desktop UI version to `1.3.1`.

## Why This Patch Exists

- `v1.3.0` added desktop Tabs support, but the VST package did not include all alphaTab runtime dependencies expected by `GuitarToolkit.UI`.
- This patch adds the missing dependencies to VST output/package and avoids initializing the Tabs page in plugin hosts.

## Verification

- Build: passed with 0 errors and 0 warnings.
- Tests: 73/73 passed.
- VST3 Release output contains `AlphaTab.dll`, `AlphaTab.Windows.dll`, `AlphaSkia.dll`, and `AlphaSkia.Native.Windows.dll`.

## Requirements

- Windows 10/11 x64.
- .NET 8 runtime.
- For VST3: a DAW host with VST3 support.
