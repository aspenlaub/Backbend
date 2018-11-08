using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    [TestClass]
    public class WebAppTest {

        [TestMethod]
        public async Task CanGetSecretBackbendApp() {
            var repository = new DvinRepository();
            var dvinApp = await repository.LoadAsync(Constants.BackbendAppId);
            Assert.IsNotNull(dvinApp);
            var folder = dvinApp.FolderOnMachine(Environment.MachineName);
            Assert.IsNotNull(folder);
        }
    }
}
