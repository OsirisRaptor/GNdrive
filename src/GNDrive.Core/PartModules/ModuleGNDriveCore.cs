using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// True GN Drive core: generates GN particles continuously (semi-perpetual) with a configurable rate.
    /// Registers with vessel-level ModuleGNParticles each physics tick.
    /// </summary>
    public class ModuleGNDriveCore : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "True GN Rate", guiUnits = "u/s")]
        public double trueRate = 10.0;

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
            _agg.AddTrueRate(Math.Max(0.0, trueRate));
        }
    }
}
