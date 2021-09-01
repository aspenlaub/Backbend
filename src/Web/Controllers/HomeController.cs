using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Web.ViewModel;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web.Controllers {
    public class HomeController : Controller {
        private readonly ISecretRepository SecretRepository;
        private readonly IFolderResolver FolderResolver;

        public HomeController(IFolderResolver folderResolver, ISecretRepository secretRepository) {
            FolderResolver = folderResolver;
            SecretRepository = secretRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var analyzer = new BackbendFoldersAnalyser(FolderResolver, SecretRepository);
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFoldersToBeArchived = (await analyzer.AnalyzeAsync(errorsAndInfos)).ToList();
            var model = new HomeIndexViewModel {
                BackbendFoldersToBeArchivedPerReason = backbendFoldersToBeArchived.Select(b => b.Reason).Distinct()
                    .Select(r => new ReasonAndBackbendFoldersToBeArchived {
                        Reason = r, Folders = backbendFoldersToBeArchived.Where(b => b.Reason == r).Select(b => b.Folder)
                    }),
                Errors = errorsAndInfos.Errors
            };
            return View(model);
        }
    }
}
