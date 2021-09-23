using CommandLine;
using Compori.MagentoApi.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Compori.MagentoApi.SoapSvcUtil
{
    class Program
    {
        /// <summary>
        /// Lists the services.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        private static async Task<int> ListInternal(ListOptions options)
        {
            try
            {
                var tokenRequest = new TokenRequest() as ITokenRequest;

                
                var token = !string.IsNullOrWhiteSpace(options.AccessToken) ? options.AccessToken : await tokenRequest.RequestTokenAsync(
                    TokenType.Admin,
                    options.BaseEndpointAddress,
                    options.UserAgent,
                    options.AdminTokenRequestUserName, options.AdminTokenRequestPassword,
                    options.HttpAuthUser, options.HttpAuthPassword);

                var services = await ServiceList.GetServicesAsync(
                    options.BaseEndpointAddress,
                    token,
                    options.HttpAuthUser, options.HttpAuthPassword);

                var output = new StringBuilder();

                foreach (var item in services)
                {
                    var line = item.Name;
                    if (options.WithServiceEndpoint)
                    {
                        line += ";" + item.Endpoint;
                    }
                    if (!options.Silent)
                    {
                        Console.WriteLine(line);
                    }
                    output.AppendLine(line);
                }

                if (!string.IsNullOrEmpty(options.OutputFile))
                {
                    File.WriteAllText(options.OutputFile, output.ToString(), Encoding.UTF8);
                }
                return 0;

            }
            catch (Exception ex)
            {

                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return -1;
            }
        }
        
        /// <summary>
        /// Builds the specified file information.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public static async Task<int> CreateSourceFile(string sourceFile, string serviceName, string outputFolder, string baseNamespace)
        {
            var outputFile = Path.GetFullPath(Path.Combine(outputFolder, serviceName + ".cs"));

            // Delete default output directory of svcutil
            if (Directory.Exists("ServiceReference1"))
            {
                Directory.Delete("ServiceReference1", true);
            }

            // create our output directory if not exists
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Run svcutil
            var parameter = new string[]
                {
                    sourceFile,
                    "-nl",      // No logo or banner
                    "-v",       // Silent
                    "Silent",
                    "-n",       // namespace
                    "*, " + (string.IsNullOrEmpty(baseNamespace) ? serviceName : baseNamespace + "." + serviceName)
                };
            int result = 0;
            var restoreOutStream = Console.Out;
            var path = "";
            try
            {
                var sb = new StringBuilder();
                using(var text = new StringWriter(sb))
                {
                    Console.SetOut(text);
                    var source = new CancellationTokenSource();
                    result = await Microsoft.Tools.ServiceModel.Svcutil.Tool.MainAsync(parameter, source.Token);
                }
                // path = sb.ToString().TrimEnd(new char[] { '\n', '\r', ' ' });
                path = @"ServiceReference1\Reference.cs";
            }
            finally
            {
                Console.SetOut(restoreOutStream);
            }

            if (result == 0 && File.Exists(path))
            {
                // Move Reference.cs to output file
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                Console.WriteLine(path + " -> " + outputFile);
                File.Move(path, outputFile);
                Directory.Delete("ServiceReference1");
                return 0;

            }
            return -1;
        }

        /// <summary>
        /// Builds the specified cancellation token.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        private static async Task<int> BuildAsync(BuildOptions options)
        {
            var tokenRequest = new TokenRequest() as ITokenRequest;

            var token = !string.IsNullOrWhiteSpace(options.AccessToken) ? options.AccessToken : await tokenRequest.RequestTokenAsync(
                TokenType.Admin,
                options.BaseEndpointAddress,
                options.UserAgent,
                options.AdminTokenRequestUserName, options.AdminTokenRequestPassword,
                options.HttpAuthUser, options.HttpAuthPassword);

            var services = await ServiceList.GetServicesAsync(
                options.BaseEndpointAddress,
                token,
                options.HttpAuthUser, options.HttpAuthPassword);

            var serviceNames = File.ReadAllLines(options.ServiceFile);

            var serviceFiles = new Dictionary<string, string>();

            try
            {

                foreach (var item in serviceNames)
                {

                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }

                    var serviceName = item.Trim();
                    if (serviceName.StartsWith("#"))
                    {
                        continue;
                    }

                    //
                    // Find service endpoint
                    //
                    var service = services.FirstOrDefault(v => v.Name.Equals(serviceName));
                    if(service == null)
                    {
                        continue;
                    }

                    var tempPath = Path.GetTempFileName();
                    var serviceWsdlUri = service.Endpoint;

                    Console.Write("Download service " + serviceName);

                    //
                    // write content to tempfile and build it.
                    //
                    try
                    {
                        File.WriteAllText(tempPath, await ServiceList.GetWsdlAsync(serviceWsdlUri, token, options.HttpAuthUser, options.HttpAuthPassword), Encoding.UTF8);
                        serviceFiles.Add(serviceName, tempPath);
                        Console.WriteLine(" ... OK");
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(" ... Error:" + ex.Message);
                    }
                }

                foreach (var serviceFile in serviceFiles)
                {
                    var fileName = serviceFile.Value;
                    var serviceName = serviceFile.Key;

                    await CreateSourceFile(
                        fileName,
                        serviceName.Substring(0, 1).ToUpperInvariant() + serviceName[1..],
                        options.OutputFolder,
                        options.BaseNamespace);

                    Console.WriteLine("Build service " + serviceName);
                }
            }
            catch (Exception ex)
            {

                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return -1;
            }
            finally
            {
                    Parallel.ForEach(serviceFiles, service => File.Delete(service.Value));
            }
            return 0;
        }
        
        /// <summary>
        /// Lists the and return exit code.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>System.Int32.</returns>
        public static int List(ListOptions options)
        {
            return ListInternal(options).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Builds the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>System.Int32.</returns>
        public static int Build(BuildOptions options)
        {
            return BuildAsync(options).GetAwaiter().GetResult();
        }
        
        #region Entry Point

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<ListOptions, BuildOptions>(args).MapResult(
                (ListOptions opts) => List(opts),
                (BuildOptions opts) => Build(opts),
                errs => 1);
            return result;
        }

        #endregion
    }
}
