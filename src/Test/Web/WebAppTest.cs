using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    [TestClass]
    public class WebAppTest {
        private readonly IContainer vContainer;

        public WebAppTest() {
            var builder = new ContainerBuilder().UseDvinAndPegh(new DummyCsArgumentPrompter());
            vContainer = builder.Build();
        }

        [TestMethod]
        public async Task CanGetSecretBackbendApp() {
            var repository = vContainer.Resolve<IDvinRepository>();
            var errorsAndInfos = new ErrorsAndInfos();
            var dvinApp = await repository.LoadAsync(Constants.BackbendAppId, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.IsNotNull(dvinApp);
        }
    }
}
