﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Core {
    [TestClass]
    public class BackbendFoldersTest {
        private readonly IContainer vContainer;

        public BackbendFoldersTest() {
            var builder = new ContainerBuilder().UsePegh(new DummyCsArgumentPrompter());
            vContainer = builder.Build();
        }

        [TestInitialize]
        public void Initialize() {
            var folder = BackbendFoldersSecret.DefaultFolder;
            if (Directory.Exists(folder)) { return; }

            Directory.CreateDirectory(folder);
        }

        [TestMethod]
        public void CanGetBackbendFoldersDefault() {
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFoldersSecret = new BackbendFoldersSecret();
            var backbendFolders = backbendFoldersSecret.DefaultValue;
            backbendFolders.Resolve(vContainer.Resolve<IFolderResolver>(), errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(1, backbendFolders.Count);
        }

        [TestMethod]
        public async Task CanGetBackbendFolders() {
            var secretRepository = vContainer.Resolve<ISecretRepository>();
            var backbendFoldersSecret = new BackbendFoldersSecret();
            var errorsAndInfos = new ErrorsAndInfos();
            var backbendFolders = await secretRepository.GetAsync(backbendFoldersSecret, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.IsNotNull(backbendFolders);
            Assert.IsTrue(backbendFolders.Count >= 3);
            var clone = backbendFolders.Clone();
            Assert.AreEqual(backbendFolders.Count, clone.Count);
            for(var i = 0; i < backbendFolders.Count; i ++) {
                Assert.AreEqual(backbendFolders[i].Name, clone[i].Name);
            }
            backbendFolders.Resolve(vContainer.Resolve<IFolderResolver>(), errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        }
    }
}
