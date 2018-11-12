using System;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Allows hooking up a credentials provider
	/// </summary>
    public interface ICredentialsProvider
    {

        /// <summary>
        /// Executes the provider. Returns false on failure
        /// </summary>
        /// <returns></returns>
        bool Execute(out string domain, out string userName, out string password);
    }
}
