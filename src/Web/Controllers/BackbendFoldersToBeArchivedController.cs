using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web.Controllers {
    public class BackbendFoldersToBeArchivedController : ODataController {
        private readonly ISecretRepository vSecretRepository;
        private readonly IFolderResolver vFolderResolver;

        public BackbendFoldersToBeArchivedController(IFolderResolver folderResolver, ISecretRepository secretRepository) {
            vFolderResolver = folderResolver;
            vSecretRepository = secretRepository;
        }

        [HttpGet, ODataRoute("BackbendFoldersToBeArchived"), EnableQuery]
        public async Task<IActionResult> Get() {
            var analyzer = new BackbendFoldersAnalyser(vFolderResolver, vSecretRepository);
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFoldersToBeArchived = await analyzer.AnalyseAsync(errorsAndInfos);
            return Ok(backbendFoldersToBeArchived);
        }
    }
}
