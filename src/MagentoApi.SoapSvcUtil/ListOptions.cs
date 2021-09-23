using CommandLine;

namespace Compori.MagentoApi.SoapSvcUtil
{
    /// <summary>
    /// Class ListOptions.
    /// </summary>
    [Verb("list", HelpText = "Lists all available services.")]
    public class ListOptions
    {
        /// <summary>
        /// Gets or sets the base endpoint address.
        /// </summary>
        /// <value>The base endpoint address.</value>
        [Option(shortName: 'a', longName: "base-endpoint-address", HelpText = "Endpoint address to shop", Required = true)]
        public string BaseEndpointAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        [Option(longName: "user-agent", HelpText = "User agent.")]
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the name of the admin token request user.
        /// </summary>
        /// <value>The name of the admin token request user.</value>
        [Option(shortName: 't', longName: "access-token", HelpText = "Access token.", Required = false)]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the name of the admin token request user.
        /// </summary>
        /// <value>The name of the admin token request user.</value>
        [Option(shortName: 'u', longName: "admin-token-request-user", HelpText = "Admin user to generate access token.", Required = false)]
        public string AdminTokenRequestUserName { get; set; }

        /// <summary>
        /// Gets or sets the admin token request password.
        /// </summary>
        /// <value>The admin token request password.</value>
        [Option(shortName: 'p', longName: "admin-token-request-password", HelpText = "Admin user to generate access token.", Required = false)]
        public string AdminTokenRequestPassword { get; set; }

        /// <summary>
        /// Gets or sets the HTTP authentication user.
        /// </summary>
        /// <value>The HTTP authentication user.</value>
        [Option(longName: "http-auth-user", HelpText = "Http authentification user name.")]
        public string HttpAuthUser { get; set; }

        /// <summary>
        /// Gets or sets the HTTP authentication password.
        /// </summary>
        /// <value>The HTTP authentication password.</value>
        [Option(longName: "http-auth-password", HelpText = "Http authentification user name.")]
        public string HttpAuthPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [output raw file].
        /// </summary>
        /// <value><c>true</c> if [output raw file]; otherwise, <c>false</c>.</value>
        [Option(shortName: 'o', longName: "output-file", HelpText = "Write the services into a file.")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [output services file].
        /// </summary>
        /// <value><c>true</c> if [output services file]; otherwise, <c>false</c>.</value>
        [Option(longName: "with-service-endpoint", HelpText = "Add the services endpoint sperated with a ';' after service name.")]
        public bool WithServiceEndpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ListOptions"/> is silent.
        /// </summary>
        /// <value><c>true</c> if silent; otherwise, <c>false</c>.</value>
        [Option(longName: "silent", HelpText = "Suppress info output.")]
        public bool Silent { get; set; }
    }
}
