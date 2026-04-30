# GuitarToolkit v1.3.3

This release focuses on diagnostics, tab workflow polish, and project readiness before the planned interface redesign.

## Assets

- `GuitarToolkit_VST3_v.1.3.3.zip` - VST3 plugin package for DAW hosts.
- `GuitarToolkit_DESKTOP_v.1.3.3.zip` - standalone Windows desktop application.

## Added

- Basic application/plugin logging to `%AppData%\GuitarToolkit\logs`.
- About page with version, GitHub link, log folder shortcut, feature summary, and third-party library notes.
- Roadmap for product direction, bughunting, and commercial-readiness work.
- Tabs state saving: last file, selected track, volume, speed, auto-follow, sync offset, solo/mute state, and attempted cursor tick restore.
- Recent tab files, favorite tab files, and a simple recursive tab library folder.
- Library refresh button for rescanning the selected tab folder.
- `THIRD_PARTY_NOTICES.md` with dependency/license notes.

## Changed

- VST install documentation now emphasizes copying the complete plugin folder, including dependencies and `runtimes`.
- README installation notes now mention diagnostics and the log folder.
- README Tabs description now includes recent files, favorites, and library folder support.
- Desktop minimum window size now matches the startup size to protect the current layout.
- Tabs toolbar spacing was improved when controls wrap.
- Updated project, desktop, and VST plugin version to `1.3.3`.

## Known Follow-Up

- Restoring the saved tab cursor tick does not currently move alphaTab from the beginning after app restart.
- The Tabs toolbar is functional but visually crowded; the next development stage is a dedicated interface redesign.

## Verification

- Build should pass with 0 errors and 0 warnings after closing any running GuitarToolkit Desktop instance.
- Test suite should pass: 73/73.
