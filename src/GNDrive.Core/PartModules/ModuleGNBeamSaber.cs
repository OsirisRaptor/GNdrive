using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN Beam Saber (stub): drains particles while active; placeholder for damage application and blade visuals.
    /// </summary>
    public class ModuleGNBeamSaber : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Saber Active")]
        public bool saberActive = false;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Drain Rate", guiUnits = "u/s")]
        public double drainRate = 10.0;

        private ModuleGNParticles _agg;

        [KSPEvent(guiActive = true, guiName = "Toggle Saber")]
        public void ToggleSaber() => saberActive = !saberActive;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            _agg = VesselServices.GetOrCreate(vessel.id, () => part.gameObject.AddComponent<ModuleGNParticles>());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!saberActive || _agg == null) return;

            double dt = 0.02;
#if !KSP_STUBS
            dt = UnityEngine.Time.fixedDeltaTime;
#endif
            _agg.RequestParticles(Math.Max(0.0, drainRate) * dt);
            // TODO: collision/damage handling (ray/volume), heating, visuals
        }
    }
}
