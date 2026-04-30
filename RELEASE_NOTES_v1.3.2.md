# GuitarToolkit v1.3.2

## Release Assets

- `GuitarToolkit_VST3_v.1.3.2.zip` - VST3 plugin build for DAW hosts.
- `GuitarToolkit_DESKTOP_v.1.3.2.zip` - standalone Windows desktop build.

## Highlights

- Tabs are now enabled in the VST3 plugin.
- FL Studio testing confirmed the plugin loads with the complete dependency set.
- VST deployment now copies the full plugin output folder, including transitive dependencies and runtime folders.
- README screenshots and bilingual feature descriptions now include the Tabs page.
- README positioning was updated from an educational project to a personal passion project.
- Updated project, VST plugin, and desktop UI version to `1.3.2`.

## Included Documentation

- English and Russian installation guides are included in release archives.
- `CHANGELOG.md` documents the `1.3.2` release.

## Verification

- Build: passed with 0 errors and 0 warnings.
- Tests: 73/73 passed.
- VST3 package includes alphaTab/AlphaSkia dependencies and runtime files needed by the tab viewer.

## Requirements

- Windows 10/11 x64.
- .NET 8 runtime.
- For VST3: a DAW host with VST3 support.
