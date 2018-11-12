using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMRest
{
    /// <summary>
    /// Singleton holding a settings instance for the current process. Use only in desktop applications.
    /// </summary>
    public class ASMRestSettingsInstance
    {
        private static object _lock = new object();
        private static ASMRestSettings _instance = null;

        /// <summary>
        /// Singleton holding a settings instance for the current process. Use only in desktop applications.
        /// </summary>
        public static ASMRestSettings Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ASMRestSettings();
                    }
                }
                return _instance;
            }

        }

    }
}
