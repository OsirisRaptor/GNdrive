using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN Armor Coating: boosts structural tolerances when powered.
    /// Applies multipliers to breakingForce, breakingTorque, and crashTolerance.
    /// </summary>
    public class ModuleGNArmorCoating : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Coating Enabled")]
        public bool enabledCoating = true;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Strength Mult", guiUnits = "x")]
        public double strengthMultiplier = 0.25; // +25%

        [KSPField(isPersistant = true, guiActive = true, guiName = "Applied")]
        public bool applied;

        private ModuleGNParticles _agg;
        private float _origBreakingForce;
        private float _origBreakingTorque;
        private float _origCrashTolerance;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (part?.gameObject != null)
            {
                _agg = VesselServices.GetOrCreate(vessel.id, () => part.gameObject.AddComponent<ModuleGNParticles>());
                _origBreakingForce = part.breakingForce;
                _origBreakingTorque = part.breakingTorque;
                _origCrashTolerance = part.crashTolerance;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Restore();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_agg == null) { applied = false; Restore(); return; }
            if (!enabledCoating) { applied = false; Restore(); return; }

            double dt = 0.02;
#if !KSP_STUBS
            dt = UnityEngine.Time.fixedDeltaTime;
#endif
            double demand = Math.Max(0.0, strengthMultiplier) * dt;
            double got = _agg.RequestParticles(demand);
            double frac = demand <= 0 ? 0 : got / demand;

            if (frac > 0.5)
            {
                double mult = 1.0 + strengthMultiplier * frac;
                part.breakingForce = (float)(_origBreakingForce * mult);
                part.breakingTorque = (float)(_origBreakingTorque * mult);
                part.crashTolerance = (float)(_origCrashTolerance * mult);
                applied = true;
            }
            else
            {
                Restore();
                applied = false;
            }
        }

        private void Restore()
        {
            part.breakingForce = _origBreakingForce;
            part.breakingTorque = _origBreakingTorque;
            part.crashTolerance = _origCrashTolerance;
        }
    }
}
