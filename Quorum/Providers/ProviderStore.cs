using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public static class ProviderStore
    {
        public static Dictionary<Type, object> Providers = new Dictionary<Type, object>();
        public static Logger Log = LogManager.GetCurrentClassLogger();

        public static T GetProvider<T>()
        {
            return (T)Providers[typeof(T)];
        }

        public static void AddProvider<T>(T provider)
        {
            if (provider == null)
            {
                Log.Warn("Added null provider of type {0}", typeof(T));
            }

            Providers[typeof(T)] = provider;
        }
    }
}
