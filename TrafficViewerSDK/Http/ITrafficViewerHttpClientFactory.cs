using System;
namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Makes http clients
    /// </summary>
    public interface IHttpClientFactory
    {
		/// <summary>
		/// Gets the type of the client to be displayed to the user
		/// </summary>
        string ClientType { get; }
		/// <summary>
		/// Method that makes the client
		/// </summary>
		/// <returns></returns>
        IHttpClient MakeClient();
		/// <summary>
		/// Action to perform on load
		/// </summary>
		void OnLoad();
    }
}
