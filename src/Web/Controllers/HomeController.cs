using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Web.ViewModel;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Web.Controllers {
    public class HomeController : Controller {
        private readonly ISecretRepository vSecretRepository;
        private readonly IFolderResolver vFolderResolver;

        public HomeController(IFolderResolver folderResolver, ISecretRepository secretRepository) {
            vFolderResolver = folderResolver;
            vSecretRepository = secretRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var analyzer = new BackbendFoldersAnalyser(vFolderResolver, vSecretRepository);
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFoldersToBeArchived = (await analyzer.AnalyseAsync(errorsAndInfos)).ToList();
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
