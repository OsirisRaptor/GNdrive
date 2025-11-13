using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN Rifle (stub): fires GN beam; consumes particles per shot. Placeholder for raycast/projectile and damage.
    /// </summary>
    public class ModuleGNRifle : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "GN/Shot", guiUnits = "u")]
        public double gnPerShot = 50.0;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Last Fire OK")]
        public bool lastFireOk = false;

        private ModuleGNParticles _agg;

        [KSPEvent(guiActive = true, guiName = "Fire")]
        public void Fire()
        {
            if (_agg == null) return;
            var got = _agg.RequestParticles(Math.Max(0.0, gnPerShot));
            lastFireOk = got >= Math.Max(0.0, gnPerShot) * 0.99;
            if (lastFireOk)
            {
                // TODO: implement raycast/projectile and apply damage
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (part?.gameObject != null)
            {
                _agg = VesselServices.GetOrCreate(vessel.id, () => part.gameObject.AddComponent<ModuleGNParticles>());
            }
        }
    }
}
