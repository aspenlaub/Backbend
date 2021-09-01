using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Web.Attributes;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var model = BackbendModelBuilder.GetEdmModel();
            services.AddControllers()
                .AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", model)
                    .Conventions.Add(new BackbendConvention())
                );

            services.AddRazorPages(); // for MapRazorPages()

            services.UseDvinAndPegh(new DummyCsArgumentPrompter());

            services.AddControllers(opt => {
                DvinExceptionFilterAttribute.SetExceptionLogFolder(new Folder(Path.GetTempPath()).SubFolder("AspenlaubExceptions"));
                opt.Filters.Add<DvinExceptionFilterAttribute>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting(); // for UseEndpoints()

            app.UseODataRouteDebug("odata"); // for display of end points at http://localhost:65169/odata
            app.UseODataQueryRequest();

            app.Use(next => context => {
                var endpoint = context.GetEndpoint();
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (endpoint == null) {
                    return next(context);
                }

                return next(context);
            });

            app.UseEndpoints(ConfigureEndpoints);
        }

        private static void ConfigureEndpoints(IEndpointRouteBuilder endpoints) {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute(); // for http://localhost:65169/
            endpoints.MapRazorPages(); // for e.g. return View("Index");
        }
    }
}
