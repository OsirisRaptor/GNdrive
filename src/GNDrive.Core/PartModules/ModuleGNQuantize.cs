using System;
using GNDrive.Utils;

namespace GNDrive.PartModules
{
    /// <summary>
    /// GN Quantization (blink): teleports vessel within a radius if coherence and particles are sufficient.
    /// Real teleportation requires KSP vessel repositioning; here we stub logic and particle checks.
    /// </summary>
    public class ModuleGNQuantize : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Blink Cost", guiUnits = "u")]
        public double blinkCost = 500.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Blink Radius", guiUnits = "m")]
        public double blinkRadius = 2000.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Min Coherence", guiUnits = "")]
        public double minCoherence = 0.6;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Last Blink OK")]
        public bool lastBlinkOk = false;

        private ModuleGNParticles _agg;

        [KSPEvent(guiActive = true, guiName = "Blink")]
        public void Blink()
        {
            if (_agg == null) return;
            if (_agg.coherence < minCoherence) { lastBlinkOk = false; return; }

            var got = _agg.RequestParticles(Math.Max(0.0, blinkCost));
            if (got < blinkCost * 0.99) { lastBlinkOk = false; return; }

            // TODO: perform vessel reposition within blinkRadius (requires flight scene API)
            lastBlinkOk = true;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            _agg = VesselServices.GetOrCreate(vessel.id, () => part.gameObject.AddComponent<ModuleGNParticles>());
        }
    }
}
