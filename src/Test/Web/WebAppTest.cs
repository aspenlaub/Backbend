using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Repositories;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    [TestClass]
    public class WebAppTest {

        [TestMethod]
        public async Task CanGetSecretBackbendApp() {
            var repository = new DvinRepository(new ComponentProvider());
            var errorsAndInfos = new ErrorsAndInfos();
            var dvinApp = await repository.LoadAsync(Constants.BackbendAppId, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.IsNotNull(dvinApp);
        }
    }
}
