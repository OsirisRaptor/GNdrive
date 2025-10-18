using System;
using System.Collections.Generic;

namespace GNDrive.Utils
{
    /// <summary>
    /// Simple vessel-scoped service locator keyed by vessel GUID.
    /// Allows per-vessel singletons like GN particle aggregator.
    /// </summary>
    public static class VesselServices
    {
        private static readonly Dictionary<Guid, Dictionary<Type, object>> VesselToServices = new();

        public static T GetOrCreate<T>(Guid vesselId, Func<T> factory)
            where T : class
        {
            if (!VesselToServices.TryGetValue(vesselId, out var map))
            {
                map = new Dictionary<Type, object>();
                VesselToServices[vesselId] = map;
            }

            var type = typeof(T);
            if (!map.TryGetValue(type, out var instance))
            {
                instance = factory();
                map[type] = instance;
            }

            return (T)instance;
        }

        public static bool TryGet<T>(Guid vesselId, out T service)
            where T : class
        {
            service = null;
            if (VesselToServices.TryGetValue(vesselId, out var map) && map.TryGetValue(typeof(T), out var instance))
            {
                service = instance as T;
                return service != null;
            }
            return false;
        }

        public static void ClearVessel(Guid vesselId)
        {
            VesselToServices.Remove(vesselId);
        }
    }
}
