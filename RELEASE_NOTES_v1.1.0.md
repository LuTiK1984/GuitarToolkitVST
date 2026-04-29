# GuitarToolkit v1.1.0

## Release Assets

- `GuitarToolkit_VST3_v.1.1.0.zip` - VST3 plugin build for DAW hosts.
- `GuitarToolkit_DESKTOP_v.1.1.0.zip` - standalone Windows desktop build.

## Highlights

- VST3 plugin and standalone desktop app are provided as separate release archives.
- Project verification now includes additional `MetronomeEngine` unit coverage.
- VST deployment script now performs required-file checks and reports clearer deploy errors.
- User settings load/save failures now write diagnostic messages to debug output while keeping the app stable.
- Development notes were added to document VST-specific project rules and verification steps.

## Stability Notes

- VST audio processing remains conservative after DAW input reassignment testing.
- The plugin was checked against the scenario where a DAW input is assigned after GuitarToolkit is already loaded.

## Verification

- Build: passed with 0 errors and 0 warnings.
- Tests: 73/73 passed.

## Requirements

- Windows 10/11 x64.
- .NET 8 runtime.
- For VST3: a DAW host with VST3 support.
