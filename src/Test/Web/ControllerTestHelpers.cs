using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Web;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    public class ControllerTestHelpers {
        public static HttpClient CreateHttpClient() {
            var builder = Program.CreateWebHostBuilder(new[] { "" });
            var server = new TestServer(builder);
            return server.CreateClient();
        }
    }
}
