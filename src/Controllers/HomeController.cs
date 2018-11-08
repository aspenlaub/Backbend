﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Backbend.ViewModel;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Controllers {
    public class HomeController : Controller {
        private readonly IComponentProvider vComponentProvider;

        public HomeController(IComponentProvider componentProvider) {
            vComponentProvider = componentProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var analyzer = new BackbendFoldersAnalyser(vComponentProvider);
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFoldersToBeArchived = (await analyzer.AnalyseAsync(errorsAndInfos)).ToList();
            var model = new HomeIndexViewModel {
                BackbendFoldersToBeArchivedPerReason = backbendFoldersToBeArchived.Select(b => b.Reason).Distinct()
                    .Select(r => new ReasonAndBackbendFoldersToBeArchived {
                        Reason = r, Folders = backbendFoldersToBeArchived.Where(b => b.Reason == r).Select(b => b.Folder)
                    })
            };
            return View(model);
        }
    }
}
