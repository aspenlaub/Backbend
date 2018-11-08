using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Controllers {
    public class BackbendFoldersToBeArchivedController : ODataController {
        private readonly IComponentProvider vComponentProvider;

        public BackbendFoldersToBeArchivedController(IComponentProvider componentProvider) {
            vComponentProvider = componentProvider;
        }

        [HttpGet, ODataRoute("BackbendFoldersToBeArchived"), EnableQuery]
        public async Task<IActionResult> Get() {
            var analyzer = new BackbendFoldersAnalyser(vComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFoldersToBeArchived = await analyzer.AnalyseAsync(errorsAndInfos);
            return Ok(backbendFoldersToBeArchived);
        }
    }
}
