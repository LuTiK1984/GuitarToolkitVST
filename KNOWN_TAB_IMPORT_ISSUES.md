# Known Tab Import Issues

This file tracks tablature files that fail before GuitarToolkit can render them because the alphaTab importer throws while parsing the source file.

## alphaTab GP4 chord import crash

- File: `samples/tabs/bach_johann_sebastian-toccata_fugue_in_dm_bwv_565_metal_version.gp4`
- Observed: 2026-05-02
- Symptom: opening the file fails during import with `System.IndexOutOfRangeException`.
- Stack location: `AlphaTab.Importer.Gp3To5Importer.ReadChord(Beat beat)`.
- GuitarToolkit behavior: the failed file is logged, hidden from the current tab picker list for the session, and removed from recent/favorite lists so it is not retried repeatedly by accident.

Notes:

- The exception is thrown inside alphaTab before a `Score` is available, so GuitarToolkit cannot repair the parsed model after loading.
- Keep a compatible alternate export beside the file when possible, such as GP3, GP5/GPX, or MusicXML.
- Re-test this file after alphaTab package upgrades.
