using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test {
    [TestClass]
    public class BackbendFoldersTest {
        [TestInitialize]
        public void Initialize() {
            var folder = BackbendFoldersSecret.DefaultFolder;
            if (Directory.Exists(folder)) { return; }

            Directory.CreateDirectory(folder);
        }

        [TestMethod]
        public void CanGetBackbendFoldersDefault() {
            var backbendFoldersSecret = new BackbendFoldersSecret();
            var backbendFolders = backbendFoldersSecret.DefaultValue;
            Assert.AreEqual(1, backbendFolders.Count);
        }

        [TestMethod]
        public void CanGetBackbendFolders() {
            var componentProvider = new ComponentProvider();
            var secretRepository = componentProvider.SecretRepository;
            var backbendFoldersSecret = new BackbendFoldersSecret();
            var backbendFolders = secretRepository.Get(backbendFoldersSecret);
            Assert.IsNotNull(backbendFolders);
            Assert.IsTrue(backbendFolders.Count >= 4);
            var clone = backbendFolders.Clone();
            Assert.AreEqual(backbendFolders.Count, clone.Count);
            for(var i = 0; i < backbendFolders.Count; i ++) {
                Assert.AreEqual(backbendFolders[i].Name, clone[i].Name);
            }

            var foldersOnThisMachine = backbendFolders.FoldersOnThisMachine().ToList();
            Assert.IsTrue(foldersOnThisMachine.Count != 0);
        }
    }
}
