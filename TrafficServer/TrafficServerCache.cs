using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;

namespace TrafficServer
{
	/// <summary>
	/// Stores sets of responses that match a certain request in the traffic log
	/// </summary>
    public class TrafficServerCache : CacheManager<ICacheable>
	{
        protected static object _lock = new object();

        protected static TrafficServerCache _instance;

        private TrafficServerCache()
        {
        }

        /// <summary>
        /// Returns the singleton
        /// </summary>
        public static TrafficServerCache Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TrafficServerCache();
                    }
                }
                return _instance;
            }
        }

		protected override int CalculateWeight(int key)
		{
			try
			{
				TrafficServerResponseSet set = _entries[key].Reserve() as TrafficServerResponseSet;
				int result = _weigthsIndex[key] + set.Count + 1;
				_entries[key].Release();
				return result;
			}
			catch
			{
				return Int32.MaxValue;
			}
		}
	}
	
}
