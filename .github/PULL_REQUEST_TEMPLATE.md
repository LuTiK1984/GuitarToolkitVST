## Summary

- 

## Verification

- [ ] `dotnet build GuitarToolkit.sln --no-restore --configuration Debug`
- [ ] `dotnet test GuitarToolkit.sln --no-restore --configuration Debug`
- [ ] Manual Desktop smoke test, if UI/audio behavior changed
- [ ] Manual VST3/DAW smoke test, if plugin behavior changed

## Checklist

- [ ] Change is focused and scoped
- [ ] `CHANGELOG.md` updated for user-visible changes
- [ ] Docs updated for setup, packaging, or DAW behavior changes
- [ ] `GuitarToolkit.Core` remains free of WPF, NAudio, and AudioPlugSharp
- [ ] Audio callback changes avoid blocking operations, file I/O, locks, and frequent logging

## DAW notes

DAW/version tested:

Relevant logs or screenshots:
