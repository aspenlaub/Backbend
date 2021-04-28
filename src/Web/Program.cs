using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web {
    public class Program {
        public static async Task Main(string[] args) {
            await (await CreateWebHostBuilderAsync(args)).Build().RunAsync();
        }

        public static async Task<IWebHostBuilder> CreateWebHostBuilderAsync(string[] args) =>
            (await WebHost.CreateDefaultBuilder(args)
#if DEBUG
                .UseDvinAndPeghAsync(Constants.BackbendAppId, false, args))
#else
                .UseDvinAndPeghAsync(Constants.BackbendAppId, true, args))
#endif
                .UseStartup<Startup>();
    }
}
