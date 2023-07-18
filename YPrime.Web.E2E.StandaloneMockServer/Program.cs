using System;
using System.Threading.Tasks;
using WireMock.Net.StandAlone;
using WireMock.Settings;

namespace YPrime.Web.E2E.StandaloneMockServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var useLocalMockServer = false;
            if (args.Length > 0)
            {
                bool.TryParse(args[0], out useLocalMockServer);
            }

            var server = new MockServer.MockServer();

            if (useLocalMockServer)
            {
                var settings = new WireMockServerSettings
                {
                    StartAdminInterface = true,
                    Port = MockServer.MockServer.mockServerPort
                };
                StandAloneApp.Start(settings);
            }
            await server.ClearMockResources();
            await server.SetupInitialMappings();

            Console.WriteLine("Press any key Delete All the mappings");
            Console.ReadKey();
            await server.ClearMockResources();
        }
    }
}
