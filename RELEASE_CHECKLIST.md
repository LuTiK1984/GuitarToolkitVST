# Release Checklist

Use this checklist before publishing a GuitarToolkit release.

## 1. Version

- Update `Directory.Build.props`.
- Update `GuitarToolkit.Plugin/GuitarToolkitPlugin.cs` plugin version.
- Update desktop window title if the version is visible in UI.
- Update `CHANGELOG.md`.
- Update README release archive names if needed.
- Check `THIRD_PARTY_NOTICES.md` when dependencies change.
- Check community files if contributor workflow changed:
  - `CONTRIBUTING.md`
  - `SECURITY.md`
  - `.github/PULL_REQUEST_TEMPLATE.md`
- Check DAW docs if VST behavior changed:
  - `docs/FL_STUDIO.md`
  - `docs/REAPER.md`
  - `docs/SUPPORTED_DAWS.md`

## 2. Build

Run:

```powershell
dotnet build GuitarToolkit.sln --no-restore --configuration Debug
dotnet test GuitarToolkit.sln --no-restore --configuration Debug
powershell -ExecutionPolicy Bypass -File .\build-release.ps1 -Version <version> -Configuration Release
```

Expected:

- Build has 0 errors and 0 warnings.
- All tests pass.
- Release ZIP files are created in `artifacts/release/`.

## 3. Manual Desktop Check

- Launch `GuitarToolkit.Desktop.exe`.
- Confirm the application icon appears in the window and taskbar.
- Select an input device.
- Check tuner input.
- Check metronome playback.
- Check chord/scale playback.
- Check the Tabs page: open a Guitar Pro file, switch tracks, play/pause/stop, solo/mute, resize maximize/restore, and verify auto-follow.
- Check Tabs recent files, favorites, library folder loading, and library refresh.
- Known follow-up: while the Tabs page adapts after resize/maximize/restore, alphaTab can visibly re-render a few times before settling.

## 4. Manual VST3 Check

- Close the DAW.
- Deploy the plugin or copy release files to the VST3 folder.
- Open the DAW and rescan plugins.
- Add GuitarToolkit to an audio track.
- Assign or change the recording input after the plugin is loaded.
- Check tuner input, metronome playback, and chord/scale playback.
- Check Tabs page loading, playback, recent files, favorites, and library folder if the DAW can open the editor reliably.

## 5. GitHub Release

- Create a draft release.
- Upload:
  - `GuitarToolkit_VST3_v.<version>.zip`
  - `GuitarToolkit_DESKTOP_v.<version>.zip`
- Paste release notes from `CHANGELOG.md` or a dedicated release notes file.
- Review asset names and description.
- Publish the release.

## 6. Post-release Repository Check

- Confirm GitHub Community Standards still pass.
- Confirm repository description and topics are up to date.
- Confirm the latest release assets download correctly.
- Open a fresh issue form preview if issue templates changed.
