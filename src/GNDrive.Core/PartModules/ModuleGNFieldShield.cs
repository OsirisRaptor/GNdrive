using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN Field Shield (spherical): placeholder logic for particle drain and strength metric.
    /// Future: damage hooks and visual sphere.
    /// </summary>
    public class ModuleGNFieldShield : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Field Enabled")]
        public bool fieldEnabled = true;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Field Draw", guiUnits = "u/s")]
        public double fieldDrawRate = 5.0;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Field Strength")]
        public double fieldStrength = 0.0;

        private ModuleGNParticles _agg;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (part?.gameObject != null)
            {
                _agg = VesselServices.GetOrCreate(vessel.id, () => part.gameObject.AddComponent<ModuleGNParticles>());
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_agg == null) return;
            if (!fieldEnabled) { fieldStrength = 0; return; }

            double dt = 0.02;
#if !KSP_STUBS
            dt = UnityEngine.Time.fixedDeltaTime;
#endif
            double need = Math.Max(0.0, fieldDrawRate) * dt;
            double got = _agg.RequestParticles(need);
            double frac = need <= 0 ? 0 : got / need;
            fieldStrength = frac * (1.0 + _agg.coherence);
        }
    }
}
