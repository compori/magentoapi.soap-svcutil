using CommandLine;

namespace Compori.MagentoApi.SoapSvcUtil
{
    [Verb("build", HelpText = "Builds services.")]
    public class BuildOptions
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
        /// Gets or sets the output folder.
        /// </summary>
        /// <value>The output folder.</value>
        [Option(shortName: 's', longName: "service-file", HelpText = "File with services. Each service name has its own line", Required = true)]
        public string ServiceFile { get; set; }

        /// <summary>
        /// Gets or sets the output folder.
        /// </summary>
        /// <value>The output folder.</value>
        [Option(shortName: 'o', longName: "output-folder", HelpText = "Folder to write generated services files.", Required = true)]
        public string OutputFolder { get; set; }

        /// <summary>
        /// Gets or sets the base namespace.
        /// </summary>
        /// <value>The base namespace.</value>
        [Option(shortName: 'n', longName: "base-namespace", HelpText = "Base namespace of generated services classes.", Required = true)]
        public string BaseNamespace { get; set; }
    }
}
