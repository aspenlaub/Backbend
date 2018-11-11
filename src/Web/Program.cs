using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web {
    public class Program {
        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if DEBUG
                .UseDvin(Constants.BackbendAppId, false, args)
#else
                .UseDvin(Constants.BackbendAppId, true, args)
#endif
                .UseStartup<Startup>();
    }
}
