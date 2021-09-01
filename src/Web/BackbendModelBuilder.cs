using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web {
    public static class BackbendModelBuilder {
        public static IEdmModel GetEdmModel() {
            var builder = new ODataConventionModelBuilder {
                Namespace = "Aspenlaub.Net.GitHub.CSharp.Backbend",
                ContainerName = "DefaultContainer"
            };
            builder.EntitySet<BackbendFolderToBeArchived>("BackbendFoldersToBeArchived");

            return builder.GetEdmModel();
        }
    }
}