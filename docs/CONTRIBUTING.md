# GN Drive Rebuild - Contributor Guide and Project Plan

Paste of the initial Cursor prompt and acceptance criteria for traceability.

---

Project: “GN Drive Rebuild (KSP1, .NET 4.7.2, KSP 1.12.x)”

Goal: Implement a modular GN Drive system in Kerbal Space Program that reproduces the following canonical effects, with tunable, physically-motivated parameters and clean separation of concerns:

GN particle generation & storage (true GN Drive vs. GN Tau vs. condensers) with Trans-Am burst; particles feed all downstream systems. 

GN Field for defense and shaped fields for aerothermal mitigation during atmospheric entry; attachable as a spherical or conformal shell around the craft/collider set. 

Structural reinforcement via GN particle “coating/field within armor gaps” (boost joint/connection strength and crash/impact tolerances). 

Beam saber (positron-field container holding compressed GN particles as a blade) and beam rifle (GN particle beam), with shared GN-particle budget and heat/overheat controls. 

Quantization/Wormhole: short-range “quantum teleportation”/blink that relocates the vessel within a bounded radius when particle density and coherence exceed thresholds (00’s “quantization”/teleport behavior). Provide optional long-range jump as a gated late-tech feature. 

Source to fork/learn from: the historical KSP GN Drive mod by flywlyx (SpaceDock → GitHub). We are starting a fresh codebase but we’ll mirror good ideas in architecture/CFGs and keep license compatibility notes. 

KSP/tech stack: KSP 1.12.x, C#/.NET Framework 4.7.2, PartModule API; optional HarmonyKSP for patches; optional KSPCommunityPartModules utilities. Provide editor UI (PAW) fields, action group hooks, and ModuleManager-friendly config. 

Deliverables

Assemblies: GNDrive.Core.dll + optional sub-assemblies per feature.

Parts: GNDrive, GNTauDrive, GNCondenser, GNFieldEmitter, GNSaber, GNRifle. Each ships with .cfg + simple placeholder models.

Docs: /docs/README.md (install, dependencies, balancing), /docs/physics.md (our approximations and links), /CHANGELOG.md.

CI: GitHub Actions pipeline to build on tag; zip release with GameData layout.

High-level architecture

Core (no Unity colliders yet—just API)

ModuleGNParticles (singleton per vessel):

States: particleCapacity, particleRateTrue, particleRateTau, transAmMultiplier, transAmCooldown.

Exposes RequestParticles(double amount); tracks “coherence” (used by field/quantization).

TransAm event: temporary rate+coherence spike with heat & wear debuffs (post-buff decay).

Save/load via KSPField; PAW sliders for test tuning. (PartModule patterns per KSP API doc.) 

ModuleGNDriveCore (true drive) & ModuleGNTauCore (tau):

True drive: “semi-perpetual” emission (moment-limited but time-indefinite). Tau: converts EC→GN at set efficiency. 

Optional ModuleGNCondenser for buffer tanks (fill/vent logic). 

Defensive/Aero Field

ModuleGNFieldShield:

Generates spherical or mesh-conformal field. Blocks configurable fractions of kinetic and beam damage (plugins can hook). 

Particle drain ∝ incoming flux; failure when storage empty or coherence too low.

ModuleGNReentryField:

When in atmo and Mach>threshold, apply: (a) drag-reduction factor, (b) shock heating clamp (cap convective flux), (c) GN-particle power draw. Tweakable to allow “casual re-entry” when properly powered (as depicted). 

Structural reinforcement

ModuleGNArmorCoating:

On enable: multiply breakingForce, breakingTorque, crashTolerance on attached part tree; decay multipliers when particles low. Rationale: GN fielding within armor gaps increases durability/resists deformation. 

Weapons

ModuleGNBeamSaber:

Emits a blade collider whose contact deals damage ∝ particle flow; positron-field container metaphor is descriptive only—implement as a controlled GN particle drain + heat. 

ModuleGNRifle:

Fires raycast or projectile with GN beam damage; reuses particle budget. Provide native mode first; later add BDArmory integration as optional wrapper. 

Quantization/Wormhole

ModuleGNQuantize:

Short-range blink: on hotkey, teleport vessel by Δr within safe radius if coherence>θ & particles>cost.

Long-range jump (experimental): gated behind high coherence + dual-drive synergy; add nav UI to pick a local SOI waypoint. (This mirrors 00’s quantization portrayal; treat as optional realism toggle.) 

Physics/gameplay approximations (document in /docs/physics.md)

GN Field heat clamp: cap convective flux to min(stockFlux, k_field * particleFlux) and scale drag by Cd_eff = Cd * (1 – α_field) while field active. 

Structural boost: breakingForce_eff = breakingForce * (1+β); tune β via tests. 

Beam saber: per-tick particle drain ∝ contact time; apply damage via stock hit events or BDArmory API (if present). 

Trans-Am: multiply particle rates and thrust/field strength for t_transam, then enforce cooldown and heat. (UI glow optional.)

Quantization: disallow if inside atmosphere by default; add “unsafe” dev flag for testing. 

Code plan (tasks)

- Scaffold solution: .sln + GameData/GNDrive/Plugins, Part prefabs, sample .cfg.
- Implement ModuleGNParticles + unit tests (where feasible) + in-game PAW.
- Implement ModuleGNDriveCore, ModuleGNTauCore, ModuleGNCondenser.
- Implement ModuleGNFieldShield (spherical first), then ModuleGNReentryField.
- Implement ModuleGNArmorCoating.
- Implement ModuleGNBeamSaber and ModuleGNRifle (native), then optional BDArmory adapter.
- Implement ModuleGNQuantize.
- Balancing pass + config curves; write /docs/physics.md with citations.
- CI pipeline (GitHub Actions) and packaged release zip.

Integration/deps

- KSP PartModule API (1.12.x), patterns for KSPField/OnStart/FixedUpdate.
- HarmonyKSP (optional) for safe runtime patches.
- KSPCommunityPartModules (optional utilities).

Licensing & attribution

The historical mod on SpaceDock is CC BY-NC-SA 4.0; we are not copying code, but we will (a) acknowledge inspiration, (b) avoid asset reuse without permission, and (c) license our repo CC BY-NC-SA 4.0 to preserve ecosystem norms.

Acceptance tests (in-game)

- Field: withstand stock 1,500 m/s re-entry at ~20 km periapsis on Kerbin without exploding; GN particles drop predictably.
- Structure: drop test from 100 m → fewer joint breaks vs. baseline.
- Weapons: saber damages target parts with continuous drain; rifle hits consume particles.
- Quantize: 2 km blink outside atmosphere with state preserved; long-range jump gated.

Version-control plan

A. Create a clean “from-scratch” repo while preserving upstream references

- Fork the historical mod only for reference (don’t base your code on it): `git remote add legacy https://github.com/flywlyx/GNdrive.git`
- `git fetch legacy`
- Keep our main clean; use `git range-diff` or `git show` for learning only.

B. Branching model

- `main` = stable; protected.
- `dev` = integration branch.
- Feature branches: `feat/gnparticles-core`, `feat/field-shield`, `feat/reentry-field`, `feat/armor-coating`, `feat/saber`, `feat/rifle`, `feat/quantize`.
- Hotfix: `fix/*`.
- Tag releases: v0.1.0 (particles+true/tau+condenser), v0.2.0 (+field/reentry), v0.3.0 (+structure), v0.4.0 (+weapons), v0.5.0 (+quantize), v1.0.0 (balance/docs/CI).

C. CI/release

- Build on tag for KSP 1.12.x
- Zip `GameData/GNDrive/**` for release artifact

D. Dependencies

- HarmonyKSP (optional) as submodule or documented manual install

---

Issue #1 Checklist (create from this template)

- [ ] Scaffold solution and GameData layout
- [ ] Implement ModuleGNParticles vessel aggregator and PAW
- [ ] Implement true drive, Tau drive, and condenser
- [ ] Implement GN Field (spherical) and reentry clamps
- [ ] Implement armor coating multipliers
- [ ] Implement saber and rifle stubs + shared drain
- [ ] Implement quantization (blink)
- [ ] Balancing/config curves and `/docs/physics.md`
- [ ] CI workflow and tagged release packaging
