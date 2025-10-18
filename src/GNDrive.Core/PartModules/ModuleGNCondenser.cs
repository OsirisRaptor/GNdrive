using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN condenser: provides additional particle capacity and passive fill/vent controls.
    /// </summary>
    public class ModuleGNCondenser : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Extra Capacity", guiUnits = "u")]
        public double extraCapacity = 5000.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Passive Fill", guiUnits = "u/s")]
        public double passiveFillRate = 0.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Passive Vent", guiUnits = "u/s")]
        public double passiveVentRate = 0.0;

        private ModuleGNParticles _agg;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            _agg = VesselServices.GetOrCreate(vessel.id, () => part.gameObject.AddComponent<ModuleGNParticles>());
            if (_agg != null)
            {
                _agg.particleCapacity += Math.Max(0.0, extraCapacity);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_agg != null)
            {
                _agg.particleCapacity = Math.Max(0.0, _agg.particleCapacity - Math.Max(0.0, extraCapacity));
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_agg == null) return;

            double dt = 0.02;
#if !KSP_STUBS
            dt = UnityEngine.Time.fixedDeltaTime;
#endif
            if (passiveFillRate > 0)
            {
                _agg.AddParticles(passiveFillRate * dt);
            }
            if (passiveVentRate > 0)
            {
                _agg.RequestParticles(passiveVentRate * dt);
            }
        }
    }
}
