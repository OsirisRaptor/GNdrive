using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN Reentry Field: clamps convective heat and reduces drag when enabled and powered.
    /// This is a placeholder; real implementation may require Harmony hooks.
    /// </summary>
    public class ModuleGNReentryField : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Reentry Field")]
        public bool enabledField = true;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Drag Reduction", guiUnits = "fraction")]
        public double dragReduction = 0.3; // 0..1

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Heat Clamp K", guiUnits = "")]
        public double heatClampK = 0.5;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Active")]
        public bool active;

        private ModuleGNParticles _agg;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            _agg = VesselServices.GetOrCreate(vessel.id, () => part.gameObject.AddComponent<ModuleGNParticles>());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_agg == null) { active = false; return; }
            if (!enabledField) { active = false; return; }

            // Simple activation check: require some coherence and particles
            active = _agg.coherence > 0.2 && _agg.particlesStored > 1.0;

            double dt = 0.02;
#if !KSP_STUBS
            dt = UnityEngine.Time.fixedDeltaTime;
#endif
            if (active)
            {
                double drain = (dragReduction * 2.0 + heatClampK) * dt;
                _agg.RequestParticles(drain);
            }
        }
    }
}
