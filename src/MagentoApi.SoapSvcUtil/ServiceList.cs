using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Compori.MagentoApi.SoapSvcUtil
{
    /// <summary>
    /// The class ServiceList provides method in order to retrieve a list of services and to download wsdl definitions.
    /// </summary>
    public class ServiceList
    {
        /// <summary>
        /// Downloads the Wsdl specifications and returns a string.
        /// </summary>
        /// <param name="wsdlUri">The WSDL URI.</param>
        /// <param name="token">The token.</param>
        /// <param name="httpAuthUser">The HTTP authentication user.</param>
        /// <param name="httpAuthPassword">The HTTP authentication password.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        /// <exception cref="InvalidOperationException">Could not connect to server.</exception>
        /// <exception cref="System.InvalidOperationException">Could not connect to server.</exception>
        public static async Task<string> GetWsdlAsync(string wsdlUri, string token, string httpAuthUser, string httpAuthPassword)
        {
            try
            {
                //
                // Prepare default http message handler
                //
                var handler = new HttpClientHandler();
                if (!string.IsNullOrEmpty(httpAuthUser) && !string.IsNullOrEmpty(httpAuthPassword))
                {
                    handler.Credentials = new NetworkCredential(httpAuthUser, httpAuthPassword);
                }

                using var client = new HttpClient(handler);

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(wsdlUri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Could not connect to server.");
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the services asynchronous.
        /// </summary>
        /// <param name="baseEndpointAddress">The base endpoint address.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task&lt;IList&lt;ServiceListItem&gt;&gt;.</returns>
        public static Task<IList<ServiceListItem>> GetServicesAsync(string baseEndpointAddress, string token)
        {
            return GetServicesAsync(baseEndpointAddress, token, null, null);
        }

        /// <summary>
        /// get services as an asynchronous operation.
        /// </summary>
        /// <param name="baseEndpointAddress">The base endpoint address.</param>
        /// <param name="token">The token.</param>
        /// <param name="httpAuthUser">The HTTP authentication user.</param>
        /// <param name="httpAuthPassword">The HTTP authentication password.</param>
        /// <returns>Task&lt;IList&lt;ServiceListItem&gt;&gt;.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<IList<ServiceListItem>> GetServicesAsync(string baseEndpointAddress, string token, string httpAuthUser, string httpAuthPassword)
        {
            try
            {
                //
                // Prepare default http message handler
                //
                var handler = new HttpClientHandler();
                if (!string.IsNullOrEmpty(httpAuthUser) && !string.IsNullOrEmpty(httpAuthPassword))
                {
                    handler.Credentials = new NetworkCredential(httpAuthUser, httpAuthPassword);
                }

                using var client = new HttpClient(handler);

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync(baseEndpointAddress + "/soap/default?wsdl_list=1", null);
                if (!response.IsSuccessStatusCode)
                {
                    var message = "Error: " + response.ReasonPhrase + " (" + ((int)response.StatusCode).ToString() + ").";
                    var additionalMessage = await GetMessageFromResponseContent(response.Content);
                    if (additionalMessage != null)
                    {
                        message = message + " " + additionalMessage;
                    }

                    throw new InvalidOperationException(message);
                }

                //
                // Read all elements under the response tag
                //
                return await GetListFromResponseContent(response.Content);
            }
            catch
            {
                throw;
            }
        }

        private static async Task<IList<ServiceListItem>> GetListFromResponseContent(HttpContent content)
        {
            var result = new List<ServiceListItem>();

            var doc = new XmlDocument();
            doc.Load(await content.ReadAsStreamAsync());
            var nodes = doc.SelectNodes("/response/*");
            foreach (XmlElement element in nodes)
            {
                var endpoint = element.SelectSingleNode("wsdl_endpoint").InnerText;
                endpoint = endpoint.Replace("&amp;", "&");
                result.Add(new ServiceListItem { Name = element.Name, Endpoint = endpoint });
            }
            return result;
        }

        /// <summary>
        /// Gets the message from XML response if possible.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.String.</returns>
        private static async Task<string> GetMessageFromResponseContent(HttpContent content)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(await content.ReadAsStreamAsync());
                var node = doc.SelectSingleNode("/response/message");
                return node?.InnerText;
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}
