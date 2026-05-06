# GuitarToolkit Agent Guide

This file is for AI/coding agents and contributors who need a quick map of the repository, project boundaries, and verification rules.

GuitarToolkit is a Windows guitar toolkit shipped as both:

- a standalone WPF desktop app;
- a VST3 plugin for DAW hosts.

The project is sensitive to audio, VST3 runtime packaging, and DAW behavior. Prefer small, focused changes and avoid broad refactors unless explicitly requested.

## Repository map

```text
/
|-- README.md                         Public project landing page
|-- LICENSE                           MIT license
|-- CHANGELOG.md                      Canonical release history
|-- AGENTS.md                         This guide
|-- GuitarToolkit.sln                 Solution file
|-- Directory.Build.props             Shared .NET metadata and versions
|-- build-release.ps1                 Release ZIP build script
|-- deploy-vst.bat                    Local VST3 deployment helper
|-- .github/                          GitHub workflows, issue/discussion templates, Dependabot
|-- docs/
|   |-- README.md                      Documentation index
|   |-- user/                          User-facing documentation
|   |-- maintainer/                    Maintainer/project-process documentation
|   |-- releases/                      Release-process documentation
|   `-- images/                        README/docs screenshots
|-- GuitarToolkit.Core/               DSP, music theory, engines, settings
|-- GuitarToolkit.UI/                 Shared WPF UI used by Desktop and VST3
|-- GuitarToolkit.Desktop/            Standalone Windows app integration
|-- GuitarToolkit.Plugin/             VST3 plugin integration
`-- GuitarToolkit.Tests/              xUnit tests for Core behavior
```

## Documentation map

Use the documentation index first:

- `docs/README.md` - full documentation index.

User documentation:

- `docs/user/QUICK_START.md` - install and launch Desktop/VST3.
- `docs/user/SUPPORTED_DAWS.md` - DAW compatibility notes.
- `docs/user/FL_STUDIO.md` - FL Studio setup.
- `docs/user/REAPER.md` - Reaper setup.
- `docs/user/SUPPORT.md` - support routing.
- `docs/user/KNOWN_TAB_IMPORT_ISSUES.md` - known alphaTab/Guitar Pro import limitations.

Maintainer documentation:

- `docs/maintainer/MAINTAINER_GUIDE.md` - triage, issues, discussions, routines.
- `docs/maintainer/PROJECT_STRUCTURE.md` - file placement rules.
- `docs/maintainer/GITHUB_SETTINGS.md` - GitHub UI settings and external checks.
- `docs/maintainer/REPOSITORY_PRESENTATION.md` - About text, topics, pinned posts.
- `docs/maintainer/BRANCHING_AND_PROTECTION.md` - branch workflow and protection rules.
- `docs/maintainer/LABELS_AND_MILESTONES.md` - issue labels and milestones.
- `docs/maintainer/DEPENDENCY_POLICY.md` - dependency and Dependabot update rules.
- `docs/maintainer/DISCUSSIONS.md` - GitHub Discussions categories and guidance.
- `docs/maintainer/ROADMAP.md` - product direction and longer-term plans.
- `docs/maintainer/INSPIRATION_ENGINE_MODEL.md` - ONNX-first generation contract and training workflow.

Release documentation:

- `docs/releases/RELEASE_CHECKLIST.md` - short release checklist.
- `docs/releases/RELEASE_PROCESS.md` - full release process.
- `docs/releases/RELEASE_PAGE_GUIDE.md` - GitHub Releases page guidance.
- `docs/releases/RELEASE_NOTES_v1.6.0_DRAFT.md` - draft repository-polish release notes.

## Project shape

- `GuitarToolkit.Core` contains DSP, theory models, and services. Keep it free of WPF, NAudio, and AudioPlugSharp dependencies.
- `GuitarToolkit.UI` contains shared WPF controls used by both the desktop app and the VST3 plugin.
- `GuitarToolkit.Desktop` is the standalone Windows app and uses NAudio for input/output.
- `GuitarToolkit.Plugin` is the VST3 entry point and uses AudioPlugSharp/AudioPlugSharpWPF.
- `GuitarToolkit.Tests` contains xUnit tests for Core behavior.

## Important boundaries

Do not move or modify source code casually.

Safe repository-maintenance work usually includes:

- README/docs edits;
- issue/discussion templates;
- release notes;
- GitHub workflow documentation;
- `.gitignore` updates;
- non-code repository cleanup.

Risky work includes:

- audio processing;
- VST3 runtime packaging;
- DAW host behavior;
- WPF shared UI architecture;
- dependency updates for NAudio, AudioPlugSharp, alphaTab, or AlphaSkia;
- release script changes.

For risky work, keep changes isolated and update relevant documentation/checklists.

## VST notes

- Build and test VST-related changes as `x64`.
- The NuGet-sourced bridge/runtime files in the repository root are intentional and are used for VST deployment:
  - `GuitarToolkit.PluginBridge.vst3`
  - `GuitarToolkit.PluginBridge.runtimeconfig.json`
  - `AudioPlugSharpWPF.dll`
  - `Ijwhost.dll`
- Do not delete or move VST bridge/runtime files unless the release process and manual DAW tests are updated.
- Avoid changing `GuitarToolkitPlugin.Process()` unless necessary. DAW hosts can be sensitive when audio ports are reassigned while a plugin is loaded.
- Do not use blocking operations, file I/O, locks, or frequent logging in the audio callback.
- VST3 package output must include the full plugin folder and runtime dependencies, not only the `.vst3` file.

## Documentation rules

Follow the current documentation hierarchy:

- user-facing docs go in `docs/user/`;
- maintainer/process docs go in `docs/maintainer/`;
- release docs go in `docs/releases/`;
- screenshots go in `docs/images/`;
- GitHub-specific templates/configs go in `.github/`;
- root should stay clean and mostly contain project entry points.

When moving or adding docs:

1. Update `docs/README.md`.
2. Update root `README.md` only if the link is useful to normal users.
3. Update `build-release.ps1` if release packages should include the document.
4. Search for old paths after moving files.

## Release packaging notes

`build-release.ps1` packages current user-facing documentation from:

- `docs/README.md`;
- `docs/user/QUICK_START.md`;
- `docs/user/SUPPORTED_DAWS.md`;
- `docs/user/FL_STUDIO.md`;
- `docs/user/REAPER.md`;
- `docs/user/KNOWN_TAB_IMPORT_ISSUES.md`.

If documentation paths change, update `build-release.ps1` at the same time.

Release ZIP files should not be committed. They belong in GitHub Releases.

## Verification

Run these after code changes:

```powershell
dotnet restore GuitarToolkit.sln
dotnet build GuitarToolkit.sln --no-restore --configuration Debug
dotnet test GuitarToolkit.sln --no-restore --configuration Debug
```

For release packaging changes, also run:

```powershell
powershell -ExecutionPolicy Bypass -File .\build-release.ps1 -Version ci -Configuration Release
```

For VST changes, also test manually in a DAW:

1. Add GuitarToolkit to a track.
2. Assign or change the recording input after the plugin is loaded.
3. Check tuner input, metronome output, and chord/scale playback.
4. Re-scan or restart the DAW after running `deploy-vst.bat`.

## CI and GitHub checks

Expected project checks:

- `CI / Build and test` should pass on push and pull request.
- `CI / Release package check` is manual and may be skipped on normal push.

If GitHub shows `dynamic / submit-nuget`, it comes from Automatic Dependency Submission, not from this repository's normal CI. See `docs/maintainer/GITHUB_SETTINGS.md`.

## Dependency updates

Use `docs/maintainer/DEPENDENCY_POLICY.md` for dependency update rules.

In short:

- test-only updates usually need green CI;
- GitHub Actions major updates need green CI and runner/version note review;
- audio, VST3, tab-rendering, NAudio, AudioPlugSharp, alphaTab, and AlphaSkia updates need manual smoke testing in addition to CI.

## What not to commit

Do not commit:

- `bin/`;
- `obj/`;
- `.vs/`;
- `.idea/`;
- logs;
- crash dumps;
- `TestResults/`;
- coverage outputs;
- release ZIP files;
- local reports;
- user tabs or copyrighted tab files;
- temporary screenshots and exports;
- secrets, tokens, keys, or private local paths.

## Maintainer preference

Prefer:

- small changes;
- clear commit messages;
- preserving working build/release behavior;
- updating links when moving docs;
- keeping root clean;
- documenting risky changes.

Avoid:

- broad unrelated edits;
- silent source-code refactors;
- changing VST/audio behavior without tests/checks;
- leaving duplicate docs in old paths;
- adding internal maintainer docs back into the root README.
