using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Attributes;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Backbend {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddOData();
            services.AddMvc(config => config.Filters.Add(new DvinExceptionFilterAttribute()));

            services.AddTransient<IComponentProvider, ComponentProvider>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvc(routebuilder => {
                routebuilder.Select().Expand().Filter().OrderBy(QueryOptionSetting.Allowed).MaxTop(null).Count();
                routebuilder.MapODataServiceRoute("ODataRoute", "odata", GetEdmModel());
                routebuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static IEdmModel GetEdmModel() {
            var builder = new ODataConventionModelBuilder {
                Namespace = "Aspenlaub.Net.GitHub.CSharp.Backbend",
                ContainerName = "DefaultContainer"
            };
            builder.EntitySet<BackbendFolderToBeArchived>("BackbendFoldersToBeArchived");

            return builder.GetEdmModel();
        }
    }
}
