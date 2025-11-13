using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN Tau core: converts ElectricCharge to GN particles at a fixed efficiency.
    /// Consumes EC and contributes to vessel GN generation.
    /// </summary>
    public class ModuleGNTauCore : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Tau Rate", guiUnits = "u/s")]
        public double tauRate = 10.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "EC per GN", guiUnits = "EC/u")]
        public double ecPerGn = 1.0;

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

            double desiredGnPerSec = Math.Max(0.0, tauRate);
            double dt = 0.02; // approximate; in-game use Time.fixedDeltaTime
#if !KSP_STUBS
            dt = UnityEngine.Time.fixedDeltaTime;
#endif
            double gnThisTick = desiredGnPerSec * dt;
            double ecNeeded = gnThisTick * ecPerGn;

            // attempt EC draw
            double ecDrawn = part.RequestResource("ElectricCharge", ecNeeded);
            if (ecDrawn > 0)
            {
                double gnProduced = ecDrawn / Math.Max(1e-6, ecPerGn);
                _agg.AddTauRate(gnProduced / Math.Max(1e-6, dt));
            }
        }
    }
}
