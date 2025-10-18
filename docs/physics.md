# Physics and gameplay approximations

This mod implements approximations of GN particle behaviors for gameplay:

- GN Field heat clamp: cap convective flux to `min(stockFlux, k_field * particleFlux)` and scale drag as `Cd_eff = Cd * (1 – α_field)` while active.
- Structural boost: `breakingForce_eff = breakingForce * (1 + β)` for enabled coating, decaying with particle scarcity.
- Beam saber: continuous drain proportional to contact time; heat/overheat modeled as part temperature accumulation.
- Beam rifle: raycast/projectile drain per shot; optional BDArmory integration later.
- Trans-Am: multiplies particle generation and downstream effects for a short duration; followed by cooldown and heat debuffs.
- Quantization: short-range teleport gated by coherence and particle thresholds; disabled in atmosphere by default (unsafe toggle available for testing).

References are documented in `/docs/CONTRIBUTING.md`.
