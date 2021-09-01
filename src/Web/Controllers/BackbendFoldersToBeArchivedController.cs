using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web.Controllers {
    public class BackbendFoldersToBeArchivedController : ControllerBase {
        private readonly ISecretRepository SecretRepository;
        private readonly IFolderResolver FolderResolver;

        public BackbendFoldersToBeArchivedController(IFolderResolver folderResolver, ISecretRepository secretRepository) {
            FolderResolver = folderResolver;
            SecretRepository = secretRepository;
        }

        [HttpGet, EnableQuery]
        public async Task<IActionResult> Get() {
            var analyzer = new BackbendFoldersAnalyser(FolderResolver, SecretRepository);
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFoldersToBeArchived = await analyzer.AnalyzeAsync(errorsAndInfos);
            return Ok(backbendFoldersToBeArchived);
        }
    }
}
