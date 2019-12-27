using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Web;
using Microsoft.AspNetCore.Hosting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    public class ControllerTestHelpers {
        public static HttpClient CreateHttpClient() {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);
            return server.CreateClient();
        }
    }
}
