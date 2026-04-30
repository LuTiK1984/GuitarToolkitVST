# Third-Party Notices

GuitarToolkit uses several open-source libraries and runtime components. This file is a practical project notice, not a substitute for the original license texts published by each project.

## Runtime Dependencies

| Component | Version | Purpose | License | Project |
| --- | --- | --- | --- | --- |
| alphaTab / AlphaTab.Windows | 1.8.2 | Guitar Pro and MusicXML rendering/playback | MPL-2.0 | https://github.com/CoderLine/alphaTab |
| AudioPlugSharp / AudioPlugSharpWPF | 0.7.9 | VST3 plugin bridge and WPF hosting | MIT | https://github.com/mikeoliphant/AudioPlugSharp |
| NAudio | 2.2.1 | Desktop audio input/output | MIT | https://github.com/naudio/NAudio |
| .NET / WPF | 8.0 | Application runtime and desktop UI | Microsoft/.NET licenses | https://dotnet.microsoft.com/ |

## Development/Test Dependencies

| Component | Version | Purpose | License | Project |
| --- | --- | --- | --- | --- |
| xUnit | 2.6.2 | Unit tests | Apache-2.0 | https://github.com/xunit/xunit |
| Microsoft.NET.Test.Sdk | 17.8.0 | Test execution | Microsoft/.NET licenses | https://github.com/microsoft/vstest |

## Notes

- The VST3 format and VST trademark belong to Steinberg Media Technologies GmbH.
- Release packages may include transitive dependencies copied by the .NET SDK or NuGet packages.
- Before any commercial distribution, verify the bundled dependency list and include all license texts required by those projects.
