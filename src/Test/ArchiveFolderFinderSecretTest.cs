using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CsScriptExecuter = Aspenlaub.Net.GitHub.CSharp.Backbend.Test.CsScriptExecuter;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test {
    [TestClass]
    public class ArchiveFolderFinderSecretTest {
        [TestMethod]
        public void CanGetDefaultArchiveFolderAndSecretGuid() {
            var secret = new ArchiveFolderFinderSecret();
            Assert.IsNotNull(secret.DefaultValue);
            Assert.IsFalse(string.IsNullOrEmpty(secret.Guid));
        }

        [TestMethod]
        public async Task CanGetArchiveFolder() {
            var componentProvider = new ComponentProvider();
            var secretRepository = componentProvider.SecretRepository;
            var secret = new ArchiveFolderFinderSecret();
            var folder = BackbendFoldersSecret.DefaultFolder;
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            var archiveFolder = await secretRepository.ExecuteCsScriptAsync(secret.DefaultValue, new List<ICsScriptArgument> { new CsScriptArgument { Name = "folder", Value = folder } });
            Assert.IsTrue(archiveFolder.Length != 0);
            Assert.IsTrue(archiveFolder != folder);
            Assert.IsTrue(Directory.Exists(archiveFolder));
            Assert.AreEqual(0, Directory.GetFiles(archiveFolder).Length);
            Directory.Delete(archiveFolder);
            var otherArchiveFolder = await secretRepository.ExecuteCsScriptAsync(secret.DefaultValue, new List<ICsScriptArgument> { new CsScriptArgument { Name = "folder", Value = folder } });
            Assert.AreEqual(archiveFolder, otherArchiveFolder);
            Assert.IsTrue(Directory.Exists(archiveFolder));
            otherArchiveFolder = await secretRepository.ExecuteCsScriptAsync(secret.DefaultValue, new List<ICsScriptArgument> { new CsScriptArgument { Name = "folder", Value = folder } });
            Assert.IsTrue(Directory.Exists(otherArchiveFolder));
        }
    }
}
