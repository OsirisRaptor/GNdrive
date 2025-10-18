# GN Drive Rebuild (KSP 1.12.x)

A from-scratch, clean-room implementation of a modular GN Drive system inspired by Mobile Suit Gundam 00, targeting Kerbal Space Program 1.12.x and .NET Framework 4.7.2.

## Features (planned)
- GN particle generation and storage (true, Tau, condensers) with Trans-Am
- GN defensive field and re-entry aero/heat mitigation
- Structural reinforcement via GN coating
- Beam saber and beam rifle using a shared GN particle budget
- Quantization (short-range blink; optional long-range jump)

## Installation
- Copy the `GameData/GNDrive` folder into your KSP `GameData`.
- Requires ModuleManager for the sample parts to function properly (recommended).

## Building
This project targets .NET Framework 4.7.2 and references KSP/Unity assemblies from your local KSP install.

- Set environment variable `KSP_DIR` to your KSP root directory, or set `KSP_DLL_DIR` to the Managed folder (e.g., `<KSP_DIR>/KSP_x64_Data/Managed`).
- Build with MSBuild or Visual Studio on Windows. On Linux/macOS, Mono/MSBuild may work but is not officially supported.

Example (PowerShell):
```powershell
$env:KSP_DIR = "C:\\Games\\KSP_1.12.5"
msbuild GNDrive.sln /p:Configuration=Release
```

Artifacts will be copied to `GameData/GNDrive/Plugins` during packaging in CI.

## Licensing & attribution
- Code and configs: CC BY-NC-SA 4.0 (see `LICENSE`).
- We do not copy code/assets from legacy mods. We acknowledge inspiration from flywlyx's historical GN Drive mod and document compatibility ideas in `/docs/physics.md`.

## Roadmap
See `/docs/CONTRIBUTING.md` for the full plan and references.
