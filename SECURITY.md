# Security Policy

GuitarToolkit is a local Windows desktop/VST3 tool. It does not provide a
network service and does not intentionally process secrets, but security and
crash reports are still welcome.

## Supported versions

Only the latest public release is actively supported for security fixes.

| Version | Supported |
| --- | --- |
| Latest release | Yes |
| Older releases | Best effort |

## Reporting a vulnerability

If the report contains sensitive details, use GitHub private vulnerability
reporting if it is enabled for the repository. If private reporting is not
available, open a GitHub issue with a high-level description and avoid posting
exploit details publicly.

Please include:

- GuitarToolkit version.
- Desktop or VST3 target.
- Windows version.
- Steps to reproduce.
- Logs from `%AppData%\GuitarToolkit\logs` if relevant.

The maintainer will review the report, ask for details if needed, and publish a
fix or mitigation when the issue is confirmed.
