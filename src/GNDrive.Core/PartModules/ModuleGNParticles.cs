using System;
using System.Xml.Serialization;

#if !KSP_STUBS
using KSP.UI.Screens;
#endif

namespace GNDrive.PartModules
{
    /// <summary>
    /// Vessel-level GN particle aggregator and coherence tracker.
    /// Exposes RequestParticles and tracks generation from true/tau drives and condensers.
    /// </summary>
    public class ModuleGNParticles : PartModule
    {
        // Storage and rates
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "GN Capacity", guiUnits = "u")]
        public double particleCapacity = 10000.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "GN Stored", guiUnits = "u")]
        public double particlesStored = 0.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "True Rate", guiUnits = "u/s")]
        public double particleRateTrue = 0.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Tau Rate", guiUnits = "u/s")]
        public double particleRateTau = 0.0;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Coherence", guiUnits = "")]
        public double coherence = 0.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Trans-Am Mult", guiUnits = "x")]
        public double transAmMultiplier = 3.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Trans-Am Dur", guiUnits = "s")]
        public double transAmDuration = 20.0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Trans-Am Cooldown", guiUnits = "s")]
        public double transAmCooldown = 120.0;

        [KSPField(isPersistant = true, guiActive = true, guiName = "Trans-Am Active")] public bool transAmActive = false;
        [KSPField(isPersistant = true, guiActive = true, guiName = "Trans-Am CD", guiUnits = "s")] public double transAmRemaining = 0.0;
        [KSPField(isPersistant = true, guiActive = true, guiName = "Cooldown", guiUnits = "s")] public double cooldownRemaining = 0.0;

        private double _lastFixedTime;

        /// <summary>
        /// Request GN particles from vessel storage. Returns the amount actually provided.
        /// </summary>
        public double RequestParticles(double amount)
        {
            if (amount <= 0) return 0;
            var provided = Math.Min(amount, particlesStored);
            particlesStored -= provided;
            return provided;
        }

        /// <summary>
        /// Add GN particles into storage. Returns the amount actually accepted.
        /// </summary>
        public double AddParticles(double amount)
        {
            if (amount <= 0) return 0;
            var space = Math.Max(0.0, particleCapacity - particlesStored);
            var accepted = Math.Min(amount, space);
            particlesStored += accepted;
            return accepted;
        }

        /// <summary>
        /// Add to true/tau generation rates (called by drive modules).
        /// </summary>
        public void AddTrueRate(double rate) => particleRateTrue += Math.Max(0.0, rate);
        public void AddTauRate(double rate) => particleRateTau += Math.Max(0.0, rate);

        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Activate Trans-Am")]
        public void ActivateTransAm()
        {
            if (cooldownRemaining > 0 || transAmActive) return;
            transAmActive = true;
            transAmRemaining = transAmDuration;
        }

        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Deactivate Trans-Am")]
        public void DeactivateTransAm()
        {
            transAmActive = false;
            cooldownRemaining = transAmCooldown;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            _lastFixedTime = Planetarium.GetUniversalTime();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            double now = Planetarium.GetUniversalTime();
            // Initialize on first FixedUpdate if OnStart hasn't run yet
            if (_lastFixedTime == 0.0)
            {
                _lastFixedTime = now;
            }
            double dt = Math.Max(0.0, now - _lastFixedTime);
            _lastFixedTime = now;

            // compute effective generation
            double baseGen = particleRateTrue + particleRateTau;
            double mult = transAmActive ? transAmMultiplier : 1.0;
            double generated = baseGen * mult * dt;
            AddParticles(generated);

            // simple coherence model: normalized by capacity and activity
            double targetCoherence = Math.Min(1.0, particlesStored / Math.Max(1.0, particleCapacity)) * (transAmActive ? 1.0 : 0.85);
            // smooth
            coherence = Lerp(coherence, targetCoherence, Math.Min(1.0, dt * 0.5));

            // handle trans-am timers
            if (transAmActive)
            {
                transAmRemaining -= dt;
                if (transAmRemaining <= 0)
                {
                    transAmActive = false;
                    cooldownRemaining = transAmCooldown;
                }
            }
            else if (cooldownRemaining > 0)
            {
                cooldownRemaining = Math.Max(0.0, cooldownRemaining - dt);
            }

            // reset per-tick additive rates; providers should add each tick
            particleRateTrue = 0.0;
            particleRateTau = 0.0;
        }

        private static double Lerp(double a, double b, double t) => a + (b - a) * Math.Max(0.0, Math.Min(1.0, t));
    }

    internal static class Planetarium
    {
#if KSP_STUBS
        public static double GetUniversalTime() => DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
#else
        public static double GetUniversalTime() => global::Planetarium.GetUniversalTime();
#endif
    }
}
