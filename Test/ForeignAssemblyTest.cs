using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test {
    [TestClass]
    public class ForeignAssemblyTest {
        [TestMethod]
        public void AreForeignAssembliesUpToDate() {
            var componentProvider = new ComponentProvider();
            bool success;
            int numberOfRepositoryFiles, numberOfCopiedFiles;
            componentProvider.AssemblyRepository.UpdateIncludeFolder(App.SourceFileFullName(), out success, out numberOfRepositoryFiles, out numberOfCopiedFiles);
            Assert.IsTrue(success);
            Assert.AreEqual(0, numberOfCopiedFiles);
        }
    }
}
